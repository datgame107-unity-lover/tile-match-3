using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator 
{
    private static readonly Dictionary<Type, object> services = new();
    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"[ServiceLocator] Overwriting: {type.Name}");
        }
        services[type]  = service;
    }
    public static T Get<T>() where T : class
    {
        if(services.TryGetValue(typeof(T), out var service))
            return (T)service;
        throw new Exception($"[ServiceLocator] Not found: {typeof(T).Name}");
    }
    public static bool TryGet<T>(out T service) where T: class
    {
        if(services.TryGetValue(typeof(T),out var raw))
        {
            service = (T)raw;
            return true;
        }
        service = null;
        return false;
    }
    public static void Unregister<T>()where T : class
    {
        services.Remove(typeof(T));
    }
    public static void Clear()
    {     
        services.Clear();
    }
}


public static class ServiceLocato
{
    private static readonly Dictionary<Type,object> services = new();
    public static void Register<T>(T service) where T : class
    {
        var type = typeof (T);
        if (!services.ContainsKey(type))
        {
            services[type] = service;
        }
    }
    public static T Get<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;

        throw new Exception($"[ServiceLocator] Not found: {typeof(T).Name}");

    }

    public static void Unregister<T>(T service) where T : class
    {
        services.Remove(typeof(T));
    }
    public static void Clear()
    {
        services.Clear();
    }
}
