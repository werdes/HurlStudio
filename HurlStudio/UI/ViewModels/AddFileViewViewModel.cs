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
    public class AddFileViewViewModel : ViewModelBase
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IHurlFileTemplateService _templateService;
        private MainWindowViewModel? _mainWindowViewModel;
        private string? _query;
        private ObservableCollection<HurlFileTemplateContainer> _templates;
        private HurlFileTemplateContainer? _selectedTemplate;
        private string _fileName;

        public AddFileViewViewModel(ILogger<EditorViewViewModel> logger, IConfiguration configuration, IUserSettingsService userSettingsService, MainWindowViewModel mainWindowViewModel, IHurlFileTemplateService hurlFileTemplateService) : base(typeof(AddFileView))
        {
            _log = logger;
            _userSettingsService = userSettingsService;
            _mainWindowViewModel = mainWindowViewModel;
            _configuration = configuration;
            _templateService = hurlFileTemplateService;

            _templates = new ObservableCollection<HurlFileTemplateContainer>();
            _fileName = GlobalConstants.HURL_FILE_EXTENSION;
        }

        /// <summary>
        /// Binds the events on the templates
        /// </summary>
        public void BindEvents()
        {
            foreach (HurlFileTemplateContainer item in _templates)
            {
                item.HurlFileTemplateContainerSelected += this.On_TemplateContainer_HurlFileTemplateContainerSelected;
            }
        }


        /// <summary>
        /// Select given template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_TemplateContainer_HurlFileTemplateContainerSelected(object? sender, Model.EventArgs.HurlFileTemplateContainerSelectedEventArgs e)
        {
            if (e.SelectedHurlFileTemplateContainer == null) return;
            if (!_templates.Contains(e.SelectedHurlFileTemplateContainer)) return;

            this.SelectedTemplate = e.SelectedHurlFileTemplateContainer;
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

        public string? Query
        {
            get => _query;
            set
            {
                _query = value;
                this.Notify();
            }
        }

        public ObservableCollection<HurlFileTemplateContainer> Templates
        {
            get => _templates;
            set
            {
                _templates = value;
                this.Notify();
            }
        }

        public HurlFileTemplateContainer? SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                this.Notify();
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                this.Notify();
            }
        }
    }
}
