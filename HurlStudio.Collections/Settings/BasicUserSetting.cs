using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class BasicUserSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "user";
        private const char VALUE_SEPARATOR = ':';

        private string? _user;
        private string? _password;

        public BasicUserSetting()
        {
        }

        [HurlSettingDisplayString]
        public string? User
        {
            get => _user;
            set
            {
                _user = value;
                this.Notify();
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();
            
            if (!string.IsNullOrEmpty(this.User))
            {
                arguments.Add(new UserArgument(this.User, this.Password ?? string.Empty));
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            string[] parts = value.Split(VALUE_SEPARATOR);
            if (parts.Length == 0) return null;

            this.User = parts.Get(0);

            string? passwordBase64 = parts.Get(1);
            if (!string.IsNullOrEmpty(passwordBase64))
            {
                this.Password = passwordBase64.DecodeBase64();
            }

            return this;
        }

        /// <summary>
        /// Returns null, since this setting isn't key/value based
        /// </summary>
        /// <returns></returns>
        public override string? GetConfigurationKey()
        {
            return null;
        }


        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return this.User ?? string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior -> Overwrite -> Only one user can be passed
        /// into a new instance of the returned argument at execution time
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Overwrite;
        }
        
        /// <summary>
        /// Returns the configuration name (file_root)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value 
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return $"{this.User}{VALUE_SEPARATOR}{this.Password?.EncodeBase64()}";
        }
    }
}