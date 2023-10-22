﻿using HurlUI.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Collections.Model.Serializer
{
    public class HurlCollectionSectionContainer
    {
        public CollectionSectionType Type { get; set; }
        public List<string> Lines { get; set; }

        public HurlCollectionSectionContainer()
        {
            Type = CollectionSectionType.None;
            Lines = new List<string>();
        }

        public HurlCollectionSectionContainer(CollectionSectionType sectionType)
        {
            Type = sectionType;
            Lines = new List<string>();
        }
    }
}
