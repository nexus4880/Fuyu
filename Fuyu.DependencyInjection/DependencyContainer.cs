using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fuyu.DependencyInjection.Attributes;
using Fuyu.DependencyInjection.Registrations;

namespace Fuyu.DependencyInjection;

public class DependencyContainer
{
    private readonly Dictionary<Type, List<AbstractDependencyRegistration>> _registrations;

    public DependencyContainer()
    {
        _registrations = new Dictionary<Type, List<AbstractDependencyRegistration>>();
    }

    #region Registrations

    private void AddRegistration(Type type, AbstractDependencyRegistration registration)
    {
        if (!_registrations.TryGetValue(type, out var registrations))
        {
            _registrations[type] = registrations = new List<AbstractDependencyRegistration>();
        }
        else
        {
            var index = registrations.FindIndex(d => d.Id == registration.Id);
            if (index != -1)
            {
                Console.WriteLine($"Prevented multi registering of {registration.Id}");
                registrations[index] = registration;
                return;
            }
        }

        registrations.Add(registration);
    }

    private List<AbstractDependencyRegistration> GetRegistrations(Type type)
    {
        if (_registrations.TryGetValue(type, out var registrations))
        {
            return registrations;
        }

        return new List<AbstractDependencyRegistration>();
    }

    #endregion

    #region Resolve

    /// <param name="id">Can be null -- if null will not search for existing registrations but will create a new instance</param>
    public object Resolve(string id, Type type)
    {
        // If we have an id we are willing to take an existing registration
        if (id != null)
        {
            // Get registrations by type
            var registrations = GetRegistrations(type);
            foreach (var registration in registrations)
            {
                // Find the one with the id we are looking for
                if (registration.Id == id)
                {
                    return registration.GetValue();
                }
            }
        }

        // If id was not found (or we didn't specify it) try to create one
        // We cannot create an instance of an interface or abstract type
        if (type.IsInterface || type.IsAbstract)
        {
            throw new InvalidOperationException($"Cannot create instance of unimplemented type {type.Name}");
        }

        // Find constructor that is injectable
        var constructor = type
            .GetConstructors()
            .FirstOrDefault(c => c.GetCustomAttribute<InjectableAttribute>() != null);

        if (constructor == null)
        {
            throw new InvalidOperationException($"Cannot find injectable constructor on {type.Name}, did you forget the attribute?");
        }

        var parameters = constructor.GetParameters();

        if (!parameters.All(p => p.GetCustomAttribute<InjectAttribute>() != null))
        {
            throw new ArgumentException($"One or more parameters on {type.Name} are not injectable");
        }

        var parameterArguments = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            // Assume not null because above check passed
            var injectAttribute = parameter.GetCustomAttribute<InjectAttribute>();
            var parameterType = parameter.ParameterType;

            if (injectAttribute is InjectAllAttribute)
            {
                var parameterTypeGenericArguments = parameterType.GetGenericArguments();

                if (parameterTypeGenericArguments.Length != 1)
                {
                    throw new Exception($"Using InjectAll but {parameter.Name} does not have 1 generic argument");
                }

                var injectAllType = parameterTypeGenericArguments[0];
                // Using List<T> instead of HashSet<T> to avoid needlessly hashing objects we know SHOULD be unique
                var genericListType = typeof(List<>).MakeGenericType(injectAllType);

                if (!parameterType.IsAssignableFrom(genericListType))
                {
                    throw new Exception($"Using InjectAll but {parameter.Name} uses type {parameterType.Name} when it should be List<T>");
                }

                // Array.Empty<Type> to target ResolveAll<T>() instead of ResolveAll(Type)
                parameterArguments[i] =
                    typeof(DependencyContainer)
                    .GetMethod(nameof(ResolveAll), Array.Empty<Type>())
                    .MakeGenericMethod(injectAllType)
                    .Invoke(this, Array.Empty<object>());
            }
            else
            {
                if (injectAttribute.Id == null)
                {
                    if (parameter.ParameterType != typeof(DependencyContainer))
                    {
                        throw new Exception($"Cannot inject '{parameter.Name}' on constructor for {type.Name} because dependency id is null and it is not requesting the container");
                    }

                    parameterArguments[i] = this;
                }
                else
                {
                    parameterArguments[i] = Resolve(injectAttribute.Id, parameterType);
                }
            }
        }

