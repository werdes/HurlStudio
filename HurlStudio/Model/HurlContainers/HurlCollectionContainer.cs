using HurlStudio.Collections.Model;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlCollectionContainer : HurlContainerHierarchyBase
    {
        private HurlCollection _collection;

        public HurlCollectionContainer(HurlCollection collection) : base()
        {
            _collection = collection;
            _collection.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
        }

        public HurlCollection Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                this.Notify();

                if (_collection != null)
                {
                    _collection.ComponentPropertyChanged -= this.On_HurlComponent_ComponentPropertyChanged;
                    _collection.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Returns a unique Identifier for this collection 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the collections location is null</exception>
        public override string GetId()
        {
            if(_collection.CollectionFileLocation == null) throw new ArgumentNullException(nameof(_collection.CollectionFileLocation));
            return _collection.CollectionFileLocation.ToSha256Hash();
        }

        /// <summary>
        /// Returns the components' path
        /// </summary>
        /// <returns></returns>
        public override string GetPath()
        {
            return _collection.CollectionFileLocation;
        }

        public override string ToString()
        {
            return $"{nameof(HurlCollectionContainer)}: {this.GetId()} | {this.Collection}";
        }
    }
}
