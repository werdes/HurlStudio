using HurlStudio.Common.UI;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public interface IEditorDocument
    {
        event EventHandler<SettingEvaluationChangedEventArgs>? SettingAdded;
        event EventHandler<SettingEvaluationChangedEventArgs>? SettingRemoved;
        
        HurlContainerBase? HurlContainer { get; }
        HurlCollectionContainer? UnderlyingCollection { get; }

        bool HasChanges { get; set; }
        OrderedObservableCollection<HurlSettingSection> SettingSections { get; set; }
        string GetId();
        void AddSetting(HurlSettingContainer settingContainer, int idx = 0);
        void RemoveSetting(HurlSettingContainer settingContainer);

    }
}
