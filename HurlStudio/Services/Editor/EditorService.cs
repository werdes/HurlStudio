using HurlStudio.Model.CollectionContainer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public class EditorService : IEditorService
    {
        private ILogger _log;

        public EditorService(ILogger<EditorService> logger)
        {
            _log = logger;
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
    }
}
