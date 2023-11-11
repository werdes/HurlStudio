using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using HurlUI.UI.Controls;
using HurlUI.UI.ViewModels;
using HurlUI.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.Dock
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
        private ServiceManager<ControlBase> _controlBuilder;

        private IRootDock _rootDock;
        private CollectionExplorerToolViewModel _collectionExplorer;

        public LayoutFactory(ILogger<LayoutFactory> logger, IConfiguration configuration, ServiceManager<Tool> toolLayoutBuilder, ServiceManager<Document> documentLayoutBuilder, ServiceManager<ControlBase> controlBuilder)
        {
            this._configuration = configuration;
            this._log = logger;
            this._toolLayoutBuilder = toolLayoutBuilder;
            this._documentLayoutBuilder = documentLayoutBuilder;
            this._controlBuilder = controlBuilder;
        }

        /// <summary>
        /// Builds the dock layout
        /// </summary>
        /// <returns></returns>
        public override IRootDock CreateLayout()
        {
            CollectionExplorerToolViewModel collectionExplorer = this._toolLayoutBuilder.Get<CollectionExplorerToolViewModel>();
            collectionExplorer.Id = COLLECTION_EXPLORER_TOOL_ID;

            FileSettingsToolViewModel fileSettings = this._toolLayoutBuilder.Get<FileSettingsToolViewModel>();
            fileSettings.Id = FILE_SETTINGS_TOOL_ID;

            FileDocumentViewModel fileDocument = this._documentLayoutBuilder.Get<FileDocumentViewModel>();
            fileDocument.Id = Guid.NewGuid().ToString();
            fileDocument.Title = "default";

            ObservableCollection<IDockable> list = new ObservableCollection<IDockable>();
            list.Add(fileDocument);
            for(int i = 0; i < 15; i++)
            {
                FileDocumentViewModel fileDocument2 = this._documentLayoutBuilder.Get<FileDocumentViewModel>();
                fileDocument2.Id = Guid.NewGuid().ToString();
                fileDocument2.Title = fileDocument2.Id.Split('-').First();
                list.Add(fileDocument2);
            }

            //CollectionExplorerToolViewModel collectionExplorer2 = _toolLayoutBuilder.Get<CollectionExplorerToolViewModel>();
            //collectionExplorer2.Id = COLLECTION_EXPLORER_TOOL_ID + "2";
            //CollectionExplorerToolViewModel collectionExplorer3 = _toolLayoutBuilder.Get<CollectionExplorerToolViewModel>();
            //collectionExplorer3.Id = COLLECTION_EXPLORER_TOOL_ID + "3";
            //CollectionExplorerToolViewModel collectionExplorer4 = _toolLayoutBuilder.Get<CollectionExplorerToolViewModel>();
            //collectionExplorer4.Id = COLLECTION_EXPLORER_TOOL_ID + "4";

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
                        ActiveDockable = collectionExplorer,
                        GripMode = GripMode.Hidden
                    }
                )
            };

            DocumentDock centerDock = new DocumentDock()
            {
                ActiveDockable = null,
                IsCollapsable = false,
                Proportion = double.NaN,
                Id = DOCUMENT_DOCK_ID,
                Title="abc",
                VisibleDockables= list,
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
                    centerDock,
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


            this._rootDock = rootDock;
            this._collectionExplorer = collectionExplorer;
            return rootDock;

            //RootDock dockContainer = new RootDock();
            //dockContainer.Id = MAIN_DOCK_ID;
            //dockContainer.Title = "MainDock";
            //dockContainer.ActiveDockable = mainLayout;
            //dockContainer.VisibleDockables = CreateList<IDockable>(mainLayout);

            //IRootDock rootDock = CreateRootDock();
            //rootDock.IsCollapsable = false;
            //rootDock.DefaultDockable = dockContainer;
            //rootDock.VisibleDockables = CreateList<IDockable>(dockContainer);

            //this._rootDock = rootDock;

            //return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object?>>()
            {
                [COLLECTION_EXPLORER_TOOL_ID] = () => layout,
                [COLLECTION_EXPLORER_TOOL_ID + "2"] = () => layout,
                [COLLECTION_EXPLORER_TOOL_ID + "3"] = () => layout,
                [COLLECTION_EXPLORER_TOOL_ID + "4"] = () => layout
            };

            DockableLocator = new Dictionary<string, Func<IDockable?>>()
            {
                [MAIN_DOCK_ID] = () => this._rootDock,
                [COLLECTION_EXPLORER_TOOL_ID] = () => this._collectionExplorer
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }

    }
}
