using System;
using System.Collections.Generic;

namespace Game.Application.Core
{
    /// <summary>
    /// Lightweight dependency injection container.
    /// Type-safe, minimal reflection, explicit lifetime management.
    /// 
    /// Design principles:
    /// - Explicit over implicit (all registrations clear)
    /// - Type-safe (compile-time checking)
    /// - Fast resolution (dictionary-based lookups)
    /// - Clear ownership of lifetimes
    /// - No service locator anti-pattern (exists for bootstrapping only)
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private class ServiceDescriptor
        {
            public Type InterfaceType { get; set; }
            public object Instance { get; set; }
            public Delegate Factory { get; set; }
            public bool IsSingleton { get; set; }
        }

        private readonly Dictionary<Type, ServiceDescriptor> _services = new();
        private readonly HashSet<Type> _resolving = new(); // To detect circular dependencies

        /// <summary>
        /// Register a service singleton by instance.
        /// </summary>
        public void Register<TInterface>(TInterface instance) where TInterface : IService
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = typeof(TInterface);
            _services[type] = new ServiceDescriptor
            {
                InterfaceType = type,
                Instance = instance,
                IsSingleton = true
            };
        }

        /// <summary>
        /// Register a service singleton by factory.
        /// Factory is called once; result cached.
        /// </summary>
        public void Register<TInterface>(Func<IServiceContainer, TInterface> factory) where TInterface : IService
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var type = typeof(TInterface);
            _services[type] = new ServiceDescriptor
            {
                InterfaceType = type,
                Factory = factory,
                IsSingleton = true
            };
        }

        public void Register(Type interfaceType, object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!interfaceType.IsInstanceOfType(instance))
                throw new InvalidOperationException($"{instance.GetType().Name} không thực thi {interfaceType.Name}");

            _services[interfaceType] = new ServiceDescriptor
            {
                InterfaceType = interfaceType,
                Instance = instance,
                IsSingleton = true
            };
        }

        /// <summary>
        /// Register a transient service by factory.
        /// Factory called each time; no caching.
        /// </summary>
        public void RegisterTransient<TInterface>(Func<IServiceContainer, TInterface> factory) where TInterface : IService
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var type = typeof(TInterface);
            _services[type] = new ServiceDescriptor
            {
                InterfaceType = type,
                Factory = factory,
                IsSingleton = false
            };
        }

        /// <summary>
        /// Resolve a service. Throws if not registered.
        /// </summary>
        public TInterface Resolve<TInterface>() where TInterface : IService
        {
            var type = typeof(TInterface);

            if (!_services.TryGetValue(type, out var descriptor))
            {
                throw new InvalidOperationException(
                    $"Service {type.Name} not registered in ServiceContainer. " +
                    $"Register it before resolution."
                );
            }

            return ResolveDescriptor<TInterface>(descriptor);
        }

        /// <summary>
        /// Try to resolve a service.
        /// </summary>
        public bool TryResolve<TInterface>(out TInterface service) where TInterface : IService
        {
            service = default(TInterface);
            var type = typeof(TInterface);

            if (!_services.TryGetValue(type, out var descriptor))
                return false;

            try
            {
                service = ResolveDescriptor<TInterface>(descriptor);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public object TryResolve(Type type)
        {
            // Tìm kiếm trong Dictionary _services của bạn
            if (_services.TryGetValue(type, out var descriptor))
            {
                // Nếu tìm thấy Descriptor, trả về Instance
                return descriptor.Instance;
            }
            
            // Nếu không tìm thấy, trả về null thay vì ném Exception
            return null;
        }

        /// <summary>
        /// Check if a service is registered.
        /// </summary>
        public bool IsRegistered<TInterface>() where TInterface : IService
        {
            return _services.ContainsKey(typeof(TInterface));
        }

        public bool IsRegistered(Type type)
        {
            return _services.ContainsKey(type);
        }

        private TInterface ResolveDescriptor<TInterface>(ServiceDescriptor descriptor) where TInterface : IService
        {
            var type = typeof(TInterface);

            // Detect circular dependencies
            if (_resolving.Contains(type))
            {
                throw new InvalidOperationException(
                    $"Circular dependency detected while resolving {type.Name}. " +
                    $"Review your service dependencies."
                );
            }

            try
            {
                _resolving.Add(type);

                // Already have singleton instance
                if (descriptor.Instance != null)
                    return (TInterface)descriptor.Instance;

                // Create via factory
                if (descriptor.Factory != null)
                {
                    var factory = (Func<IServiceContainer, TInterface>)descriptor.Factory;
                    var instance = factory(this);

                    // Cache singleton
                    if (descriptor.IsSingleton)
                        descriptor.Instance = instance;

                    return instance;
                }

                throw new InvalidOperationException(
                    $"Service {type.Name} has no instance or factory."
                );
            }
            finally
            {
                _resolving.Remove(type);
            }
        }
    }
}
