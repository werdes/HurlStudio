using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.ViewModels.Tools;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Dock
{
    public class LayoutFactory : Factory
    {
        private const string COLLECTION_EXPLORER_TOOL_ID = "COLLECTIONEXPLORER";
        private const string FILE_SETTINGS_TOOL_ID = "FILESETTINGS";
        private const string DOCUMENT_DOCK_ID = "DOCUMENTS";
        private const string MAIN_DOCK_ID = "MAINDOCK";

        private ILogger _log;
        private IConfiguration _configuration;
        private ServiceManager<Tool> _toolLayoutBuilder;
        private ServiceManager<Document> _documentLayoutBuilder;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;

        private IRootDock _rootDock;
        private CollectionExplorerToolViewModel _collectionExplorer;
        private IDocumentDock _documentDock;
        private EditorViewViewModel _editorViewViewModel;

        public LayoutFactory(ILogger<LayoutFactory> logger, IConfiguration configuration, ServiceManager<Tool> toolLayoutBuilder, ServiceManager<Document> documentLayoutBuilder, ServiceManager<ViewModelBasedControl> controlBuilder, EditorViewViewModel editorViewViewModel)
        {
            _configuration = configuration;
            _log = logger;
            _toolLayoutBuilder = toolLayoutBuilder;
            _documentLayoutBuilder = documentLayoutBuilder;
            _controlBuilder = controlBuilder;
            _editorViewViewModel = editorViewViewModel;
        }

        /// <summary>
        /// Builds the dock layout
        /// </summary>
        /// <returns></returns>
        public override IRootDock CreateLayout()
        {
            CollectionExplorerToolViewModel collectionExplorer = _toolLayoutBuilder.Get<CollectionExplorerToolViewModel>();
            collectionExplorer.Id = COLLECTION_EXPLORER_TOOL_ID;
            collectionExplorer.IsEnabled = true;

            FileSettingsToolViewModel fileSettings = _toolLayoutBuilder.Get<FileSettingsToolViewModel>();
            fileSettings.Id = FILE_SETTINGS_TOOL_ID;

            ProportionalDock leftDock = new ProportionalDock()
            {
                Proportion = 0.2,
                Orientation = Orientation.Vertical,
                ActiveDockable = collectionExplorer,
                FocusedDockable = null,
                CanClose = false,
                CanFloat = false,
                IsCollapsable = false,
                CanPin = false,
                VisibleDockables = CreateList<IDockable>(
                    new ToolDock()
                    {
                        IsCollapsable = false,
                        ActiveDockable = collectionExplorer,
                        GripMode = GripMode.Hidden
                    }
                )
            };

            DocumentDock documentDock = new DocumentDock()
            {
                ActiveDockable = _editorViewViewModel.ActiveDocument,
                IsCollapsable = false,
                Id = DOCUMENT_DOCK_ID,
                VisibleDockables = _editorViewViewModel.Documents,
                CanCreateDocument = false
            };


            ProportionalDock rightDock = new ProportionalDock()
            {
                Proportion = 0.20,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>(
                    new ToolDock()
                    {
                        ActiveDockable = fileSettings,
                        GripMode = GripMode.Hidden
                    }
                )
            };

            var windowLayout = CreateRootDock();
            windowLayout.Title = MAIN_DOCK_ID;
            var windowLayoutContent = new ProportionalDock
            {
                Orientation = Orientation.Horizontal,
                IsCollapsable = false,
                VisibleDockables = CreateList<IDockable>(
                    leftDock,
                    new ProportionalDockSplitter(),
                    documentDock,
                    new ProportionalDockSplitter(),
                    rightDock
                )
            };

            windowLayout.IsCollapsable = false;
            windowLayout.VisibleDockables = CreateList<IDockable>(windowLayoutContent);
            windowLayout.ActiveDockable = windowLayoutContent;

            var rootDock = CreateRootDock();
            rootDock.IsCollapsable = false;
            rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
            rootDock.ActiveDockable = windowLayout;
            rootDock.DefaultDockable = windowLayout;


            _rootDock = rootDock;
            _collectionExplorer = collectionExplorer;
            _documentDock = documentDock;

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            base.InitLayout(layout);
        }

        public void AddDocument(IDockable document)
        {
            this.AddDockable(_documentDock, document);
        }

        public void AddWelcomeDocument()
        {
            IDockable welcomeDocument = _documentLayoutBuilder.Get<WelcomeDocumentViewModel>();
            this.AddDockable(_documentDock, welcomeDocument);
            this.SetActiveDockable(welcomeDocument);
            this.SetFocusedDockable(_rootDock, welcomeDocument);
        }
    }
}
