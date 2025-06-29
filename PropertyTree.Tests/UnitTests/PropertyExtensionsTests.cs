using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class PropertyExtensionsTests
    {
        [Test]
        public void GetFullPath_SimpleProperty_ReturnsPropertyName()
        {
            // Arrange
            var property = new TestBaseProperty("TestProperty");

            // Act
            var path = property.GetFullPath();

            // Assert
            Assert.AreEqual("TestProperty", path);
        }

        [Test]
        public void GetFullPath_NestedProperty_ReturnsFullPath()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property = new TestBaseProperty("TestProperty");

            rootGroup.Add(subGroup);
            subGroup.Add(property);

            // Act
            var path = property.GetFullPath();

            // Assert
            Assert.AreEqual("RootGroup.SubGroup.TestProperty", path);
        }

        [Test]
        public void GetFullPath_NullProperty_ReturnsEmptyString()
        {
            // Arrange
            IProperty property = null;

            // Act
            var path = property.GetFullPath();

            // Assert
            Assert.AreEqual(string.Empty, path);
        }

        [Test]
        public void GetDepth_RootProperty_ReturnsZero()
        {
            // Arrange
            var property = new TestBaseProperty("TestProperty");

            // Act
            var depth = property.GetDepth();

            // Assert
            Assert.AreEqual(0, depth);
        }

        [Test]
        public void GetDepth_NestedProperty_ReturnsCorrectDepth()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property = new TestBaseProperty("TestProperty");

            rootGroup.Add(subGroup);
            subGroup.Add(property);

            // Act
            var depth = property.GetDepth();

            // Assert
            Assert.AreEqual(2, depth);
        }

        [Test]
        public void GetDepth_NullProperty_ReturnsNegativeOne()
        {
            // Arrange
            IProperty property = null;

            // Act
            var depth = property.GetDepth();

            // Assert
            Assert.AreEqual(-1, depth);
        }

        [Test]
        public void IsAtLevel_PropertyAtCorrectLevel_ReturnsTrue()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var property = new TestBaseProperty("TestProperty");
            rootGroup.Add(property);

            // Act
            var isAtLevel = property.IsAtLevel(1);

            // Assert
            Assert.IsTrue(isAtLevel);
        }

        [Test]
        public void IsAtLevel_PropertyAtWrongLevel_ReturnsFalse()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var property = new TestBaseProperty("TestProperty");
            rootGroup.Add(property);

            // Act
            var isAtLevel = property.IsAtLevel(0);

            // Assert
            Assert.IsFalse(isAtLevel);
        }

        [Test]
        public void GetParentChain_RootProperty_ReturnsEmptyArray()
        {
            // Arrange
            var property = new TestBaseProperty("TestProperty");

            // Act
            var parentChain = property.GetParentChain();

            // Assert
            Assert.AreEqual(0, parentChain.Length);
        }

        [Test]
        public void GetParentChain_NestedProperty_ReturnsCorrectChain()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property = new TestBaseProperty("TestProperty");

            rootGroup.Add(subGroup);
            subGroup.Add(property);

            // Act
            var parentChain = property.GetParentChain();

            // Assert
            Assert.AreEqual(2, parentChain.Length);
            Assert.AreEqual(rootGroup, parentChain[0]);
            Assert.AreEqual(subGroup, parentChain[1]);
        }

        [Test]
        public void FindByPath_ExistingProperty_ReturnsProperty()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property = new TestBaseProperty("TestProperty");

            rootGroup.Add(subGroup);
            subGroup.Add(property);

            // Act
            var result = rootGroup.FindByPath("SubGroup.TestProperty");

            // Assert
            Assert.AreEqual(property, result);
        }

        [Test]
        public void FindByPath_NonExistentProperty_ReturnsNull()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");

            // Act
            var result = rootGroup.FindByPath("NonExistent");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void FindByPath_EmptyPath_ReturnsNull()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");

            // Act
            var result = rootGroup.FindByPath("");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetByPath_ExistingProperty_ReturnsProperty()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var property = new TestBaseProperty("TestProperty");
            rootGroup.Add(property);

            // Act
            var result = rootGroup.GetByPath("TestProperty");

            // Assert
            Assert.AreEqual(property, result);
        }

        [Test]
        public void GetByPath_NonExistentProperty_ThrowsArgumentException()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => rootGroup.GetByPath("NonExistent"));
        }

        [Test]
        public void FindByPattern_WildcardPattern_ReturnsMatchingProperties()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property1 = new TestBaseProperty("TestProperty1");
            var property2 = new TestBaseProperty("TestProperty2");

            rootGroup.Add(subGroup);
            subGroup.Add(property1);
            subGroup.Add(property2);

            // Act
            var results = rootGroup.FindByPattern("SubGroup.*");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.Contains(property1, results);
            Assert.Contains(property2, results);
        }

        [Test]
        public void FindByPrefix_ExistingPrefix_ReturnsMatchingProperties()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup = new PropertyGroup("SubGroup");
            var property = new TestBaseProperty("TestProperty");

            rootGroup.Add(subGroup);
            subGroup.Add(property);

            // Act
            var results = rootGroup.FindByPrefix("RootGroup.SubGroup");

            // Assert
            Assert.AreEqual(2, results.Count); // Both SubGroup and TestProperty start with "RootGroup.SubGroup"
            Assert.Contains(subGroup, results);
            Assert.Contains(property, results);
        }

        [Test]
        public void GetAllProperties_ComplexStructure_ReturnsAllProperties()
        {
            // Arrange
            var rootGroup = new PropertyGroup("RootGroup");
            var subGroup1 = new PropertyGroup("SubGroup1");
            var subGroup2 = new PropertyGroup("SubGroup2");
            var property1 = new TestBaseProperty("Property1");
            var property2 = new TestBaseProperty("Property2");

            rootGroup.Add(subGroup1);
            rootGroup.Add(subGroup2);
            subGroup1.Add(property1);
            subGroup2.Add(property2);

            // Act
            var results = rootGroup.GetAllProperties();

            // Assert
            Assert.AreEqual(4, results.Count); // 2 groups + 2 properties
            Assert.Contains(subGroup1, results);
            Assert.Contains(subGroup2, results);
            Assert.Contains(property1, results);
            Assert.Contains(property2, results);
        }

        private class TestBaseProperty : works.mmzk.PropertyTree.BaseProperty
        {
            public TestBaseProperty(string name) : base(name)
            {
            }
        }
    }
}
