using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface IEnvironmentService
    {
        Task<HurlEnvironment> GetEnvironmentAsync(string enviromentLocation);
        Task<IEnumerable<HurlEnvironment>> GetEnvironmentsAsync();
        Task<HurlEnvironmentContainer> GetEnvironmentContainerAsync(HurlEnvironment enviroment);
        Task<ObservableCollection<HurlEnvironmentContainer>> GetEnvironmentContainersAsync();
    }
}
