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
        Task<HurlEnvironmentContainer> SetEnvironmentContainerAsync(HurlEnvironmentContainer container, HurlEnvironment environment);
        Task StoreEnvironmentAsync(HurlEnvironment environment, string environmentLocation);
        Task<bool> CreateEnvironment(HurlEnvironment environment);

        /// <summary>
        /// Removes an environment from environment directory
        /// </summary>
        /// <param name="environmentContainer">The environment to be removed</param>
        /// <param name="deletePermanently">Remove the environment permanently instead of moving it to system trash</param>
        /// <returns></returns>
        Task<bool> DeleteEnvironment(HurlEnvironmentContainer environmentContainer, bool deletePermanently);
    }
}
