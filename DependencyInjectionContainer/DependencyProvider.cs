using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private Dictionary<Type, List<Implementation>> dependencies;


        public TDependency Resolve<TDependency>(Enum namedDependency = null)
        {
            return (TDependency)Resolve(typeof(TDependency), Convert.ToInt32(namedDependency));
        }

        private object Resolve(Type tDependency, int namedDependency = 0)
        {
            return null;
        }


            public DependencyProvider(DependenciesConfiguration dependenciesConfiguration)
        {
            dependencies = dependenciesConfiguration.dependencies;
        }
    }
}
