using System;

namespace Util
{
    public static class GuidProvider
    {
        public static string NewGuid(bool fromParameters = true)
        {
            var guid = Guid.NewGuid();
            string result = null;
            if (fromParameters)
                result = $"P={Global.Population},G={Global.Games},M={Global.Mutation}({guid})";
            else
                result = guid.ToString();
            return result;
        }
    }
}