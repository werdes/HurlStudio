using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI
{
    public class ServiceManager<T>
    {
        private readonly Dictionary<Type, T> _instances = new Dictionary<Type, T>();
        private readonly Dictionary<Type, Func<T>> _providers = new Dictionary<Type, Func<T>>();

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
                if (_instances.ContainsKey(serviceInstance.GetType()) || _providers.ContainsKey(serviceInstance.GetType()))
                {
                    throw new InvalidOperationException($"Service with type {serviceInstance.GetType().Name} is already registered");
                }

                _instances[serviceInstance.GetType()] = serviceInstance;
            }

            return this;
        }

        /// Registers a service provider
        /// </summary>
        /// <typeparam name="TService">Type of the service to be registered</typeparam>
        /// <param name="provider">Provider of said service</param>
        /// <returns>the service manager</returns>
        /// <exception cref="ArgumentNullException">if the service instance is null</exception>
        /// <exception cref="InvalidOperationException">if a service of the given type has already been registered</exception>
        public ServiceManager<T> RegisterProvider<TService>(Func<T> provider)
        {
            lock (this._instances)
            {
                if (this._instances.ContainsKey(typeof(TService)) || this._providers.ContainsKey(typeof(TService)))
                {
                    throw new InvalidOperationException($"Service with type {typeof(TService).Name} is already registered");
                }

                this._providers[typeof(TService)] = provider;
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

            if (this._instances.ContainsKey(serviceType))
            {
                T instance = this._instances[serviceType];
                return instance;
            }

            if (this._providers.ContainsKey(serviceType))
            {
                Func<T> provider = this._providers[serviceType];
                return provider();
            }

            throw new ArgumentException($"No service of type {serviceType} was registered");
        }

        /// <summary>
        /// Syntactic sugar for the Get method
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Instance of the base type of this service manager instance</returns>
        public TService Get<TService>() where TService : T => (TService)Get(typeof(TService));

        /// <summary>
        /// Returns a list of all registered types
        /// </summary>
        /// <returns>a list of all registered types</returns>
        public Type[] GetAllKeys()
        {
            List<Type> types = new List<Type>();
            types.AddRange(this._providers.Keys);
            types.AddRange(this._instances.Keys);
            return types.ToArray();
        }

        /// <summary>
        /// Returns a list of instances of all registered services
        /// -> for registered instances the given instance
        /// -> for registered providers a new instance produced by the provider
        /// </summary>
        /// <returns></returns>
        public T[] GetInstancesOfAllRegisteredServices()
        {
            List<T> instances = new List<T>();
            instances.AddRange(this._instances.Values);
            instances.AddRange(this._providers.Select(x => x.Value()));
            return instances.ToArray();
        }
    }
}
