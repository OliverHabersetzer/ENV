using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ENV.Runtime
{
    public class PluginManager<T> where T : class
    {
        public readonly Type _pluginType = typeof(T);

        public Type[] Plugins { get; private set; } = new Type[0];
        public Assembly[] AssembliesWithPlugins { get; private set; } = new Assembly[0];

        public Type[] LoadPlugins()
        {
            var assemblies = new HashSet<Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                assemblies.Add(assembly);

            int size = assemblies.Count;
            int previousSize = 0;


            while (size > previousSize)
            {
                foreach (var assembly in assemblies.ToArray())
                {
                    foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                    {
                        assemblies.Add(Assembly.Load(referencedAssemblyName));
                    }
                }

                previousSize = size;
                size = assemblies.Count;
            }

            Plugins = assemblies
                .Where(assembly => assembly != null)
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => _pluginType != type && _pluginType.IsAssignableFrom(type))
                .ToArray();

            AssembliesWithPlugins = Plugins
                .Select(plugin => plugin.Assembly)
                .Distinct()
                .ToArray();

            return Plugins;
        }

        public T InstantiatePlugin(Type pluginType)
        {
            return Activator.CreateInstance(pluginType) as T;
        }

        public T InstantiatePlugin(Type pluginType, params object[] parameters)
        {
            return pluginType.GetConstructor(
                parameters
                    .Select(parameter => parameter.GetType())
                    .ToArray()
            )?.Invoke(parameters) as T;
        }
    }
}