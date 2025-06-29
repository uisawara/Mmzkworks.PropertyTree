using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class ValuePropertyAdapterTests
    {
        [Test]
        public void ValuePropertyAdapter_Constructor_SetsMinMaxAndDelegates()
        {
            // Arrange
            var min = 0;
            var max = 100;
            var currentValue = 50;
            Func<int> getter = () => currentValue;
            Action<int> setter = value => currentValue = value;

            // Act
            var adapter = new ValuePropertyAdapter<int>("TestAdapter", min, max, getter, setter);

            // Assert
            Assert.AreEqual(min, adapter.Min);
            Assert.AreEqual(max, adapter.Max);
            Assert.AreEqual("TestAdapter", adapter.Name);
        }

        [Test]
        public void ValuePropertyAdapter_Get_CallsGetter()
        {
            // Arrange
            var expectedValue = 42;
            Func<int> getter = () => expectedValue;
            Action<int> setter = value => { };

            var adapter = new ValuePropertyAdapter<int>("TestAdapter", 0, 100, getter, setter);

            // Act
            var result = adapter.Value;

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void ValuePropertyAdapter_Set_CallsSetter()
        {
            // Arrange
            var setValue = 0;
            Func<int> getter = () => setValue;
            Action<int> setter = value => setValue = value;

            var adapter = new ValuePropertyAdapter<int>("TestAdapter", 0, 100, getter, setter);

            // Act
            adapter.Value = 75;

            // Assert
            Assert.AreEqual(75, setValue);
        }

        [Test]
        public void ValuePropertyAdapter_Set_TriggersUpdatedEvent()
        {
            // Arrange
            var setValue = 0;
            Func<int> getter = () => setValue;
            Action<int> setter = value => setValue = value;

            var adapter = new ValuePropertyAdapter<int>("TestAdapter", 0, 100, getter, setter);
            bool eventTriggered = false;
            adapter.Updated += _ => eventTriggered = true;

            // Act
            adapter.Value = 50;

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void ValuePropertyAdapter_GetAndSet_WorksWithString()
        {
            // Arrange
            var currentValue = "Initial";
            Func<string> getter = () => currentValue;
            Action<string> setter = value => currentValue = value;

            var adapter = new ValuePropertyAdapter<string>("TestAdapter", "A", "Z", getter, setter);

            // Act & Assert
            Assert.AreEqual("Initial", adapter.Value);
            
            adapter.Value = "Updated";
            Assert.AreEqual("Updated", adapter.Value);
        }

        [Test]
        public void ValuePropertyAdapter_GetAndSet_WorksWithFloat()
        {
            // Arrange
            var currentValue = 0.5f;
            Func<float> getter = () => currentValue;
            Action<float> setter = value => currentValue = value;

            var adapter = new ValuePropertyAdapter<float>("TestAdapter", 0f, 1f, getter, setter);

            // Act & Assert
            Assert.AreEqual(0.5f, adapter.Value);
            
            adapter.Value = 0.75f;
            Assert.AreEqual(0.75f, adapter.Value);
        }

        [Test]
        public void ValuePropertyAdapter_ImplementsIHasValueRange()
        {
            // Arrange
            Func<int> getter = () => 0;
            Action<int> setter = value => { };

            // Act
            var adapter = new ValuePropertyAdapter<int>("TestAdapter", 0, 100, getter, setter);

            // Assert
            Assert.IsTrue(adapter is works.mmzk.PropertyTree.IHasValueRange<int>);
            var range = adapter as works.mmzk.PropertyTree.IHasValueRange<int>;
            Assert.AreEqual(0, range.Min);
            Assert.AreEqual(100, range.Max);
        }
    }
} 
