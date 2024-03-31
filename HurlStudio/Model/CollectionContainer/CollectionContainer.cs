using HurlStudio.Collections.Model.Collection;
using HurlStudio.Common.Extensions;
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
    public class CollectionContainer : CollectionComponentHierarchyBase
    {
        private HurlCollection _collection;

        public CollectionContainer(HurlCollection collection) : base()
        {
            _collection = collection;
        }

        public HurlCollection Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                Notify();
            }
        }

        /// <summary>
        /// Returns a unique Identifier for this collection 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the collections location is null</exception>
        public override string GetId()
        {
            if(_collection.FileLocation == null) throw new ArgumentNullException(nameof(_collection.FileLocation));
            return _collection.FileLocation.ToSha256Hash();
        }

        public override string ToString()
        {
            return $"{nameof(CollectionContainer)}: {GetId()} | {Collection}";
        }
    }
}
