using NUnit.Framework;
using UnityEngine;
using UnityIoc.Runtime;
using UnityIoc.Runtime.Attributes;

namespace _PackageRoot.Tests.Runtime
{
    public class Test_Container
    {
        public interface ITestInterface { }
        public class TestImplementation : ITestInterface { }

        public class TestImplementationWithInjection : ITestInterface
        {
            [Inject]
            public ITestInterface testInterfaceField;

            [Inject]
            public TestImplementation testImplementationField;

            [Inject]
            public ITestInterface testInterfaceProp { get; set; }

            [Inject]
            public TestImplementation testImplementationProp { get; set; }

        }

        public class MonoSingleton : Singleton<MonoSingleton> { }
        public class MultiSceneMonoSingleton : MultiSceneSingleton<MultiSceneMonoSingleton> { }

        [Test]
        public void TestRegisteringSingletonWithInterface() {
            Container.RegisterSingleton<ITestInterface, TestImplementation>();
            var instance = Container.Resolve<ITestInterface>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<TestImplementation>(instance);
        }

        [Test]
        public void TestRegisteringSingletonWithoutInterface() {
            Container.RegisterSingleton<TestImplementation>();
            var instance = Container.Resolve<TestImplementation>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<TestImplementation>(instance);
        }

        [Test]
        public void TestRegisteringTransientWithInterface() {
            Container.RegisterTransient<ITestInterface, TestImplementation>();
            var instance = Container.Resolve<ITestInterface>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<TestImplementation>(instance);
        }

        [Test]
        public void TestInjecting() {
            Container.RegisterTransient<ITestInterface, TestImplementation>();
            Container.RegisterSingleton<TestImplementationWithInjection>();
            var instance = Container.Resolve<TestImplementationWithInjection>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<TestImplementationWithInjection>(instance);

            Assert.IsNotNull(instance.testInterfaceField);
            Assert.IsNotNull(instance.testImplementationField);
            Assert.IsNotNull(instance.testInterfaceProp);
            Assert.IsNotNull(instance.testImplementationProp);
        }

        [Test]
        public void TestMonoSingleton() {
            Assert.IsNull(Object.FindFirstObjectByType<MonoSingleton>());
            Container.RegisterSingleton<MonoSingleton>();

            var instance = Container.Resolve<MonoSingleton>();
            
            Assert.IsNotNull(Object.FindFirstObjectByType<MonoSingleton>());
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<MonoSingleton>(instance);
        }

        [Test]
        public void TestMultiSceneMonoSingleton() {
            Assert.IsNull(Object.FindFirstObjectByType<MultiSceneMonoSingleton>());
            Container.RegisterSingleton<MultiSceneMonoSingleton>();

            var instance = Container.Resolve<MultiSceneMonoSingleton>();
            
            Assert.IsNotNull(Object.FindFirstObjectByType<MultiSceneMonoSingleton>());
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<MultiSceneMonoSingleton>(instance);
        }


    }
}