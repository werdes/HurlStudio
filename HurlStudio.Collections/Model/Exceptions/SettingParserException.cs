using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Exceptions
{
    public class SettingParserException : Exception
    {
        private string _value;
        private Type? _settingType;

        public SettingParserException(string value, Type? settingType, Exception innerException) : base($"Unable to parse setting [{value}]", innerException)
        {
            _value = value;
            _settingType = settingType;
        }

        public string Value
        {
            get => _value;
        }

        public Type? SettingType
        {
            get => _settingType;
        }
    }
}
