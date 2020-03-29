using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.KMeans;
using Util.Models;
using Encog.ML.Data.Basic;

namespace Util
{
    public static class StaticNN
    {
        static readonly ThreadLocal<BasicNetwork> network =
            new ThreadLocal<BasicNetwork>(() =>
            {
                var result = new BasicNetwork();
                result.AddLayer(new BasicLayer(null, false, Global.NNInputLayer)); // entradas

                for (int i = 0; i < Global.NNHiddenLayers.Length; i++) // hidden
                    result.AddLayer(new BasicLayer(new ActivationSigmoid(), Global.NNBiases[i], Global.NNHiddenLayers[i]));

                result.AddLayer(new BasicLayer(new ActivationSigmoid(), false, Global.NNOutputLayer)); // saída
                result.Structure.FinalizeStructure();
                return result;
            });

        public static void SetWeights(double[] weights)
        {
            int count = 0;
            var layers = new ConcactableArray<int>() { Global.NNInputLayer, Global.NNHiddenLayers, Global.NNOutputLayer };

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
            double[] input = new double[Global.NNInputLayer]
            {
                (double)info.PlayerSum,
                (double)info.DealerFaceupCard.FaceValue,
                info.IsSplit ? 1 : 0,
                (double)info.PlayerHand[0].FaceValue,
                (double)info.PlayerHand[1].FaceValue
            };
            var result = NN.Compute(new BasicMLData(input));
            double max = -1;
            int index = 0;
            for (int i = 0; i < Global.NNOutputLayer; i++)
                if (result[i] > max)
                {
                    max = result[i];
                    index = i;
                }
            return (BlackjackMove)index;
        }
    }
}