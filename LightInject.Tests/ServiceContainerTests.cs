﻿namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceContainerTests
    {
        #region Values

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("SomeValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsLastRegisteredValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            container.Register("AnotherValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("AnotherValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnSameValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            var value1 = (string)container.GetInstance(typeof(string));
            var value2 = (string)container.GetInstance(typeof(string));
            Assert.AreSame(value1, value2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register(42);
            var value = (int)container.GetInstance(typeof(int));
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void GetInstance_NamedValue_ReturnsNamedValue()
        {
            var container = CreateContainer();
            container.Register(42, "Answer");
            var value = (int)container.GetInstance(typeof(int), "Answer");
            Assert.AreEqual(42, value);
        }

        #endregion
        [TestMethod]
        public void GetInstance_OneService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, Foo>("AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TwoNamedServices_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo)), ErrorMessages.UnableToResolveType);
        }

        [TestMethod]
        public void GetInstance_DuplicateRegistration_ReturnsLastRegisteredService()
        {
            var container = CreateContainer();            
            container.Register<IFoo, Foo>();
            container.Register<IFoo>(new AnotherFoo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_UnknownGenericType_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar<>), typeof(Bar<>));
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo<int>)), ErrorMessages.UnknownGenericDependency);
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstanceAfterGettingDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.GetInstance(typeof(IFoo), "AnotherFoo");
            object defaultInstance = container.GetInstance(typeof(IFoo));            
            Assert.IsInstanceOfType(defaultInstance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OneNamedService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OneNamedClosedGenericService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_NamedService_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance<IFoo>("SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsInstanceOfLastRegisteredType()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_NamedOpenGenericType_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsClosedGenericInstancesIfPresent()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<string>), typeof(FooWithStringTypeParameter));
            var instance = container.GetInstance(typeof(IFoo<string>));
            Assert.IsInstanceOfType(instance, typeof(FooWithStringTypeParameter));
        }

        [TestMethod]
        public void GetInstance_DefaultAndNamedOpenGenericType_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_TwoNamedOpenGenericTypes_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericTypeWithDependency_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
            var instance = (FooWithGenericDependency<IBar>)container.GetInstance(typeof(IFoo<IBar>));
            Assert.IsInstanceOfType(instance.Dependency, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_OpenGenericSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_CallsConstructorOnlyOnce()
        {
            var container = CreateContainer();
            Foo.Instances = 0;
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());            
            container.GetInstance(typeof(IFoo));
            container.GetInstance(typeof(IFoo));
            Assert.AreEqual(1, Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_NamedSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo", new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo), "SomeFoo");
            var instance2 = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_PerScopeService_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo>();
                var instance2 = container.GetInstance<IFoo>();
                Assert.AreSame(instance1, instance2);
            }            
        }

        [TestMethod]
        public void GetInstance_GenericServiceWithPerScopeLifetime_DoesNotShareLifetimeInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerScopeLifetime());
            using(container.BeginScope())
            {
                var intInstance = container.GetInstance<IFoo<int>>();
                var stringInstance = container.GetInstance<IFoo<string>>();
                Assert.IsInstanceOfType(intInstance, typeof(IFoo<int>));
                Assert.IsInstanceOfType(stringInstance, typeof(IFoo<string>));
            }            
        }
        
        [TestMethod]
        public void GetInstance_ServiceWithPerScopeLifeTimeOutSideResolutionScope_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.AreNotSame(firstInstance, secondInstance);
        }

        #region Func Services

        [TestMethod]
        public void GetInstance_Func_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<IFoo>));
            Assert.IsInstanceOfType(factory, typeof(Func<IFoo>));
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<string, IFoo>));
            Assert.IsInstanceOfType(factory, typeof(Func<string, IFoo>));
        }

        [TestMethod]
        public void GetInstance_Func_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var factory2 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            Assert.AreSame(factory1, factory2);
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            var factory2 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            Assert.AreSame(factory1, factory2);
        }

        [TestMethod]
        public void GetInstance_Func_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance = factory();
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            var factory = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            var instance = factory("SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FuncWithTransientTarget_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreNotSame(instance1, instance2);
        }
        #endregion

        #region Func Factory

        [TestMethod]
        public void GetInstance_FuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo());
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }
    
        [TestMethod]
        public void GetInstance_FuncFactory_ReturnsLastRegisteredFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithMultipleConstructors());
            container.Register<IFoo>(c => new FooWithMultipleConstructors(new Bar()));
            var instance = (FooWithMultipleConstructors)container.GetInstance(typeof(IFoo));            
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_NamedFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_NamedSingletonFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo", new PerContainerLifetime());
            var firstInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            var secondInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar>(c => new Bar());
            container.Register<IFoo>(c => new FooWithDependency(c.GetInstance<IBar>()));
            var instance = (FooWithDependency)container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithReferenceTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithReferenceTypeDependency("SomeStringValue"));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual("SomeStringValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithValueTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithValueTypeDependency(42));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithEnumDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithEnumDependency(Encoding.UTF8));
            var instance = (FooWithEnumDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual(Encoding.UTF8, instance.Value);
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactory_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithMethodCall_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo());
            var foo = container.GetInstance<IFoo>();
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactoryWithMethodCall_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo(), new PerContainerLifetime());
            var instance1 = container.GetInstance<IFoo>();
            var instance2 = container.GetInstance<IFoo>();
            Assert.AreSame(instance1, instance2);
        }

        private IFoo GetFoo()
        {
            return new Foo();
        }

        #endregion

        #region IEnumerable

        [TestMethod]
        public void GetInstance_IEnumerable_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GenericGetAllInstances_UnknownService_ReturnsEmptyIEnumerable()
        {
            var container = CreateContainer();
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetInstance_IEnumerableWithReferenceTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            container.Register("AnotherValue", "AnotherStringValue");
            var services = container.GetInstance<IEnumerable<string>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GetInstance_IEnumerableWithValueTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(1024);
            container.Register(2048, "AnotherInt");
            var services = container.GetInstance<IEnumerable<int>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GetInstance_IEnumerable_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instance1 = container.GetInstance<IEnumerable<IFoo>>();
            var instance2 = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreNotSame(instance1, instance2);
        }
               
        [TestMethod]
        public void GetAllInstances_NonGeneric_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetAllInstances_Generic_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetAllInstances_TwoOpenGenericServices_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetAllInstances_ClosedAndOpenGenericService_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetAllInstances_EnumerableWithRecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetAllInstances<IFoo>(), ex => ex.InnerException.InnerException.InnerException.Message == ErrorMessages.RecursiveDependency);
        }

        [TestMethod]
        public void GetInstance_UsingServicePredicate_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(IFoo));
        }

        [TestMethod]
        public void GetInstance_PerContainerLifetimeUsingServicePredicate_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo(), new PerContainerLifetime());
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void CanGetInstance_KnownService_ReturnsTrue()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IFoo), string.Empty);
            Assert.IsTrue(canCreateInstance);
        }
        [TestMethod]
        public void CanGetInstance_UnknownService_ReturnFalse()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IBar), string.Empty);
            Assert.IsFalse(canCreateInstance);
        }      

        [TestMethod]
        public void GetInstance_RegisterAfterGetInstance_ReturnsInstanceOfSecondRegistration()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.GetInstance<IFoo>();            
            container.Register<IFoo, AnotherFoo>();

            var instance = container.GetInstance<IFoo>();
            
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_RegisterAfterGetInstance_ReturnsDependencyOfSecondRegistration()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();
            container.GetInstance<IFoo>();            
            container.Register<IBar, AnotherBar>();

            var instance = (FooWithDependency)container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));
        }

        [TestMethod]
        public void GetInstance_SingletonRegisterAfterInvalidate_ReturnsInstanceOfSecondRegistration()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            container.GetInstance<IFoo>();
            container.Register<IFoo, AnotherFoo>(new PerContainerLifetime());

            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
            
        }

        [TestMethod]
        public void Run()
        {
            for (int i = 0; i < 1; i++)
            {
                GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance();
            }
        }
        
        [TestMethod]
        public void GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            Foo.Instances = 0;
            IList<IFoo> instances = new List<IFoo>();
            for (int i = 0; i < 100; i++)
            {
                RunParallel(container);
            }
                      
            Assert.AreEqual(1,Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_UnknownDependencyService_DoesNotThrowRecursiveDependencyExceptionOnSecondAttempt()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            try
            {
                container.GetInstance<IFoo>();
            }
            catch (Exception)
            {
                var ex = ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>(), e => !e.InnerException.Message.Contains("Recursive"));
            }
        }

        [TestMethod]
        public void BeginResolutionScope_TwoContainers_ResolutionContextIsScopedPerContainer()
        {
            var firstContainer = CreateContainer();
            var secondContainer = CreateContainer();
            
            using (Scope firstResolutionScope = firstContainer.BeginScope())
            {
                using (Scope secondResolutionScope = secondContainer.BeginScope())
                {
                    Assert.AreNotSame(firstResolutionScope, secondResolutionScope);
                }
            }
        }



        private static void RunParallel(IServiceContainer container)
        {
            Parallel.Invoke(
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>());
        }

        #endregion

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }

        
        
    }
}