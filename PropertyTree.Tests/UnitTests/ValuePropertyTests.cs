using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class ValuePropertyTests
    {
        [Test]
        public void BoolProperty_Constructor_WithoutInitialValue_SetsFalse()
        {
            // Arrange & Act
            var property = new BoolProperty("TestBool");

            // Assert
            Assert.IsFalse(property.Value);
        }

        [Test]
        public void BoolProperty_Constructor_WithInitialValue_SetsValue()
        {
            // Arrange & Act
            var property = new BoolProperty("TestProperty", true);

            // Assert
            Assert.IsTrue(property.Value);
        }

        [Test]
        public void BoolProperty_ValueProperty_CanGetAndSet()
        {
            // Arrange
            var property = new BoolProperty("TestProperty", false);

            // Act
            property.Value = true;

            // Assert
            Assert.IsTrue(property.Value);
        }

        [Test]
        public void FloatProperty_Constructor_WithoutInitialValue_SetsZero()
        {
            // Arrange & Act
            var property = new FloatProperty("TestFloat");

            // Assert
            Assert.AreEqual(0f, property.Value);
        }

        [Test]
        public void FloatProperty_Constructor_WithInitialValue_SetsValue()
        {
            // Arrange & Act
            var property = new FloatProperty("TestProperty", 42.5f);

            // Assert
            Assert.AreEqual(42.5f, property.Value);
        }

        [Test]
        public void FloatProperty_ValueProperty_CanGetAndSet()
        {
            // Arrange
            var property = new FloatProperty("TestProperty", 10.0f);

            // Act
            property.Value = 25.5f;

            // Assert
            Assert.AreEqual(25.5f, property.Value);
        }

        [Test]
        public void IntProperty_Constructor_WithoutInitialValue_SetsZero()
        {
            // Arrange & Act
            var property = new IntProperty("TestInt");

            // Assert
            Assert.AreEqual(0, property.Value);
        }

        [Test]
        public void IntProperty_Constructor_WithInitialValue_SetsValue()
        {
            // Arrange & Act
            var property = new IntProperty("TestProperty", 42);

            // Assert
            Assert.AreEqual(42, property.Value);
        }

        [Test]
        public void IntProperty_ValueProperty_CanGetAndSet()
        {
            // Arrange
            var property = new IntProperty("TestProperty", 10);

            // Act
            property.Value = 25;

            // Assert
            Assert.AreEqual(25, property.Value);
        }

        [Test]
        public void StringProperty_Constructor_WithoutInitialValue_SetsEmptyString()
        {
            // Arrange & Act
            var property = new StringProperty("TestString");

            // Assert
            Assert.AreEqual(string.Empty, property.Value);
        }

        [Test]
        public void StringProperty_Constructor_WithInitialValue_SetsValue()
        {
            // Arrange & Act
            var property = new StringProperty("TestProperty", "InitialValue");

            // Assert
            Assert.AreEqual("InitialValue", property.Value);
        }

        [Test]
        public void StringProperty_ValueProperty_CanGetAndSet()
        {
            // Arrange
            var property = new StringProperty("TestProperty", "InitialValue");

            // Act
            property.Value = "NewValue";

            // Assert
            Assert.AreEqual("NewValue", property.Value);
        }

        [Test]
        public void EnumProperty_Constructor_WithoutInitialValue_SetsDefault()
        {
            // Arrange & Act
            var property = new EnumProperty<TestEnum>("TestEnum");

            // Assert
            Assert.AreEqual(default(TestEnum), property.Value);
        }

        [Test]
        public void EnumProperty_Constructor_WithInitialValue_SetsValue()
        {
            // Arrange & Act
            var property = new EnumProperty<TestEnum>("TestProperty", TestEnum.Value2);

            // Assert
            Assert.AreEqual(TestEnum.Value2, property.Value);
        }

        [Test]
        public void EnumProperty_ValueProperty_CanGetAndSet()
        {
            // Arrange
            var property = new EnumProperty<TestEnum>("TestProperty", TestEnum.Value1);

            // Act
            property.Value = TestEnum.Value3;

            // Assert
            Assert.AreEqual(TestEnum.Value3, property.Value);
        }

        [Test]
        public void EnumProperty_GetAndSet_WorksCorrectly()
        {
            // Arrange
            var property = new EnumProperty<TestEnum>("TestEnum");

            // Act & Assert
            Assert.AreEqual(default(TestEnum), property.Value);
            
            property.Value = TestEnum.Value2;
            Assert.AreEqual(TestEnum.Value2, property.Value);
        }

        [Test]
        public void AllProperties_Constructor_SetsName()
        {
            // Arrange & Act
            var boolProperty = new BoolProperty("BoolTest");
            var floatProperty = new FloatProperty("FloatTest");
            var intProperty = new IntProperty("IntTest");
            var stringProperty = new StringProperty("StringTest");
            var enumProperty = new EnumProperty<TestEnum>("EnumTest");

            // Assert
            Assert.AreEqual("BoolTest", boolProperty.Name);
            Assert.AreEqual("FloatTest", floatProperty.Name);
            Assert.AreEqual("IntTest", intProperty.Name);
            Assert.AreEqual("StringTest", stringProperty.Name);
            Assert.AreEqual("EnumTest", enumProperty.Name);
        }

        [Test]
        public void AllProperties_SetValue_TriggersUpdatedEvent()
        {
            // Arrange
            var boolProperty = new BoolProperty("BoolTest");
            var floatProperty = new FloatProperty("FloatTest");
            var intProperty = new IntProperty("IntTest");
            var stringProperty = new StringProperty("StringTest");
            var enumProperty = new EnumProperty<TestEnum>("EnumTest");

            bool boolUpdated = false, floatUpdated = false, intUpdated = false, stringUpdated = false, enumUpdated = false;
            
            boolProperty.Updated += _ => boolUpdated = true;
            floatProperty.Updated += _ => floatUpdated = true;
            intProperty.Updated += _ => intUpdated = true;
            stringProperty.Updated += _ => stringUpdated = true;
            enumProperty.Updated += _ => enumUpdated = true;

            // Act
            boolProperty.Value = true;
            floatProperty.Value = 1.5f;
            intProperty.Value = 42;
            stringProperty.Value = "Test";
            enumProperty.Value = TestEnum.Value2;

            // Assert
            Assert.IsTrue(boolUpdated);
            Assert.IsTrue(floatUpdated);
            Assert.IsTrue(intUpdated);
            Assert.IsTrue(stringUpdated);
            Assert.IsTrue(enumUpdated);
        }

        private enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}
