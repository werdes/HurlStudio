using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingTypeContainerSelectedEventArgs : System.EventArgs
    {
        private HurlSettingTypeContainer _hurlSettingTypeContainer;

        public SettingTypeContainerSelectedEventArgs(HurlSettingTypeContainer hurlSettingTypeContainer)
        {
            _hurlSettingTypeContainer = hurlSettingTypeContainer;
        }

        public HurlSettingTypeContainer SelectedHurlSettingTypeContainer
        {
            get => _hurlSettingTypeContainer;
        }
    }
}
