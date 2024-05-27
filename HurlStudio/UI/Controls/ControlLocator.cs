using Avalonia.Controls.Templates;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Dock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HurlStudio.Utility;
using Avalonia.Controls.Presenters;
using HurlStudio.Common.UI;
using Avalonia.VisualTree;

namespace HurlStudio.UI.Controls
{
    public class ControlLocator : IDataTemplate
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;

        public ControlLocator(ILogger<ControlLocator> logger, IConfiguration configuration, ServiceManager<ViewModelBasedControl> controlBuilder)
        {
            _log = logger;
            _configuration = configuration;
            _controlBuilder = controlBuilder;
        }

        /// <summary>
        /// Retrieves the registered control from the control builder according to the supplied view model
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Control Build(object? data)
        {
            if (data != null)
            {
                try
                {
                    ViewModelBasedControl? control = _controlBuilder.GetAssociated(data.GetType());

                    if (control != null)
                    {
                        control.SetViewModel(data);

                        _log.LogTrace($"Built {control.GetType().Name} control: {control.GetHashCode()}");
                        return (Control)control;
                    }
                }
                catch { }

                return new TextBlock() { Text = $"{data?.GetType().Name} has not been registered in the control builder" };
            }
            return new TextBlock() { Text = $"No object was supplied to the locator" };
        }

        /// <summary>
        /// Checks against the registered types
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Match(object? data)
        {
            if (data == null) return false;

            return _controlBuilder.CheckAssociation(data.GetType());
        }
    }
}
