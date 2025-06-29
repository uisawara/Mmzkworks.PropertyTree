using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class BaseValuePropertyTests
    {
        [Test]
        public void BaseValueProperty_Set_TriggersUpdatedEvent()
        {
            // Arrange
            var property = new TestValueProperty("TestProperty");
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Value = "TestValue";

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void BaseValueProperty_SetSameValue_DoesNotTriggerUpdatedEvent()
        {
            // Arrange
            var property = new TestValueProperty("TestProperty");
            property.Value = "TestValue";
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Value = "TestValue";

            // Assert
            Assert.IsFalse(eventTriggered);
        }

        [Test]
        public void BaseValueProperty_SetDifferentValue_TriggersUpdatedEvent()
        {
            // Arrange
            var property = new TestValueProperty("TestProperty");
            property.Value = "TestValue1";
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Value = "TestValue2";

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void BaseValueProperty_InitialSet_TriggersUpdatedEvent()
        {
            // Arrange
            var property = new TestValueProperty("TestProperty");
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Value = "InitialValue";

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void BaseValueProperty_Get_ReturnsSetValue()
        {
            // Arrange
            var property = new TestValueProperty("TestProperty");
            var expectedValue = "TestValue";

            // Act
            property.Value = expectedValue;
            var result = property.Value;

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        private class TestValueProperty : works.mmzk.PropertyTree.BaseValueProperty<string>
        {
            public TestValueProperty(string name) : base(name)
            {
            }
        }
    }
} 
