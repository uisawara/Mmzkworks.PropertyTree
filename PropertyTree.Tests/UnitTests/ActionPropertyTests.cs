using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class ActionPropertyTests
    {
        [Test]
        public void ActionProperty_Constructor_WithAction_SetsAction()
        {
            // Arrange
            bool actionExecuted = false;
            Action action = () => actionExecuted = true;

            // Act
            var property = new ActionProperty("TestAction", action);
            property.Execute();

            // Assert
            Assert.IsTrue(actionExecuted);
        }

        [Test]
        public void ActionProperty_Constructor_WithoutAction_DoesNotThrow()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => new ActionProperty("TestAction"));
        }

        [Test]
        public void ActionProperty_Execute_WithNullAction_DoesNotThrow()
        {
            // Arrange
            var property = new ActionProperty("TestAction");

            // Act & Assert
            Assert.DoesNotThrow(() => property.Execute());
        }

        [Test]
        public void ActionProperty_Execute_TriggersUpdatedEvent()
        {
            // Arrange
            var property = new ActionProperty("TestAction", () => { });
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Execute();

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void ActionProperty_Execute_WithAction_TriggersUpdatedEvent()
        {
            // Arrange
            bool actionExecuted = false;
            bool eventTriggered = false;
            Action action = () => actionExecuted = true;
            
            var property = new ActionProperty("TestAction", action);
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Execute();

            // Assert
            Assert.IsTrue(actionExecuted);
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void ActionProperty_Execute_WithNullAction_StillTriggersUpdatedEvent()
        {
            // Arrange
            var property = new ActionProperty("TestAction");
            bool eventTriggered = false;
            property.Updated += _ => eventTriggered = true;

            // Act
            property.Execute();

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void ActionProperty_Constructor_SetsName()
        {
            // Arrange
            var propertyName = "TestActionProperty";

            // Act
            var property = new ActionProperty(propertyName);

            // Assert
            Assert.AreEqual(propertyName, property.Name);
        }
    }
} 
