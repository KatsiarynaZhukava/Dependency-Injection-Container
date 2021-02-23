using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        internal Dictionary<Type, List<Implementation>> dependencies = new Dictionary<Type, List<Implementation>>();

        public void Register<TDependency, TImplementation>(LifeCycle lifeCycle = LifeCycle.INSTANCE_PER_DEPENDENCY)
        {
            Register(typeof(TDependency), typeof(TImplementation), lifeCycle);
        }

        public void Register(Type TDependency, Type TImplementation, LifeCycle lifeCycle = LifeCycle.INSTANCE_PER_DEPENDENCY)
        {
            if (TDependency == null || TImplementation == null)
                throw new Exception("Cannot register dependency: TDependency and TImplementation must not be null");
            if (!(TDependency.IsClass || TDependency.IsInterface))
                throw new Exception("Cannot register dependency: TDependency must be of reference type");
            if (TImplementation.IsAbstract)
                throw new Exception("Cannot register dependency: TImplementation must not be abstract.");


            if (TImplementation.GetConstructors().Length != 0)
            {
                if (!dependencies.ContainsKey(TDependency))
                {
                    dependencies.Add(TDependency, new List<Implementation> { new Implementation(TImplementation, lifeCycle) });
                }
                else
                {
                    dependencies[TDependency].Add(new Implementation(TImplementation, lifeCycle));
                }
            }
            else
            {
                throw new Exception("Cannot register dependency: no constructors for the implementation found.");
            }
        }
    }
}