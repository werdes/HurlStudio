using HurlStudio.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Serializer
{
    public class HurlEnvironmentSectionContainer
    {
        public EnvironmentSectionType Type { get; set; }
        public List<string> Lines { get; set; }

        public HurlEnvironmentSectionContainer()
        {
            this.Type = EnvironmentSectionType.None;
            this.Lines = new List<string>();
        }

        public HurlEnvironmentSectionContainer(EnvironmentSectionType sectionType)
        {
            this.Type = sectionType;
            this.Lines = new List<string>();
        }
    }
}