        return constructor.Invoke(parameterArguments);
    }

    /// <summary>
    /// Only used to create an instance of an injectable
    /// </summary>
    public object Resolve(Type type)
    {
        return Resolve(null, type);
    }

    /// <summary>
    /// Only used to create an instance of an injectable
    /// </summary>
    public T Resolve<T>() where T : class
    {
        return (T)Resolve(typeof(T));
    }

    /// <summary>
    /// Used to resolve an object of the desired type and id
    /// </summary>
    public T Resolve<T>(string id) where T : class
    {
        return (T)Resolve(id, typeof(T));
    }

    /// <summary>
    /// Used to resolve an object of the desired type and id
    /// </summary>
    public T Resolve<TBase, T>(string id) where T : class
        where TBase : class
    {
        return (T)Resolve(id, typeof(TBase));
    }

    public T Resolve<TBase, T>() where T : class
        where TBase : class
    {
        return (T)Resolve(typeof(T).Name, typeof(TBase));
    }

    #endregion

    #region ResolveAll

    public List<object> ResolveAll(Type type)
    {
        var registrations = GetRegistrations(type);
        var instances = new List<object>(registrations.Count);

        for (var i = 0; i < registrations.Count; i++)
        {
            instances.Add(registrations[i].GetValue());
        }

        return instances;
    }

    public List<T> ResolveAll<T>() where T : class
    {
        var registrations = GetRegistrations(typeof(T));
        var instances = new List<T>(registrations.Count);

        for (var i = 0; i < registrations.Count; i++)
        {
            instances.Add((T)registrations[i].GetValue());
        }

        return instances;
    }

    #endregion

    #region Register

    #region Transient

    public void RegisterTransient<TBase, T>(string id, Func<T> factory) where T : class
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(id, factory));
    }

    public void RegisterTransient<TBase, T>(string id) where T : class, new()
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(id, () => new T()));
    }

    public void RegisterTransientResolved<TBase, T>(string id) where T : class
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(id, Resolve<T>));
    }

    public void RegisterTransient<T>(string id, Func<T> factory) where T : class
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(id, factory));
    }

    public void RegisterTransient<T>(string id) where T : class, new()
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(id, () => new T()));
    }

    public void RegisterTransientResolved<T>(string id) where T : class
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(id, Resolve<T>));
    }

    public void RegisterTransient<TBase, T>(Func<T> factory) where T : class
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(typeof(T).Name, factory));
    }

    public void RegisterTransient<TBase, T>() where T : class, new()
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(typeof(T).Name, () => new T()));
    }

    public void RegisterTransientResolved<TBase, T>() where T : class
    {
        AddRegistration(typeof(TBase), new TransientRegistration<T>(typeof(T).Name, Resolve<T>));
    }

    public void RegisterTransient<T>(Func<T> factory) where T : class
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(typeof(T).Name, factory));
    }

    public void RegisterTransient<T>() where T : class, new()
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(typeof(T).Name, () => new T()));
    }

    public void RegisterTransientResolved<T>() where T : class
    {
        AddRegistration(typeof(T), new TransientRegistration<T>(typeof(T).Name, Resolve<T>));
    }

    #endregion

    #region Singleton

    public void RegisterSingleton<TBase, T>(string id, T instance) where T : class
    {
        AddRegistration(typeof(TBase), new SingletonRegistration<T>(id, instance));
    }

    public void RegisterSingleton<TBase, T>(string id) where T : class, new()
    {
        AddRegistration(typeof(TBase), new SingletonRegistration<T>(id, new T()));
    }

    public void RegisterSingleton<TBase, T>(T instance) where T : class
    {
        AddRegistration(typeof(TBase), new SingletonRegistration<T>(typeof(T).Name, instance));
    }

    public void RegisterSingleton<TBase, T>() where T : class, new()
    {
        AddRegistration(typeof(TBase), new SingletonRegistration<T>(typeof(T).Name, new T()));
    }

    public void RegisterSingleton<T>(string id, T instance) where T : class
    {
        AddRegistration(typeof(T), new SingletonRegistration<T>(id, instance));
    }

    public void RegisterSingleton<T>(string id) where T : class, new()
    {
        AddRegistration(typeof(T), new SingletonRegistration<T>(id, new T()));
    }

    public void RegisterSingleton<T>(T instance) where T : class
    {
        AddRegistration(typeof(T), new SingletonRegistration<T>(typeof(T).Name, instance));
    }

    public void RegisterSingleton<T>() where T : class, new()
    {
        AddRegistration(typeof(T), new SingletonRegistration<T>(typeof(T).Name, new T()));
    }

    #endregion

    #endregion
}