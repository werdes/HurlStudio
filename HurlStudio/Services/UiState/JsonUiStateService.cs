using Avalonia.Styling;
using HurlStudio.Common;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.Enums;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Views;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HurlStudio.UI.ViewModels.Documents;
using Dock.Model.Core;

namespace HurlStudio.Services.UiState
{
    public class JsonUiStateService : IUiStateService
    {
        private JsonSerializerOptions _serializerOptions;
        private IConfiguration _configuration;
        private ILogger _logger;
        private Model.UiState.UiState? _uiState;

        public JsonUiStateService(IConfiguration configuration, ILogger<JsonUiStateService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _uiState = null;
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString | JsonNumberHandling.Strict,
                Converters = {
                    new JsonStringEnumConverter()
                }
            };
        }

        /// <summary>
        /// returns the ui state
        /// </summary>
        /// <param name="refresh">reload the state from disk</param>
        /// <returns>The deserialized UiState object</returns>
        public async Task<Model.UiState.UiState?> GetUiStateAsync(bool refresh)
        {
            if (refresh || _uiState == null)
            {
                await this.LoadUiStateAsync();
            }

            return _uiState;
        }

        /// <summary>
        /// returns the ui state
        /// </summary>
        /// <param name="refresh">reload the state from disk</param>
        /// <returns>The deserialized UiState object</returns>
        public Model.UiState.UiState? GetUiState(bool refresh)
        {
            if (refresh || _uiState == null)
            {
                this.LoadUiState();
            }

            return _uiState;
        }

        /// <summary>
        /// Stores the ui state to a .json-file on disk
        /// </summary>
        public async Task StoreUiStateAsync()
        {
            string path = this.GetUiStateFilePath();

            if (_uiState == null) throw new ArgumentNullException($"no ui state was provided to {nameof(JsonUiStateService)}");

            string json = JsonSerializer.Serialize(_uiState, _serializerOptions);
            await File.WriteAllTextAsync(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Stores the ui state to a .json-file on disk
        /// </summary>
        public void StoreUiState()
        {
            string path = this.GetUiStateFilePath();

            if (_uiState == null) throw new ArgumentNullException($"no ui state was provided to {nameof(JsonUiStateService)}");

            string json = JsonSerializer.Serialize(_uiState, _serializerOptions);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Loads the ui state from disk
        /// </summary>
        /// <returns></returns>
        private async Task LoadUiStateAsync()
        {
            string path = this.GetUiStateFilePath();
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path, Encoding.UTF8);
                _uiState = JsonSerializer.Deserialize<Model.UiState.UiState>(json, _serializerOptions);
            }
            else
            {
                _uiState = this.GetDefaultUiState();
                await this.StoreUiStateAsync();
            }
        }

        /// <summary>
        /// Loads the ui state from disk
        /// </summary>
        /// <returns></returns>
        private void LoadUiState()
        {
            string path = this.GetUiStateFilePath();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                _uiState = JsonSerializer.Deserialize<Model.UiState.UiState>(json, _serializerOptions);
            }
            else
            {
                _uiState = this.GetDefaultUiState();
                this.StoreUiState();
            }
        }

