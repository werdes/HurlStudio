using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.Enums;
using HurlStudio.Services.UiState;
using HurlStudio.UI.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.ViewModels.Tools;
using HurlStudio.UI.Views;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
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
        private IUiStateService _uiStateService;

        private IRootDock? _rootDock;
        private EditorViewViewModel _editorViewViewModel;

        public LayoutFactory(ILogger<LayoutFactory> logger, IConfiguration configuration, ServiceManager<Tool> toolLayoutBuilder, ServiceManager<Document> documentLayoutBuilder, ServiceManager<ViewModelBasedControl> controlBuilder, EditorViewViewModel editorViewViewModel, IUiStateService uiStateService)
        {
            _configuration = configuration;
            _log = logger;
            _toolLayoutBuilder = toolLayoutBuilder;
            _documentLayoutBuilder = documentLayoutBuilder;
            _controlBuilder = controlBuilder;
            _editorViewViewModel = editorViewViewModel;
            _uiStateService = uiStateService;


            this.DockableLocator = new Dictionary<string, Func<IDockable?>>();
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

            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);
            double collectionExplorerProportion = uiState?.CollectionExplorerProportion ?? 0.2D;

            ProportionalDock leftDock = new ProportionalDock()
            {
                Proportion = collectionExplorerProportion,
                Orientation = Orientation.Vertical,
                ActiveDockable = collectionExplorer,
                FocusedDockable = null,
                CanClose = false,
                CanFloat = false,
                IsCollapsable = false,
                CanPin = false,
                VisibleDockables = this.CreateList<IDockable>(
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

            var windowLayout = this.CreateRootDock();
            windowLayout.Title = MAIN_DOCK_ID;
            var windowLayoutContent = new ProportionalDock
            {
                Orientation = Orientation.Horizontal,
                IsCollapsable = false,
                VisibleDockables = this.CreateList<IDockable>(
                    leftDock,
                    new ProportionalDockSplitter(),
                    documentDock
                )
            };

            windowLayout.IsCollapsable = false;
            windowLayout.VisibleDockables = this.CreateList<IDockable>(windowLayoutContent);
            windowLayout.ActiveDockable = windowLayoutContent;

            var rootDock = this.CreateRootDock();
            rootDock.IsCollapsable = false;
            rootDock.VisibleDockables = this.CreateList<IDockable>(windowLayout);
            rootDock.ActiveDockable = windowLayout;
            rootDock.DefaultDockable = windowLayout;


            _rootDock = rootDock;
            _editorViewViewModel.DocumentDock = documentDock;

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            base.InitLayout(layout);
        }

        public void AddDocument(IDockable document)
        {
            if (_editorViewViewModel != null && _editorViewViewModel.DocumentDock != null)
            {
                this.AddDockable(_editorViewViewModel.DocumentDock, document);
                //this.DockableLocator?.Add(document.Id, () => document);
            }
        }

        public void AddWelcomeDocument()
        {
            if (_editorViewViewModel != null && _editorViewViewModel.DocumentDock != null)
            {
                IDockable welcomeDocument = _documentLayoutBuilder.Get<WelcomeDocumentViewModel>();
                this.AddDockable(_editorViewViewModel.DocumentDock, welcomeDocument);
                this.SetActiveDockable(welcomeDocument);
                this.SetFocusedDockable(_rootDock, welcomeDocument);
            }
        }

        /// <summary>
        /// Closes a dockable asynchronously
        /// </summary>
        /// <param name="dockable"></param>
        /// <returns></returns>
        public async Task<bool> CloseDockableAsync(IDockable dockable)
        {
            if (dockable is IExtendedAsyncDockable dockableAsync)
            {
                DockableCloseMode closeMode = await dockableAsync.AskAllowClose();
                switch (closeMode)
                {
                    case DockableCloseMode.Cancel:
                        return false;
                    case DockableCloseMode.Close:
                    case DockableCloseMode.Discard:
                        await dockableAsync.Discard();
                        base.CloseDockable(dockable);
                        break;
                    case DockableCloseMode.Save:
                        await dockableAsync.Save();
                        base.CloseDockable(dockable);
                        break;
                    case DockableCloseMode.Undefined:
                        base.CloseDockable(dockable);
                        break;
                }
            }
            else
            {
                base.CloseDockable(dockable);
            }

            return true;
        }

        /// <summary>
        /// Overwritten dockable closer that asks for a method for closing the dockable
        /// </summary>
        /// <param name="dockable"></param>
        public override void CloseDockable(IDockable dockable)
        {
            Task task = new Task(async () =>
            {
                await this.CloseDockableAsync(dockable);
            });

            task.RunSynchronously();
        }
    }
}
