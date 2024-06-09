using HurlStudio.Collections.Model;
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
        private bool _isActiveEnvironment;

        public HurlEnvironmentContainer(HurlEnvironment environment, string fileLocation) : base()
        {
            _fileLocation = fileLocation;
            _enviroment = environment;
            _isActiveEnvironment = false;
            _enviroment.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
        }

        public HurlEnvironment Environment
        {
            get => _enviroment;
            set
            {
                _enviroment = value;
                this.Notify();

                if (_enviroment != null)
                {
                    _enviroment.ComponentPropertyChanged -= this.On_HurlComponent_ComponentPropertyChanged;
                    _enviroment.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
                }
            }
        }

        public bool IsActiveEnvironment
        {
            get => _isActiveEnvironment;
            set
            {
                _isActiveEnvironment = value;
                this.Notify();
            }
        }

        public string EnvironmentFileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation= value;
                this.Notify();
            }
        }

        /// <summary>
        /// Returns the components' path
        /// </summary>
        /// <returns></returns>
        public override string GetPath()
        {
            return _fileLocation;
        }

        public override string GetId()
        {
            if (_enviroment == null) throw new ArgumentNullException(nameof(this.Environment));

            return _enviroment.EnvironmentFileLocation.ToSha256Hash();
        }
    }
}
