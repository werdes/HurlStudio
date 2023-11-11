using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Services.Editor
{
    public interface ICollectionService
    {
        Task<IEnumerable<HurlCollection>> GetCollectionsAsync();

    }
}