        /// <summary>
        /// Returns the path of the state file
        /// </summary>
        /// <returns>path of the state file</returns>
        private string GetUiStateFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                GlobalConstants.APPLICATION_DIRECTORY_NAME,
                                GlobalConstants.UISTATE_JSON_FILE_NAME);
        }

        /// <summary>
        /// Returns a default ui state in case no file exists
        /// </summary>
        /// <returns> default ui state object</returns>
        private Model.UiState.UiState GetDefaultUiState()
        {
            return new Model.UiState.UiState();
        }

        /// <summary>
        /// Sets a specific collapse state
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collapsed"></param>
        public void SetCollectionExplorerCollapseState(string id, bool collapsed)
        {
            if (_uiState == null) _uiState = this.GetDefaultUiState();

            if (!_uiState.ExpandedCollectionExplorerComponents.ContainsKey(id))
            {
                _uiState.ExpandedCollectionExplorerComponents.Add(id, false);
            }
            _uiState.ExpandedCollectionExplorerComponents[id] = collapsed;
        }


        /// <summary>
        /// Builds the ui state from models
        /// </summary>
        private void BuildUiStateCollections(EditorViewViewModel editorViewViewModel)
        {
            if (_uiState == null) _uiState = this.GetDefaultUiState();

            foreach (HurlCollectionContainer collectionContainer in editorViewViewModel.Collections)
            {
                string collectionId = collectionContainer.GetId();

                if (!_uiState.ExpandedCollectionExplorerComponents.ContainsKey(collectionId))
                {
                    _uiState.ExpandedCollectionExplorerComponents.Add(collectionId, false);
                }
                _uiState.ExpandedCollectionExplorerComponents[collectionId] = collectionContainer.Collapsed;

                foreach (HurlFolderContainer collectionFolder in collectionContainer.Folders)
                {
                    this.BuildUiStateFolder(collectionFolder);
                }
            }
        }

        /// <summary>
        /// Builds the ui state of the folders
        /// </summary>
        /// <param name="folder"></param>
        /// <exception cref="ArgumentNullException">if the ui state is null</exception>
        private void BuildUiStateFolder(HurlFolderContainer folder)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));

            string folderId = folder.GetId();
            if (!_uiState.ExpandedCollectionExplorerComponents.ContainsKey(folderId))
            {
                _uiState.ExpandedCollectionExplorerComponents.Add(folderId, false);
            }
            _uiState.ExpandedCollectionExplorerComponents[folderId] = folder.Collapsed;

            foreach (HurlFolderContainer subFolder in folder.Folders)
            {
                this.BuildUiStateFolder(subFolder);
            }
        }

        /// <summary>
        /// Sets the collections from _viewModel to the ui state object
        /// </summary>
        /// <param name="editorView">view model of the editor view</param>
        public void SetCollectionExplorerState(EditorViewViewModel editorView)
        {
            this.BuildUiStateCollections(editorView);
        }

        /// <summary>
        /// Adds the main window state to the ui state object
        /// </summary>
        /// <param name="mainWindow">the main window control</param>
        public void SetMainWindowState(MainWindow mainWindow)
        {
            if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));

            _uiState.MainWindowPosition = new System.Drawing.Rectangle(mainWindow.Position.X, mainWindow.Position.Y, (int)mainWindow.Width, (int)mainWindow.Height);
            _uiState.MainWindowIsMaximized = mainWindow.WindowState == Avalonia.Controls.WindowState.Maximized;
        }

        /// <summary>
        /// Adds the opened file history to the ui state
        /// </summary>
        /// <param name="editorView">view model of the editor view</param>
        public void SetFileHistory(EditorViewViewModel editorView)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));
            if (editorView == null) throw new ArgumentNullException(nameof(editorView));

            _uiState.FileHistoryEntries.Clear();
            _uiState.FileHistoryEntries.AddRange(editorView.FileHistoryEntries);
        }

        /// <summary>
        /// Sets the collapsed state of settings
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        public void SetSettingCollapsedState(string id, bool visible)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!_uiState.SettingCollapsedStates.ContainsKey(id))
            {
                _uiState.SettingCollapsedStates.Add(id, false);
            }
            _uiState.SettingCollapsedStates[id] = visible;
        }

        /// <summary>
        /// Sets the collapsed state of a setting section
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        public void SetSettingSectionCollapsedState(string id, bool visible)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!_uiState.SettingSectionCollapsedStates.ContainsKey(id))
            {
                _uiState.SettingSectionCollapsedStates.Add(id, false);
            }
            _uiState.SettingSectionCollapsedStates[id] = visible;
        }

        /// <summary>
        /// Sets the opened files
        /// </summary>
        /// <param name="openedFiles"></param>
        public void SetOpenedDocuments(List<string> openedFiles)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));

            _uiState.OpenedDocuments.Clear();
            _uiState.OpenedDocuments.AddRange(openedFiles);
        }

        /// <summary>
        /// Sets the collection explorers' proportional width
        /// </summary>
        /// <param name="proportion"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetCollectionExplorerProportion(double? proportion)
        {
            if (proportion == null) return;
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));

            _uiState.CollectionExplorerProportion = proportion.Value;
        }

        /// <summary>
        /// Sets the active environment file
        /// </summary>
        /// <param name="environmentFile"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetActiveEnvironment(string? environmentFile)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));

            _uiState.ActiveEnvironmentFile = environmentFile;
        }

        /// <summary>
        /// Sets the collapsed state of settings (non file specific ones, for enabling/disabling per file)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        public void SetSettingEnabledState(string id, bool visible)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!_uiState.SettingEnabledStates.ContainsKey(id))
            {
                _uiState.SettingEnabledStates.Add(id, false);
            }
            _uiState.SettingEnabledStates[id] = visible;
        }

        /// <summary>
        /// Sets the active document path
        /// </summary>
        /// <param name="documentPath"></param>
        public void SetActiveDocument(EditorViewViewModel editorViewViewModel)
        {
            if (_uiState == null) throw new ArgumentNullException(nameof(_uiState));
            if (editorViewViewModel == null) throw new ArgumentNullException(nameof(editorViewViewModel));

            IDockable? activeDocument = editorViewViewModel.ActiveDocument;

            if (activeDocument is IEditorDocument document &&
                document.HurlContainer != null)
            {
                _uiState.ActiveDocument = document.HurlContainer.GetPath();
            }
        }
    }
}
