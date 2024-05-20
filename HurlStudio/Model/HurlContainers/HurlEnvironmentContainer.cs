using HurlStudio.Collections.Model.Environment;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlEnvironmentContainer : HurlContainerBase
    {
        private HurlEnvironment _enviroment;
        private string _fileLocation;

        public HurlEnvironmentContainer(HurlEnvironment environment, string fileLocation) : base()
        {
            _enviroment = environment;
            _fileLocation = fileLocation;
        }

        public HurlEnvironment Environment
        {
            get => _enviroment;
            set
            {
                _enviroment = value;
                this.Notify();
            }
        }

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation= value;
                this.Notify();
            }
        }

        public override string GetId()
        {
            if (_enviroment == null) throw new ArgumentNullException(nameof(this.Environment));

            return _enviroment.FileLocation.ToSha256Hash();
        }
    }
}
