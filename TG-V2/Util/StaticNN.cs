using System;
using System.Threading;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.KMeans;
using Util.Models;

namespace Util
{
    public static class StaticNN
    {
        static readonly ThreadLocal<BasicNetwork> network =
            new ThreadLocal<BasicNetwork>(() =>
            {
                var result = new BasicNetwork();
                result.AddLayer(new BasicLayer(null, false, 3)); // entradas

                for (int i = 0; i < Global.NNHiddenLayers.Length; i++) // hidden
                    result.AddLayer(new BasicLayer(new ActivationSigmoid(), Global.NNBiases[i], Global.NNHiddenLayers[i]));

                result.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1)); // saída
                return result;
            });

        public static void SetWeights(double[] weights)
        {
            int count = 0;
            var layers = new ConcactableArray<int>() { 3, Global.NNHiddenLayers, 1 };

            for (int layer = 0; layer < layers.Length - 1; layer++)
            {
                var A = layers[layer];
                var B = layers[layer + 1];

                for (int i = 0; i < A; i++)
                    for (int j = 0; j < B; j++)
                    {
                        network.Value.SetWeight(layer, i, j, weights[count]);
                        count++;
                    }
            }
        }

        public static BlackjackMove Compute(BlackjackInformation info)
        {
            var NN = network.Value;
            var result = NN.Compute(new BlackjackMLData(info))[0];
            BlackjackMove move = BlackjackMove.Stand;
            if (result >= 0 && result < 0.5)
                move = BlackjackMove.DoubleDown;
            else if (result >= 0.5 && result < 1)
                move = BlackjackMove.Split;
            else if (result > 1)
                move = BlackjackMove.Hit;
            return move;
        }
    }

    class BlackjackMLData : IMLData
    {
        private BlackjackInformation innerInfo;
        private double[] inner;
        public double this[int x] => inner[x];
        public int Count => inner.Length;

        public BlackjackMLData(BlackjackInformation information)
        {
            innerInfo = information;
            inner = new double[3]
            {
                (double)information.PlayerSum,
                (double)information.DealerFaceupCard.FaceValue,
                information.IsSplit ? 1 : 0
            };
        }

        public object Clone() => new BlackjackMLData(innerInfo);
        public void CopyTo(double[] target, int targetIndex, int count) => Buffer.BlockCopy(inner, 0, target, targetIndex, count);
        public ICentroid<IMLData> CreateCentroid() => null;
    }
}