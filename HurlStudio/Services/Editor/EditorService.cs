using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public class EditorService : IEditorService
    {
        private ILogger _log;
        private LayoutFactory _layoutFactory;
        private ServiceManager<Document> _documentControlBuilder;
        private EditorViewViewModel _editorViewViewModel;
        private IUserSettingsService _userSettingsService;
        private IConfiguration _configuration;

        public EditorService(ILogger<EditorService> logger, ServiceManager<Document> documentControlBuilder, EditorViewViewModel editorViewViewModel, LayoutFactory layoutFactory, IUserSettingsService userSettingsService, IConfiguration configuration)
        {
            _log = logger;
            _documentControlBuilder = documentControlBuilder;
            _editorViewViewModel = editorViewViewModel;
            _layoutFactory = layoutFactory;
            _userSettingsService = userSettingsService;
            _configuration = configuration;
        }

        public async Task<ObservableCollection<IDockable>> GetOpenDocuments()
        {
            ObservableCollection<IDockable> dockables = new ObservableCollection<IDockable>();
            dockables.Add(_documentControlBuilder.Get<WelcomeDocumentViewModel>());

            return dockables;
        }

        public Task<bool> MoveFileToCollection(CollectionFile collectionFile, CollectionFolder parentFolder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToCollectionRoot(CollectionFile collectionFile, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToFolder(CollectionFile collectionFile, CollectionFolder folder)
        {
            _log.LogInformation($"Moving [{collectionFile}] to folder [{folder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollection(CollectionFolder folder, CollectionFolder parentFolder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollectionRoot(CollectionFolder folder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to collection [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToFolder(CollectionFolder folder, CollectionFolder parentFolder)
        {
            _log.LogInformation($"Moving [{folder}] to folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task OpenCollectionSettings(CollectionContainer collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a collection file as a document 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if no layout has been created</exception>
        public async Task OpenFile(CollectionFile file)
        {
            if(_editorViewViewModel.Layout == null) throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            IDockable? openDocument = _editorViewViewModel.Documents.Where(x => x is FileDocumentViewModel)
                                                                    .Select(x => (FileDocumentViewModel)x)
                                                                    .Where(x => x.File != null && x.File.Equals(file))
                                                                    .FirstOrDefault();

            if (openDocument == null)
            {
                FileDocumentViewModel fileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
                fileDocument.File = file;
                
                _layoutFactory.AddDocument(fileDocument);
                _layoutFactory.SetActiveDockable(fileDocument);
                _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, fileDocument);

                _editorViewViewModel.FileHistoryEntries.Add(new Model.UiState.FileHistoryEntry(file.Location, DateTime.UtcNow));
            }
            else
            {
                _layoutFactory.SetActiveDockable(openDocument);
                _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
            }
        }

        public Task OpenFolderSettings(CollectionFolder folder)
        {
            throw new NotImplementedException();
        }
    }
}
