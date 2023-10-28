using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI
{
    public class ServiceManager<T>
    {
        private readonly Dictionary<Type, T> _instances = new Dictionary<Type, T>();

        public ServiceManager() { }

        /// Registers a service
        /// </summary>
        /// <typeparam name="TService">Type of the service to be registered</typeparam>
        /// <param name="serviceInstance">Instance of said service</param>
        /// <returns>the service manager</returns>
        /// <exception cref="ArgumentNullException">if the service instance is null</exception>
        /// <exception cref="InvalidOperationException">if a service of the given type has already been registered</exception>
        public ServiceManager<T> Register<TService>(TService serviceInstance) where TService : T
        {
            if (serviceInstance == null)
                throw new ArgumentNullException(nameof(serviceInstance));

            lock (_instances)
            {
                if (_instances.ContainsKey(serviceInstance.GetType()))
                {
                    throw new InvalidOperationException($"Service with type {serviceInstance.GetType().Name} is already registered");
                }

                _instances[serviceInstance.GetType()] = serviceInstance;
            }

            return this;
        }

        /// <summary>
        /// Returns a base type instance (for example a view model assigned to a view type)
        /// </summary>
        /// <param name="serviceType">runtime type</param>
        /// <returns>Instance of the base type of this service manager instance</returns>
        public T Get(Type serviceType)
        {
            if (!serviceType.IsAssignableTo(typeof(T)))
                throw new InvalidOperationException($"Service type {serviceType} is not assignable to {typeof(T)}");

            if (_instances.ContainsKey(serviceType))
            {
                T instance = _instances[serviceType];
                return instance;
            }

            throw new ArgumentException($"No service of type {serviceType} was registered");
        }
    }
}
