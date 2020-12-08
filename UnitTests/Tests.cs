using NUnit.Framework;
using DependencyInjectionContainer;
using UnitTests.TestedDependencies;

namespace Tests
{
    public class Tests
    {
        DependencyProvider dependencyProvider;


        [SetUp]
        public void Setup()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<ISimpleInterface, SimpleClass>();
            dependenciesConfiguration.Register<ISeveralImplementations, ImplementationOne>();
            dependenciesConfiguration.Register<ISeveralImplementations, ImplementationTwo>();


            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}