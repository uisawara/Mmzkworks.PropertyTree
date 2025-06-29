using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class PropertyGroupTests
    {
        [Test]
        public void PropertyGroup_Constructor_WithName_CreatesEmptyGroup()
        {
            // Arrange & Act
            var group = new PropertyGroup("TestGroup");

            // Assert
            Assert.AreEqual("TestGroup", group.Name);
            Assert.AreEqual(0, group.Items.Count);
        }

        [Test]
        public void PropertyGroup_Constructor_WithItems_AddsAllItems()
        {
            // Arrange
            var items = new List<IProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };

            // Act
            var group = new PropertyGroup("TestGroup", items);

            // Assert
            Assert.AreEqual(2, group.Items.Count);
            Assert.AreEqual("Item1", group.Items[0].Name);
            Assert.AreEqual("Item2", group.Items[1].Name);
        }

        [Test]
        public void PropertyGroup_Constructor_WithItems_TriggersAddedEvents()
        {
            // Arrange
            var items = new List<IProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };
            var addedItems = new List<IProperty>();

            // Act
            var group = new PropertyGroup("TestGroup");
            group.Added += item => addedItems.Add(item);
            group.AddRange(items);

            // Assert
            Assert.AreEqual(2, addedItems.Count);
            Assert.Contains(items[0], addedItems);
            Assert.Contains(items[1], addedItems);
        }

        [Test]
        public void PropertyGroup_Add_SetsParentAndAddsToItems()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");

            // Act
            group.Add(property);

            // Assert
            Assert.AreEqual(1, group.Items.Count);
            Assert.AreEqual(property, group.Items[0]);
            Assert.AreEqual(group, property.Parent);
        }

        [Test]
        public void PropertyGroup_Add_TriggersAddedEvent()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            IProperty addedProperty = null;
            group.Added += item => addedProperty = item;

            // Act
            group.Add(property);

            // Assert
            Assert.AreEqual(property, addedProperty);
        }

        [Test]
        public void PropertyGroup_AddRange_AddsAllItemsAndSetsParents()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var items = new List<works.mmzk.PropertyTree.BaseProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };

            // Act
            group.AddRange(items);

            // Assert
            Assert.AreEqual(2, group.Items.Count);
            foreach (var item in items)
            {
                Assert.AreEqual(group, item.Parent);
            }
        }

        [Test]
        public void PropertyGroup_AddRange_TriggersAddedEventsForAllItems()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var items = new List<works.mmzk.PropertyTree.BaseProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };
            var addedItems = new List<IProperty>();
            group.Added += item => addedItems.Add(item);

            // Act
            group.AddRange(items);

            // Assert
            Assert.AreEqual(2, addedItems.Count);
            Assert.Contains(items[0], addedItems);
            Assert.Contains(items[1], addedItems);
        }

        [Test]
        public void PropertyGroup_Remove_RemovesItemAndClearsParent()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            group.Add(property);

            // Act
            group.Remove(property);

            // Assert
            Assert.AreEqual(0, group.Items.Count);
            Assert.IsNull(property.Parent);
        }

        [Test]
        public void PropertyGroup_Remove_TriggersRemovedEvent()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            group.Add(property);
            
            IProperty removedProperty = null;
            group.Removed += item => removedProperty = item;

            // Act
            group.Remove(property);

            // Assert
            Assert.AreEqual(property, removedProperty);
        }

        [Test]
        public void PropertyGroup_ClearAll_RemovesAllItemsAndClearsParents()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var items = new List<works.mmzk.PropertyTree.BaseProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };
            group.AddRange(items);

            // Act
            group.ClearAll();

            // Assert
            Assert.AreEqual(0, group.Items.Count);
            foreach (var item in items)
            {
                Assert.IsNull(item.Parent);
            }
        }

        [Test]
        public void PropertyGroup_ClearAll_TriggersRemovedEventsForAllItems()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var items = new List<works.mmzk.PropertyTree.BaseProperty>
            {
                new TestBaseProperty("Item1"),
                new TestBaseProperty("Item2")
            };
            group.AddRange(items);
            
            var removedItems = new List<IProperty>();
            group.Removed += item => removedItems.Add(item);

            // Act
            group.ClearAll();

            // Assert
            Assert.AreEqual(2, removedItems.Count);
            Assert.Contains(items[0], removedItems);
            Assert.Contains(items[1], removedItems);
        }

        [Test]
        public void PropertyGroup_At_ReturnsCorrectItem()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            group.Add(property);

            // Act
            var result = group.At(0);

            // Assert
            Assert.AreEqual(property, result);
        }

        [Test]
        public void PropertyGroup_AtGeneric_ReturnsCorrectTypedItem()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            group.Add(property);

            // Act
            var result = group.At<TestBaseProperty>(0);

            // Assert
            Assert.AreEqual(property, result);
        }

        [Test]
        public void PropertyGroup_At_ThrowsException_WhenIndexOutOfRange()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => group.At(0));
        }

        [Test]
        public void PropertyGroup_Items_ReturnsReadOnlyCollection()
        {
            // Arrange
            var group = new PropertyGroup("TestGroup");
            var property = new TestBaseProperty("TestProperty");
            group.Add(property);

            // Act
            var items = group.Items;

            // Assert
            Assert.IsTrue(items is System.Collections.ObjectModel.ReadOnlyCollection<IProperty>);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(property, items[0]);
        }

        private class TestBaseProperty : works.mmzk.PropertyTree.BaseProperty
        {
            public TestBaseProperty(string name) : base(name)
            {
            }
        }
    }
} 
