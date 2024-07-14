using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HurlStudio.Model.HurlFileTemplates;

namespace HurlStudio.Services.HurlFileTemplates
{
    public interface IHurlFileTemplateService
    {
        /// <summary>
        /// Returns a template by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<HurlFileTemplate?> GetHurlFileTemplateByIdAsync(Guid id);

        /// <summary>
        /// Returns all available templates
        /// </summary>
        /// <param name="reload"></param>
        /// <returns></returns>
        public Task<List<HurlFileTemplate>> GetTemplatesAsync(bool reload);

        /// <summary>
        /// Deletes a template from storage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DeleteTemplateAsync(Guid id);

        /// <summary>
        /// Adds a template to storage
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public Task<bool> CreateTemplateAsync(HurlFileTemplate template);

        /// <summary>
        /// Loads templates from storage
        /// </summary>
        /// <returns></returns>
        public Task<bool> LoadTemplatesAsync();

        /// <summary>
        /// Writes all templates to storage
        /// </summary>
        /// <returns></returns>
        public Task<bool> StoreTemplatesAsync();

        /// <summary>
        /// Updates a template
        /// </summary>
        /// <returns></returns>
        public Task<bool> UpdateTemplateAsync(HurlFileTemplate template);

        /// <summary>
        /// Returns a list of all available template containers
        /// </summary>
        /// <returns></returns>
        public Task<List<HurlFileTemplateContainer>?> GetTemplateContainersAsync();

        /// <summary>
        /// Returns a template container by its id
        /// </summary>
        /// <returns></returns>
        public Task<HurlFileTemplateContainer?> GetTemplateContainerAsync(Guid id);
    }
}