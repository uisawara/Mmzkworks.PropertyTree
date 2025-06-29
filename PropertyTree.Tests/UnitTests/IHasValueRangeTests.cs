using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class IHasValueRangeTests
    {
        [Test]
        public void IHasValueRange_Implementation_HasMinAndMaxProperties()
        {
            // Arrange
            var range = new TestValueRange(0, 100);

            // Act & Assert
            Assert.AreEqual(0, range.Min);
            Assert.AreEqual(100, range.Max);
        }

        [Test]
        public void IHasValueRange_WithFloat_WorksCorrectly()
        {
            // Arrange
            var range = new TestFloatRange(0f, 1f);

            // Act & Assert
            Assert.AreEqual(0f, range.Min);
            Assert.AreEqual(1f, range.Max);
        }

        [Test]
        public void IHasValueRange_WithString_WorksCorrectly()
        {
            // Arrange
            var range = new TestStringRange("A", "Z");

            // Act & Assert
            Assert.AreEqual("A", range.Min);
            Assert.AreEqual("Z", range.Max);
        }

        [Test]
        public void IHasValueRange_CanBeUsedAsInterface()
        {
            // Arrange
            works.mmzk.PropertyTree.IHasValueRange<int> range = new TestValueRange(10, 20);

            // Act & Assert
            Assert.AreEqual(10, range.Min);
            Assert.AreEqual(20, range.Max);
        }

        [Test]
        public void IHasValueRange_GenericConstraint_WorksWithDifferentTypes()
        {
            // Arrange
            var intRange = new TestValueRange(0, 100);
            var floatRange = new TestFloatRange(0f, 1f);
            var stringRange = new TestStringRange("A", "Z");

            // Act & Assert
            Assert.IsTrue(intRange is works.mmzk.PropertyTree.IHasValueRange<int>);
            Assert.IsTrue(floatRange is works.mmzk.PropertyTree.IHasValueRange<float>);
            Assert.IsTrue(stringRange is works.mmzk.PropertyTree.IHasValueRange<string>);
        }

        [Test]
        public void FloatProperty_WithValueRange_ClampsValuesCorrectly()
        {
            // Arrange
            var property = new FloatProperty("TestProperty", 50f, 0f, 100f);

            // Act & Assert
            Assert.AreEqual(0f, property.Min);
            Assert.AreEqual(100f, property.Max);
            Assert.AreEqual(50f, property.Value);

            // Valid value within range
            property.Value = 75f;
            Assert.AreEqual(75f, property.Value);

            // Value below minimum
            property.Value = -10f;
            Assert.AreEqual(0f, property.Value);

            // Value above maximum
            property.Value = 150f;
            Assert.AreEqual(100f, property.Value);
        }

        [Test]
        public void FloatProperty_Constructor_WithRange_ClampsInitialValue()
        {
            // Arrange & Act
            var property = new FloatProperty("TestProperty", 150f, 0f, 100f);

            // Assert
            Assert.AreEqual(100f, property.Value);
        }

        [Test]
        public void IntProperty_WithValueRange_ClampsValuesCorrectly()
        {
            // Arrange
            var property = new IntProperty("TestProperty", 5, 0, 10);

            // Act & Assert
            Assert.AreEqual(0, property.Min);
            Assert.AreEqual(10, property.Max);
            Assert.AreEqual(5, property.Value);

            // Valid value within range
            property.Value = 7;
            Assert.AreEqual(7, property.Value);

            // Value below minimum
            property.Value = -5;
            Assert.AreEqual(0, property.Value);

            // Value above maximum
            property.Value = 15;
            Assert.AreEqual(10, property.Value);
        }

        [Test]
        public void IntProperty_Constructor_WithRange_ClampsInitialValue()
        {
            // Arrange & Act
            var property = new IntProperty("TestProperty", 15, 0, 10);

            // Assert
            Assert.AreEqual(10, property.Value);
        }

        [Test]
        public void FloatProperty_WithoutValueRange_AllowsAnyValue()
        {
            // Arrange
            var property = new FloatProperty("TestProperty", 0f);

            // Act & Assert
            Assert.AreEqual(float.MinValue, property.Min);
            Assert.AreEqual(float.MaxValue, property.Max);

            property.Value = float.MinValue;
            Assert.AreEqual(float.MinValue, property.Value);

            property.Value = float.MaxValue;
            Assert.AreEqual(float.MaxValue, property.Value);
        }

        [Test]
        public void IntProperty_WithoutValueRange_AllowsAnyValue()
        {
            // Arrange
            var property = new IntProperty("TestProperty", 0);

            // Act & Assert
            Assert.AreEqual(int.MinValue, property.Min);
            Assert.AreEqual(int.MaxValue, property.Max);

            property.Value = int.MinValue;
            Assert.AreEqual(int.MinValue, property.Value);

            property.Value = int.MaxValue;
            Assert.AreEqual(int.MaxValue, property.Value);
        }

        [Test]
        public void FloatProperty_SetMethod_RespectsValueRange()
        {
            // Arrange
            var property = new FloatProperty("TestProperty", 50f, 0f, 100f);

            // Act & Assert
            property.Value = 75f;
            Assert.AreEqual(75f, property.Value);

            property.Value = -10f;
            Assert.AreEqual(0f, property.Value);

            property.Value = 150f;
            Assert.AreEqual(100f, property.Value);
        }

        [Test]
        public void IntProperty_SetMethod_RespectsValueRange()
        {
            // Arrange
            var property = new IntProperty("TestProperty", 5, 0, 10);

            // Act & Assert
            property.Value = 7;
            Assert.AreEqual(7, property.Value);

            property.Value = -5;
            Assert.AreEqual(0, property.Value);

            property.Value = 15;
            Assert.AreEqual(10, property.Value);
        }

        [Test]
        public void FloatProperty_ValueRange_EdgeCases()
        {
            // Arrange
            var property = new FloatProperty("TestProperty", 0f, 0f, 0f);

            // Act & Assert
            Assert.AreEqual(0f, property.Min);
            Assert.AreEqual(0f, property.Max);
            Assert.AreEqual(0f, property.Value);

            property.Value = 1f;
            Assert.AreEqual(0f, property.Value);

            property.Value = -1f;
            Assert.AreEqual(0f, property.Value);
        }

        [Test]
        public void IntProperty_ValueRange_EdgeCases()
        {
            // Arrange
            var property = new IntProperty("TestProperty", 5, 5, 5);

            // Act & Assert
            Assert.AreEqual(5, property.Min);
            Assert.AreEqual(5, property.Max);
            Assert.AreEqual(5, property.Value);

            property.Value = 6;
            Assert.AreEqual(5, property.Value);

            property.Value = 4;
            Assert.AreEqual(5, property.Value);
        }

        // Test implementations
        private class TestValueRange : works.mmzk.PropertyTree.IHasValueRange<int>
        {
            public int Min { get; }
            public int Max { get; }

            public TestValueRange(int min, int max)
            {
                Min = min;
                Max = max;
            }
        }

        private class TestFloatRange : works.mmzk.PropertyTree.IHasValueRange<float>
        {
            public float Min { get; }
            public float Max { get; }

            public TestFloatRange(float min, float max)
            {
                Min = min;
                Max = max;
            }
        }

        private class TestStringRange : works.mmzk.PropertyTree.IHasValueRange<string>
        {
            public string Min { get; }
            public string Max { get; }

            public TestStringRange(string min, string max)
            {
                Min = min;
                Max = max;
            }
        }
    }
} 
