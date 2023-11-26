using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface IEditorService
    {
        Task<bool> MoveFileToFolder(Model.CollectionContainer.CollectionFile collectionFile, Model.CollectionContainer.CollectionFolder folder);
        Task<bool> MoveFileToCollection(Model.CollectionContainer.CollectionFile collectionFile, Model.CollectionContainer.CollectionFolder parentFolder, Model.CollectionContainer.CollectionContainer collection);
        Task<bool> MoveFileToCollectionRoot(Model.CollectionContainer.CollectionFile collectionFile, Model.CollectionContainer.CollectionContainer collection);
        Task<bool> MoveFolderToCollection(Model.CollectionContainer.CollectionFolder folder, Model.CollectionContainer.CollectionFolder parentFolder, Model.CollectionContainer.CollectionContainer collection);
        Task<bool> MoveFolderToFolder(Model.CollectionContainer.CollectionFolder folder, Model.CollectionContainer.CollectionFolder parentFolder);
        Task<bool> MoveFolderToCollectionRoot(Model.CollectionContainer.CollectionFolder folder, Model.CollectionContainer.CollectionContainer collection);
    }
}
