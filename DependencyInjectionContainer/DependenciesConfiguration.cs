using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        internal Dictionary<Type, List<Implementation>> dependencies = new Dictionary<Type, List<Implementation>>();

        public void Register(Type TDependency, Type TImplementation, LifeCycle lifeCycle = LifeCycle.INSTANCE_PER_DEPENDENCY)
        {
            if (TDependency.IsValueType)
                throw new Exception("TDependency must be of reference type");
            if (TImplementation.IsAbstract)
                throw new Exception("TImplementation can not be abstract.");

            if (TImplementation.GetConstructors().Length > 0)
            {
                dependencies.Add(
                        TDependency,
                        new List<Implementation> { new Implementation(TImplementation, lifeCycle) });
            }

            throw new Exception("TImplementation should have at least one public constructor.");
        }

        public void Register<TDependency, TImplementation>(LifeCycle lifeCycle = LifeCycle.INSTANCE_PER_DEPENDENCY)
            where TImplementation : TDependency
            where TDependency : class
        {
            Register(typeof(TDependency), typeof(TImplementation), lifeCycle);
        }
    }
}