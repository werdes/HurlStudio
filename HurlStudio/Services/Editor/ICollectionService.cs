using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface ICollectionService
    {
        Task<IEnumerable<HurlCollection>> GetCollectionsAsync();

    }
}
