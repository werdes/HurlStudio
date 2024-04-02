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
        private readonly Dictionary<Type, Type> _associations = new Dictionary<Type, Type>();

        public ServiceManager() { }

        /// <summary>
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

        /// <summary>
        /// Registers a service provider
        /// </summary>
        /// <typeparam name="TService">Type of the service to be registered</typeparam>
        /// <param name="provider">Provider of said service</param>
        /// <returns>the service manager</returns>
        /// <exception cref="ArgumentNullException">if the service instance is null</exception>
        /// <exception cref="InvalidOperationException">if a service of the given type has already been registered</exception>
        public ServiceManager<T> RegisterProvider<TService>(Func<T> provider)
        {
            lock (_instances)
            {
                if (_instances.ContainsKey(typeof(TService)) || _providers.ContainsKey(typeof(TService)))
                {
                    throw new InvalidOperationException($"Service with type {typeof(TService).Name} is already registered");
                }

                _providers[typeof(TService)] = provider;
            }

            return this;
        }

        /// <summary>
        /// Registers a service together with an associated type which can be used to determine the original service 
        /// </summary>
        /// <typeparam name="TService">Type of the service to be registered</typeparam>
        /// <typeparam name="TAssociated">Type of the object associated to the service</typeparam>
        /// <param name="serviceInstance">the instance of the service</param>
        /// <returns>the service manager</returns>
        /// <exception cref="ArgumentNullException">if the service instance is null</exception>
        /// <exception cref="InvalidOperationException">if a service of the given type has already been registered</exception>
        public ServiceManager<T> RegisterAssociated<TService, TAssociated>(TService serviceInstance) where TService : T
        {
            if (serviceInstance == null)
                throw new ArgumentNullException(nameof(serviceInstance));

            lock (_instances)
                lock (_associations)
                {
                    if (_instances.ContainsKey(serviceInstance.GetType()) || _providers.ContainsKey(serviceInstance.GetType()) || _associations.ContainsKey(typeof(TAssociated)))
                    {
                        throw new InvalidOperationException($"Service with type {typeof(TService).Name} is already registered");
                    }

                    _associations[typeof(TAssociated)] = serviceInstance.GetType();
                    _instances[serviceInstance.GetType()] = serviceInstance;
                }

            return this;
        }

        /// <summary>
        /// Registers a service provider together with an associated type which can be used to determine the original service 
        /// </summary>
        /// <typeparam name="TService">Type of the service to be registered</typeparam>
        /// <typeparam name="TAssociated">Type of the object associated to the service</typeparam>
        /// <param name="serviceInstance">Provider of said service</param>
        /// <returns>the service manager</returns>
        /// <exception cref="ArgumentNullException">if the service instance is null</exception>
        /// <exception cref="InvalidOperationException">if a service of the given type has already been registered</exception>
        public ServiceManager<T> RegisterProviderAssociated<TService, TAssociated>(Func<T> provider) where TService : T
        {
            lock (_instances)
                lock (_associations)
                {
                    if (_instances.ContainsKey(typeof(TService)) || _providers.ContainsKey(typeof(TService)) || _associations.ContainsKey(typeof(TAssociated)))
                    {
                        throw new InvalidOperationException($"Service with type {typeof(TService).Name} is already registered");
                    }

                    _associations[typeof(TAssociated)] = typeof(TService);
                    _providers[typeof(TService)] = provider;
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

            if (_providers.ContainsKey(serviceType))
            {
                Func<T> provider = _providers[serviceType];
                return provider();
            }

            throw new ArgumentException($"No service of type {serviceType} was registered");
        }

        /// <summary>
        /// Returns base type instance associated to the given type
        /// </summary>
        /// <param name="associatedType">the registered, associated type</param>
        /// <returns>Instance of the base type of this service manager instance</returns>
        /// <exception cref="ArgumentException">if the associated type was not found</exception>
        public T GetAssociated(Type associatedType)
        {
            if(_associations.ContainsKey(associatedType))
            {
                return this.Get(_associations[associatedType]);
            }

            throw new ArgumentException($"No service associated to type {associatedType} was registered");
        }

        /// <summary>
        /// Syntactic sugar for the Get method
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Instance of the base type of this service manager instance</returns>
        public TService Get<TService>() where TService : T => (TService)this.Get(typeof(TService));

        /// <summary>
        /// Syntactic sugar for the GetAssociated method
        /// </summary>
        /// <typeparam name="TAssociated">Associated type</typeparam>
        /// <returns>Instance of the base type of this service manager instance</returns>
        public T GetAssociated<TAssociated>() => this.GetAssociated(typeof(TAssociated));

        /// <summary>
        /// Checks, if an associated type is registered
        /// </summary>
        /// <typeparam name="TAssociation">The type to be checked</typeparam>
        /// <returns></returns>
        public bool CheckAssociation(Type associatedType)
        {
            return _associations.ContainsKey(associatedType);
        }
    }
}
