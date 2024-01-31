using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Enums
{
    public enum ProxyProtocol
    {
        Undefined,
        HTTP,
        HTTPS
    }

    public static class ProxyProtocolExtensions
    {
        public static ProxyProtocol[] PossibleValues
        {
            get => Enum.GetValues<ProxyProtocol>();
        }
        public static ProxyProtocol[] DisplayValues
        {
            get => Enum.GetValues<ProxyProtocol>().Where(x => x != ProxyProtocol.Undefined).ToArray();
        }
    }
}
