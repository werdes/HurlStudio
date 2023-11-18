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
    public class CollectionContainer : CollectionHierarchyBase
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
    }
}
