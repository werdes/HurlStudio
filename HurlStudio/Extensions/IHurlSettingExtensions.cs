using HurlStudio.Collections.Settings;
using HurlStudio.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Extensions
{
    public static class IHurlSettingExtensions
    {
        /// <summary>
        /// Returns the localized title for the given setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string GetLocalizedTitle(this IHurlSetting setting)
        {
            string resourceName = $"Setting.{setting.GetType().Name}.Title";
            string? localizedText = Localization.ResourceManager.GetString(resourceName);

            if (localizedText != null)
            {
                return localizedText;
            }
            return string.Empty;
        }
    }
}
