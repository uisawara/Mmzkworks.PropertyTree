using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class PropertyAdapterTests
    {
        [Test]
        public void BoolPropertyAdapter_Constructor_SetsNameAndDelegates()
        {
            // Arrange
            var currentValue = false;
            Func<bool> getter = () => currentValue;
            Action<bool> setter = value => currentValue = value;

            // Act
            var adapter = new BoolPropertyAdapter("TestBool", getter, setter);

            // Assert
            Assert.AreEqual("TestBool", adapter.Name);
        }

        [Test]
        public void BoolPropertyAdapter_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var currentValue = false;
            Func<bool> getter = () => currentValue;
            Action<bool> setter = value => currentValue = value;

            var adapter = new BoolPropertyAdapter("TestBool", getter, setter);

            // Act & Assert
            Assert.IsFalse(adapter.Value);
            
            adapter.Value = true;
            Assert.IsTrue(adapter.Value);
            
            adapter.Value = false;
            Assert.IsFalse(adapter.Value);
        }

        [Test]
        public void IntPropertyAdapter_Constructor_SetsMinMaxAndDelegates()
        {
            // Arrange
            var min = 0;
            var max = 100;
            var currentValue = 50;
            Func<int> getter = () => currentValue;
            Action<int> setter = value => currentValue = value;

            // Act
            var adapter = new IntPropertyAdapter("TestInt", min, max, getter, setter);

            // Assert
            Assert.AreEqual(min, adapter.Min);
            Assert.AreEqual(max, adapter.Max);
            Assert.AreEqual("TestInt", adapter.Name);
        }

        [Test]
        public void IntPropertyAdapter_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var currentValue = 0;
            Func<int> getter = () => currentValue;
            Action<int> setter = value => currentValue = value;

            var adapter = new IntPropertyAdapter("TestInt", 0, 100, getter, setter);

            // Act & Assert
            Assert.AreEqual(0, adapter.Value);
            
            adapter.Value = 42;
            Assert.AreEqual(42, adapter.Value);
            
            adapter.Value = 100;
            Assert.AreEqual(100, adapter.Value);
        }

        [Test]
        public void FloatPropertyAdapter_Constructor_SetsMinMaxAndDelegates()
        {
            // Arrange
            var min = 0f;
            var max = 1f;
            var currentValue = 0.5f;
            Func<float> getter = () => currentValue;
            Action<float> setter = value => currentValue = value;

            // Act
            var adapter = new FloatPropertyAdapter("TestFloat", min, max, getter, setter);

            // Assert
            Assert.AreEqual(min, adapter.Min);
            Assert.AreEqual(max, adapter.Max);
            Assert.AreEqual("TestFloat", adapter.Name);
        }

        [Test]
        public void FloatPropertyAdapter_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var currentValue = 0f;
            Func<float> getter = () => currentValue;
            Action<float> setter = value => currentValue = value;

            var adapter = new FloatPropertyAdapter("TestFloat", 0f, 1f, getter, setter);

            // Act & Assert
            Assert.AreEqual(0f, adapter.Value);
            
            adapter.Value = 0.5f;
            Assert.AreEqual(0.5f, adapter.Value);
            
            adapter.Value = 1f;
            Assert.AreEqual(1f, adapter.Value);
        }

        [Test]
        public void StringPropertyAdapter_Constructor_SetsNameAndDelegates()
        {
            // Arrange
            var currentValue = "";
            Func<string> getter = () => currentValue;
            Action<string> setter = value => currentValue = value;

            // Act
            var adapter = new StringPropertyAdapter("TestString", getter, setter);

            // Assert
            Assert.AreEqual("TestString", adapter.Name);
        }

        [Test]
        public void StringPropertyAdapter_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var currentValue = "";
            Func<string> getter = () => currentValue;
            Action<string> setter = value => currentValue = value;

            var adapter = new StringPropertyAdapter("TestString", getter, setter);

            // Act & Assert
            Assert.AreEqual("", adapter.Value);
            
            adapter.Value = "Hello";
            Assert.AreEqual("Hello", adapter.Value);
            
            adapter.Value = "World";
            Assert.AreEqual("World", adapter.Value);
        }

        [Test]
        public void EnumPropertyAdapter_Constructor_SetsNameAndDelegates()
        {
            // Arrange
            var currentValue = TestEnum.Value1;
            Func<TestEnum> getter = () => currentValue;
            Action<TestEnum> setter = value => currentValue = value;

            // Act
            var adapter = new EnumPropertyAdapter<TestEnum>("TestEnum", getter, setter);

            // Assert
            Assert.AreEqual("TestEnum", adapter.Name);
        }

        [Test]
        public void EnumPropertyAdapter_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var currentValue = TestEnum.Value1;
            Func<TestEnum> getter = () => currentValue;
            Action<TestEnum> setter = value => currentValue = value;

            var adapter = new EnumPropertyAdapter<TestEnum>("TestEnum", getter, setter);

            // Act & Assert
            Assert.AreEqual(TestEnum.Value1, adapter.Value);
            
            adapter.Value = TestEnum.Value2;
            Assert.AreEqual(TestEnum.Value2, adapter.Value);
            
            adapter.Value = TestEnum.Value3;
            Assert.AreEqual(TestEnum.Value3, adapter.Value);
        }

        [Test]
        public void AllPropertyAdapters_TriggerUpdatedEvent()
        {
            // Arrange
            var boolValue = false;
            var boolAdapter = new BoolPropertyAdapter("TestBool", () => boolValue, value => boolValue = value);
            bool boolEventTriggered = false;
            boolAdapter.Updated += _ => boolEventTriggered = true;

            var intValue = 0;
            var intAdapter = new IntPropertyAdapter("TestInt", 0, 100, () => intValue, value => intValue = value);
            bool intEventTriggered = false;
            intAdapter.Updated += _ => intEventTriggered = true;

            var floatValue = 0f;
            var floatAdapter = new FloatPropertyAdapter("TestFloat", 0f, 1f, () => floatValue, value => floatValue = value);
            bool floatEventTriggered = false;
            floatAdapter.Updated += _ => floatEventTriggered = true;

            var stringValue = "";
            var stringAdapter = new StringPropertyAdapter("TestString", () => stringValue, value => stringValue = value);
            bool stringEventTriggered = false;
            stringAdapter.Updated += _ => stringEventTriggered = true;

            var enumValue = TestEnum.Value1;
            var enumAdapter = new EnumPropertyAdapter<TestEnum>("TestEnum", () => enumValue, value => enumValue = value);
            bool enumEventTriggered = false;
            enumAdapter.Updated += _ => enumEventTriggered = true;

            // Act
            boolAdapter.Value = true;
            intAdapter.Value = 42;
            floatAdapter.Value = 0.5f;
            stringAdapter.Value = "Test";
            enumAdapter.Value = TestEnum.Value2;

            // Assert
            Assert.IsTrue(boolEventTriggered);
            Assert.IsTrue(intEventTriggered);
            Assert.IsTrue(floatEventTriggered);
            Assert.IsTrue(stringEventTriggered);
            Assert.IsTrue(enumEventTriggered);
        }

        private enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
} 
