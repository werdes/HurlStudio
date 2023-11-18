using HurlStudio.Collections.Model.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public class CollectionFolder : CollectionHierarchyBase
    {
        private CollectionContainer _collectionContainer;
        private HurlFolder _folder;
        private string _location;


        public CollectionFolder(CollectionContainer collectionContainer, string location) : base()
        {
            _collectionContainer = collectionContainer;
            _location = location;
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                Notify();
            }
        }

        public HurlFolder Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                Notify();
            }
        }

        public CollectionContainer CollectionContainer
        {
            get => _collectionContainer;
            set
            {
                _collectionContainer = value;
                Notify();
            }
        }

    }
}
