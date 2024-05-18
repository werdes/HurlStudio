using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class CookieSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "cookies";
        private const string VALUE_SEPARATOR = ";";

        private string? _cookieReadFile;
        private string? _cookieWriteFile;

        public CookieSetting()
        {

        }

        [HurlSettingDisplayString]
        public string? CookieReadFile
        {
            get => _cookieReadFile;
            set
            {
                _cookieReadFile = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? CookieWriteFile
        {
            get => _cookieWriteFile;
            set
            {
                _cookieWriteFile = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            string[] parts = value.Split(VALUE_SEPARATOR);

            this.CookieReadFile = parts.Get(0) ?? string.Empty;
            this.CookieWriteFile = parts.Get(1) ?? string.Empty;

            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> args = new List<IHurlArgument>();

            if (!string.IsNullOrWhiteSpace(_cookieReadFile))
            {
                args.Add(new CookieArgument(_cookieReadFile));
            }
            if (!string.IsNullOrWhiteSpace(_cookieWriteFile))
            {
                args.Add(new CookieJarArgument(_cookieWriteFile));
            }

            return args.ToArray();
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
        /// Returns the configuration name (cookies)
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
            return $"{_cookieReadFile}{VALUE_SEPARATOR}{_cookieWriteFile}";
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            string? text = Path.GetFileName(_cookieReadFile);
            if (text != null && !string.IsNullOrWhiteSpace(_cookieWriteFile))
            {
                text += ", ";
            }
            text += Path.GetFileName(_cookieWriteFile);

            return text;
        }

        /// <summary>
        /// Returns the inheritance behavior -> Overwrite -> Setting is unique to a file
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Overwrite;
        }

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.CookieReadFile = string.Empty;
            this.CookieWriteFile = string.Empty;

            return this;
        }
    }
}
