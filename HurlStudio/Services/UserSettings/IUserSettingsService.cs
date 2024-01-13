using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.UserSettings
{
    public interface IUserSettingsService
    {
        Task<Model.UserSettings.UserSettings> GetUserSettingsAsync(bool refresh);
        Model.UserSettings.UserSettings GetUserSettings(bool refresh);
        Task StoreUserSettingsAsync();
        void StoreUserSettings();
    }
}
