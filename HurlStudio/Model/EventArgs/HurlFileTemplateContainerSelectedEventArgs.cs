using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class HurlFileTemplateContainerSelectedEventArgs : System.EventArgs
    {
        private HurlFileTemplateContainer _hurlFileTemplateContainer;

        public HurlFileTemplateContainerSelectedEventArgs(HurlFileTemplateContainer hurlFileTemplateContainer)
        {
            _hurlFileTemplateContainer = hurlFileTemplateContainer;
        }

        public HurlFileTemplateContainer SelectedHurlFileTemplateContainer
        {
            get => _hurlFileTemplateContainer;
        }
    }
}
