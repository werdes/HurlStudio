using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;

namespace HurlStudio.UI.ViewModels
{
    public class EditTemplateViewViewModel : ViewModelBase
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IHurlFileTemplateService _templateService;
        private MainWindowViewModel? _mainWindowViewModel;
        private string? _query;
        private HurlFileTemplateContainer? _templateContainer;

        public EditTemplateViewViewModel(ILogger<EditorViewViewModel> logger, IConfiguration configuration, IUserSettingsService userSettingsService, MainWindowViewModel mainWindowViewModel, IHurlFileTemplateService hurlFileTemplateService) : base(typeof(EditTemplateView))
        {
            _log = logger;
            _userSettingsService = userSettingsService;
            _mainWindowViewModel = mainWindowViewModel;
            _configuration = configuration;
            _templateService = hurlFileTemplateService;

        }

        public MainWindowViewModel? MainWindow
        {
            get => _mainWindowViewModel;
            set
            {
                _mainWindowViewModel = value;
                this.Notify();
            }
        }

        public HurlFileTemplateContainer? TemplateContainer
        {
            get => _templateContainer;
            set
            {
                _templateContainer = value;
                this.Notify();
            }
        }
    }
}
