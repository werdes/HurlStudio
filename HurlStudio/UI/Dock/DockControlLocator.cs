using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Dock
{

    public class DockControlLocator : IDataTemplate
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;

        public DockControlLocator(ILogger<DockControlLocator> logger, IConfiguration configuration, ServiceManager<ViewModelBasedControl> controlBuilder)
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
            if (data is Tool || data is Document)
            {
                ViewModelBasedControl[] controls = _controlBuilder.GetInstancesOfAllRegisteredServices();
                ViewModelBasedControl? selectedControl = controls.Where(x => x.AttachedViewModelType == data.GetType()).FirstOrDefault();

                if (selectedControl != null)
                {
                    if (selectedControl.DataContext == null)
                    {
                        selectedControl.DataContext = data;
                    }
                    return (Control)selectedControl;
                }

                return new TextBlock() { Text = $"{data?.GetType().Name} has not been registered in the control builder" };
            }
            return new TextBlock() { Text = $"{data?.GetType().Name} is not a ControlBase" };
        }

        public bool Match(object? data)
        {
            return data is ObservableObject || data is IDockable;
        }
    }
}