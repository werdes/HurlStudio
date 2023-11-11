using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public interface IHurlArgument
    {
        string[] GetCommandLineArguments();
    }
}
