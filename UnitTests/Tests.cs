using DependencyInjectionContainer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Tests
    {
        DependenciesConfiguration dependenciesConfiguration;
        DependencyProvider dependencyProvider;

        public enum Names
        {
            First,
            Second,
            Third
        }


        public interface ISimpleDependency { }
        public class SimpleImplementation : ISimpleDependency
        {
            int a = 0;
            bool b = false;

            public override bool Equals(object obj)
            {
                if (obj is SimpleImplementation simpleImplementation)
                    return a == simpleImplementation.a && b == simpleImplementation.b;
                return false;
            }
        }

        public class SimpleWithInnerDependency : ISimpleDependency
        {
            int a = 1;
            bool b = true;
            ISimpleDependency dependency;

            public SimpleWithInnerDependency(ISimpleDependency dependency)
            {
                this.dependency = dependency;
            }

            public override bool Equals(object obj)
            {
                if (obj is SimpleWithInnerDependency sd)
                    return a == sd.a && b == sd.b && dependency.Equals(sd.dependency);
                return false;
            }
        }



        public interface ICollectionDependency { }
        public class CollectionImplementationOne : ICollectionDependency
        {
            int a = 1;
            char b = 'a';
            public override bool Equals(object obj)
            {
                if (obj is CollectionImplementationOne cc)
                    return a == cc.a && b == cc.b;
                return false;
            }
        }
        public class CollectionImplementationTwo : ICollectionDependency
        {
            int a = 2;
            char b = 'b';

            public override bool Equals(object obj)
            {
                if (obj is CollectionImplementationTwo cc)
                    return a == cc.a && b == cc.b;
                return false;
            }
        }
        public class CollectionImplementationThree : ICollectionDependency
        {
            int a = 3;
            char b = 'c';

            public override bool Equals(object obj)
            {
                if (obj is CollectionImplementationThree cc)
                    return a == cc.a && b == cc.b;
                return false;
            }
        }


        public interface ISingletonInterface { }
        public class SingletonClass : ISingletonInterface
        {
            int a = 3;
            char b = 'c';

            public override bool Equals(object obj)
            {
                if (obj is SingletonClass cc)
                    return a == cc.a && b == cc.b;
                return false;
            }

        }

        public interface ISomeInterface { }

        public class SomeClass : ISomeInterface
        {
            int a = 42;
            char b = 'g';

            public override bool Equals(object obj)
            {
                if (obj is SomeClass cc)
                    return a == cc.a && b == cc.b;
                return false;
            }
        }

        public interface IGeneric<TConstrained> 
            where TConstrained : ISomeInterface
        { }

        public class GenericClass<TConstrainedClass> : IGeneric<TConstrainedClass> 
            where TConstrainedClass : ISomeInterface
        {
            TConstrainedClass tConstrainedClass;
            public GenericClass(TConstrainedClass constrained)
            {
                tConstrainedClass = constrained;
            }

            public override bool Equals(object obj)
            {
                if (obj is GenericClass<TConstrainedClass> c)
                {
                    return c.tConstrainedClass.Equals(tConstrainedClass);
                }
                return false;
            }
        }

        public class AnotherSimpleclass : ISimpleDependency
        {
            ICollectionDependency dependency;

            public AnotherSimpleclass([DependencyName(Names.Second)]ICollectionDependency dependency)
            {
                this.dependency = dependency;
            }

            public override bool Equals(object obj)
            {
                if (obj is AnotherSimpleclass c)
                {
                    return c.dependency.Equals(dependency);
                }
                return false;
            }
        }




        [SetUp]
        public void Setup()
        {
            dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<ISimpleDependency, SimpleImplementation>();
            dependenciesConfiguration.Register<ISimpleDependency, SimpleWithInnerDependency>();
            dependenciesConfiguration.Register<ICollectionDependency, CollectionImplementationOne>(LifeCycle.INSTANCE_PER_DEPENDENCY);
            dependenciesConfiguration.Register<ICollectionDependency, CollectionImplementationTwo>(LifeCycle.INSTANCE_PER_DEPENDENCY);
            dependenciesConfiguration.Register<ICollectionDependency, CollectionImplementationThree>(LifeCycle.INSTANCE_PER_DEPENDENCY);
            dependenciesConfiguration.Register<ISingletonInterface, SingletonClass>(LifeCycle.SINGLETON);
            dependenciesConfiguration.Register<ISomeInterface, SomeClass>();
            dependenciesConfiguration.Register<IGeneric<ISomeInterface>, GenericClass<ISomeInterface>>();
            dependenciesConfiguration.Register<ISimpleDependency, AnotherSimpleclass>();
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
        }

        [Test]
        public void SimpleDependencyTest()
        {
            var actual = dependencyProvider.Resolve<ISimpleDependency>(Names.First);
            var expected = new SimpleImplementation();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SimpleInnerDependencyTest()
        {
            var actual = dependencyProvider.Resolve<ISimpleDependency>(Names.Second);
            var expected = new SimpleWithInnerDependency(new SimpleImplementation());

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void CollectionTest()
        {
            var actual = dependencyProvider.Resolve<IEnumerable<ICollectionDependency>>();
            var expected = new ICollectionDependency[] { new CollectionImplementationOne(),
                                                         new CollectionImplementationTwo(),
                                                         new CollectionImplementationThree()};
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingletonTest()
        {
            var expected = dependencyProvider.Resolve<ISingletonInterface>();
            var actual = dependencyProvider.Resolve<ISingletonInterface>();
            var anotherActual = dependencyProvider.Resolve<ISingletonInterface>();

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, anotherActual);
        }

        [Test]
        public void GenericTest()
        {
            var actual = dependencyProvider.Resolve<IGeneric<ISomeInterface>>();
            var expected = new GenericClass<ISomeInterface>(new SomeClass());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NamedDependencyTest()
        {
            var actual = dependencyProvider.Resolve<ISimpleDependency>(Names.Third);
            var expected = new AnotherSimpleclass(new CollectionImplementationTwo());

            Assert.AreEqual(expected, actual);
        }
    }
}