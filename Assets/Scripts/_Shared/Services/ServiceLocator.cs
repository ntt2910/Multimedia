using System;
using System.Collections.Generic;

namespace BW.Services
{
    public class ServiceLocator
    {
        private static readonly ServiceLocator instance = new ServiceLocator();

        private readonly Dictionary<Type, IService> services;

        private ServiceLocator()
        {
            this.services = new Dictionary<Type, IService>();
        }

        private void RegisterInternal<T>(IService service) where T : IService
        {
            if (this.services.ContainsKey(typeof(T)))
            {
                this.services[typeof(T)] = service;
                return;
            }

            this.services.Add(typeof(T), service);
        }

        private T GetServiceInternal<T>() where T : IService
        {
            if (!this.services.ContainsKey(typeof(T))) return default;
            return (T) this.services[typeof(T)];
        }

        public static void Register<T>(IService service) where T : IService
        {
            instance.RegisterInternal<T>(service);
        }

        public static T GetService<T>() where T : IService
        {
            return instance.GetServiceInternal<T>();
        }
    }
}