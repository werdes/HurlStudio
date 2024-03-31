using HurlStudio.Collections.Model.Environment;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public class CollectionEnvironment : CollectionComponentBase
    {
        private HurlEnvironment _enviroment;

        public CollectionEnvironment(HurlEnvironment environment) : base()
        {
            _enviroment = environment;
        }

        public HurlEnvironment Environment
        {
            get => _enviroment;
            set
            {
                _enviroment = value;
                Notify();
            }
        }

        public override string GetId()
        {
            if (_enviroment == null) throw new ArgumentNullException(nameof(Environment));

            return _enviroment.Name.ToSha256Hash();
        }
    }
}
