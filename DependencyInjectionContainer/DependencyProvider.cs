using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionContainer
{
    public class DependencyNameAttribute : Attribute
    {
        public Enum Name { get; }
        public DependencyNameAttribute(object name)
        {
            Name = (Enum)name;
        }
    }

    public class DependencyProvider
    {
        private Dictionary<Type, List<Implementation>> dependencies;
        private ConcurrentDictionary<Type, object> singletons = new ConcurrentDictionary<Type, object>();

        public DependencyProvider(DependenciesConfiguration dependenciesConfiguration)
        {
            dependencies = dependenciesConfiguration.dependencies;
        }


        private object GenerateImplementation(Implementation implementation)
        {
            if (singletons.ContainsKey(implementation.TImplementation))
                return singletons[implementation.TImplementation];
            else
            {
                ConstructorInfo constructorInfo = implementation.TImplementation.GetConstructors().First();
                if (constructorInfo == null)
                    throw new Exception("Cannot find any constructors");

                ParameterInfo[] constuctorParameters = constructorInfo.GetParameters();
                object[] invokeArgs = new object[constuctorParameters.Length];

                for (int i = 0; i < constuctorParameters.Length; i++)
                {
                    if (constuctorParameters[i].ParameterType.IsValueType)
                        throw new Exception("Implementation constructor takes invalid parameters.");

                    DependencyNameAttribute attribute = constuctorParameters[i].GetCustomAttribute<DependencyNameAttribute>();
                    if (attribute == null)
                    {
                        invokeArgs[i] = Resolve(constuctorParameters[i].ParameterType);
                    }
                    else
                    {
                       invokeArgs[i] = Resolve(constuctorParameters[i].ParameterType, Convert.ToInt32(attribute.Name));
                    }
                }

                object result = Activator.CreateInstance(implementation.TImplementation, invokeArgs);

                if (implementation.LifeCycle == LifeCycle.SINGLETON)
                {
                    if (!singletons.TryAdd(implementation.TImplementation, result))
                    {
                        return singletons[implementation.TImplementation];
                    }
                }
                return result;
            }
        }


        public TDependency Resolve<TDependency>(Enum namedDependency = null)
        {
            return (TDependency)Resolve(typeof(TDependency), Convert.ToInt32(namedDependency));
        }

        private object Resolve(Type tDependency, int namedDependency = 0)
        {
            Implementation implementation = null;


            if (typeof(IEnumerable).IsAssignableFrom(tDependency))
            {
                tDependency = tDependency.GetGenericArguments().First();
                if (dependencies.ContainsKey(tDependency))
                {
                    var implementations = Array.CreateInstance(tDependency, dependencies[tDependency].Count);
                    for (int i = 0; i < dependencies[tDependency].Count; i++)
                        implementations.SetValue(GenerateImplementation(dependencies[tDependency][i]), i);
                    return implementations;
                }
                else
                {
                    throw new Exception("Such dependency does not exist.");
                }
            }

            if (tDependency.GenericTypeArguments.Length != 0)
            {
                if (!dependencies.ContainsKey(tDependency) && dependencies.ContainsKey(tDependency.GetGenericTypeDefinition()))
                {
                    if (dependencies[tDependency.GetGenericTypeDefinition()].Count <= namedDependency)
                        throw new Exception("Such named dependency does not exist.");

                    Type constructedType = dependencies[tDependency.GetGenericTypeDefinition()][namedDependency]
                        .TImplementation
                        .MakeGenericType(tDependency.GetGenericArguments().First());


                    implementation = new Implementation(constructedType, dependencies[tDependency.GetGenericTypeDefinition()][namedDependency].LifeCycle);
                    tDependency = tDependency.GetGenericArguments().First();
                }
            }

            if (!dependencies.ContainsKey(tDependency))
                throw new Exception("Specified dependency does not exist");

            if (implementation == null)
            {
                if (dependencies[tDependency].Count <= namedDependency)
                    throw new Exception("Such named dependency does not exist.");
                implementation = dependencies[tDependency][namedDependency];
            }
            return GenerateImplementation(implementation);
        }      
    }
}
