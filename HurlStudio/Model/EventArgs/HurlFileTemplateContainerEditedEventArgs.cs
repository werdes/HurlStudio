using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class HurlFileTemplateContainerEditedEventArgs : System.EventArgs
    {
        private HurlFileTemplateContainer _hurlFileTemplateContainer;

        public HurlFileTemplateContainerEditedEventArgs(HurlFileTemplateContainer hurlFileTemplateContainer)
        {
            _hurlFileTemplateContainer = hurlFileTemplateContainer;
        }

        public HurlFileTemplateContainer SelectedHurlFileTemplateContainer
        {
            get => _hurlFileTemplateContainer;
        }
    }
}
