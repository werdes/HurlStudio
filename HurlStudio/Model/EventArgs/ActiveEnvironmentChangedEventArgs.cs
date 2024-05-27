using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class ActiveEnvironmentChangedEventArgs : System.EventArgs
    {
        private readonly HurlEnvironmentContainer _environment;

        public ActiveEnvironmentChangedEventArgs(HurlEnvironmentContainer environment)
        {
            _environment = environment;
        }

        public HurlEnvironmentContainer Environment { get => _environment; }
    }
}
