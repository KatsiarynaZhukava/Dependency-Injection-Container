using System;

namespace DependencyInjectionContainer
{
    public enum LifeCycle
    {
        INSTANCE_PER_DEPENDENCY,
        SINGLETON
    }

    class Implementation
    {
        public Type TImplementation { get; }
        public LifeCycle LifeCycle { get; set; }

        public Implementation(Type implementation, LifeCycle lifeCycle)
        {
            TImplementation = implementation;
            LifeCycle = lifeCycle;
        }
    }
}
