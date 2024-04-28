using Dock.Model.Core;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface IEditorService
    {
        Task<bool> MoveFileToFolder(CollectionFile collectionFile, CollectionFolder folder);
        Task<bool> MoveFileToCollection(CollectionFile collectionFile, CollectionFolder parentFolder, CollectionContainer collection);
        Task<bool> MoveFileToCollectionRoot(CollectionFile collectionFile, CollectionContainer collection);
        Task<bool> MoveFolderToCollection(CollectionFolder folder, CollectionFolder parentFolder, CollectionContainer collection);
        Task<bool> MoveFolderToFolder(CollectionFolder folder, CollectionFolder parentFolder);
        Task<bool> MoveFolderToCollectionRoot(CollectionFolder folder, CollectionContainer collection);
        Task<ObservableCollection<IDockable>> GetOpenDocuments();
        Task OpenFile(string fileLocation, string collectionLocation);
        Task OpenFile(string fileLocation);
        Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument);
        Task OpenFolderSettings(CollectionFolder folder);
        Task OpenCollectionSettings(CollectionContainer collection);
    }
}
