using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class BasePropertyTests
    {
        [Test]
        public void BaseProperty_Constructor_SetsNameCorrectly()
        {
            // Arrange & Act
            var property = new TestBaseProperty("TestProperty");

            // Assert
            Assert.AreEqual("TestProperty", property.Name);
        }

        [Test]
        public void BaseProperty_Parent_CanBeSetAndRetrieved()
        {
            // Arrange
            var property = new TestBaseProperty("TestProperty");
            var parent = new PropertyGroup("ParentGroup");

            // Act
            property.Parent = parent;

            // Assert
            Assert.AreEqual(parent, property.Parent);
        }

        private class TestBaseProperty : works.mmzk.PropertyTree.BaseProperty
        {
            public TestBaseProperty(string name) : base(name)
            {
            }
        }
    }
} 
