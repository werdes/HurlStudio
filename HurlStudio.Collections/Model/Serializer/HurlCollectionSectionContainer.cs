using HurlStudio.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Serializer
{
    public class HurlCollectionSectionContainer
    {
        public CollectionSectionType Type { get; set; }
        public List<string> Lines { get; set; }

        public HurlCollectionSectionContainer()
        {
            this.Type = CollectionSectionType.None;
            this.Lines = new List<string>();
        }

        public HurlCollectionSectionContainer(CollectionSectionType sectionType)
        {
            this.Type = sectionType;
            this.Lines = new List<string>();
        }
    }
}
