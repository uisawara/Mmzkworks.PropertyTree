using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class EnvironmentHelperTests
    {
        private const string TestEnvVarName1 = "MMZK_TEST_VAR_1";
        private const string TestEnvVarName2 = "MMZK_TEST_VAR_2";
        private const string TestEnvVarName3 = "MMZK_PREFIX_VAR_1";
        private const string TestEnvVarName4 = "MMZK_PREFIX_VAR_2";

        [SetUp]
        public void SetUp()
        {
            // Set up test environment variables
            Environment.SetEnvironmentVariable(TestEnvVarName1, "TestValue1");
            Environment.SetEnvironmentVariable(TestEnvVarName2, "TestValue2");
            Environment.SetEnvironmentVariable(TestEnvVarName3, "PrefixValue1");
            Environment.SetEnvironmentVariable(TestEnvVarName4, "PrefixValue2");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test environment variables
            Environment.SetEnvironmentVariable(TestEnvVarName1, null);
            Environment.SetEnvironmentVariable(TestEnvVarName2, null);
            Environment.SetEnvironmentVariable(TestEnvVarName3, null);
            Environment.SetEnvironmentVariable(TestEnvVarName4, null);
        }

        [Test]
        public void ToPropertyGroup_WithDefaultName_CreatesPropertyGroupWithEnvironmentVariables()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroup();

            // Assert
            Assert.AreEqual("Environment", result.Name);
            Assert.Greater(result.Items.Count, 0);
            
            // Check if test environment variables are included
            var testVar1 = result.Items.FirstOrDefault(p => p.Name == TestEnvVarName1) as StringProperty;
            Assert.IsNotNull(testVar1);
            Assert.AreEqual("TestValue1", testVar1.Value);
        }

        [Test]
        public void ToPropertyGroup_WithCustomName_CreatesPropertyGroupWithCustomName()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroup("CustomEnvironment");

            // Assert
            Assert.AreEqual("CustomEnvironment", result.Name);
            Assert.Greater(result.Items.Count, 0);
        }

        [Test]
        public void ToPropertyGroup_WithSpecificVariableNames_CreatesPropertyGroupWithSpecifiedVariables()
        {
            // Arrange
            var variableNames = new[] { TestEnvVarName1, TestEnvVarName2, "NON_EXISTENT_VAR" };

            // Act
            var result = EnvironmentHelper.ToPropertyGroup(variableNames, "SpecificEnvironment");

            // Assert
            Assert.AreEqual("SpecificEnvironment", result.Name);
            Assert.AreEqual(3, result.Items.Count);

            var testVar1 = result.Items.FirstOrDefault(p => p.Name == TestEnvVarName1) as StringProperty;
            var testVar2 = result.Items.FirstOrDefault(p => p.Name == TestEnvVarName2) as StringProperty;
            var nonExistentVar = result.Items.FirstOrDefault(p => p.Name == "NON_EXISTENT_VAR") as StringProperty;

            Assert.IsNotNull(testVar1);
            Assert.AreEqual("TestValue1", testVar1.Value);
            
            Assert.IsNotNull(testVar2);
            Assert.AreEqual("TestValue2", testVar2.Value);
            
            Assert.IsNotNull(nonExistentVar);
            Assert.AreEqual(string.Empty, nonExistentVar.Value);
        }

        [Test]
        public void ToPropertyGroup_WithEmptyVariableNames_CreatesEmptyPropertyGroup()
        {
            // Arrange
            var variableNames = new string[0];

            // Act
            var result = EnvironmentHelper.ToPropertyGroup(variableNames);

            // Assert
            Assert.AreEqual("Environment", result.Name);
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroupByPrefix_WithExistingPrefix_CreatesPropertyGroupWithFilteredVariables()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroupByPrefix("MMZK_PREFIX_", "PrefixEnvironment");

            // Assert
            Assert.AreEqual("PrefixEnvironment", result.Name);
            Assert.AreEqual(2, result.Items.Count);

            var prefixVar1 = result.Items.FirstOrDefault(p => p.Name == TestEnvVarName3) as StringProperty;
            var prefixVar2 = result.Items.FirstOrDefault(p => p.Name == TestEnvVarName4) as StringProperty;

            Assert.IsNotNull(prefixVar1);
            Assert.AreEqual("PrefixValue1", prefixVar1.Value);
            
            Assert.IsNotNull(prefixVar2);
            Assert.AreEqual("PrefixValue2", prefixVar2.Value);
        }

        [Test]
        public void ToPropertyGroupByPrefix_WithRemovePrefixTrue_CreatesPropertyGroupWithPrefixRemoved()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroupByPrefix("MMZK_PREFIX_", "PrefixEnvironment", removePrefix: true);

            // Assert
            Assert.AreEqual("PrefixEnvironment", result.Name);
            Assert.AreEqual(2, result.Items.Count);

            var prefixVar1 = result.Items.FirstOrDefault(p => p.Name == "VAR_1") as StringProperty;
            var prefixVar2 = result.Items.FirstOrDefault(p => p.Name == "VAR_2") as StringProperty;

            Assert.IsNotNull(prefixVar1);
            Assert.AreEqual("PrefixValue1", prefixVar1.Value);
            
            Assert.IsNotNull(prefixVar2);
            Assert.AreEqual("PrefixValue2", prefixVar2.Value);
        }

        [Test]
        public void ToPropertyGroupByPrefix_WithNonExistentPrefix_CreatesEmptyPropertyGroup()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroupByPrefix("NON_EXISTENT_PREFIX_");

            // Assert
            Assert.AreEqual("Environment", result.Name);
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroupByPrefix_WithEmptyPrefix_CreatesPropertyGroupWithAllVariables()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroupByPrefix("");

            // Assert
            Assert.AreEqual("Environment", result.Name);
            Assert.Greater(result.Items.Count, 0);
        }

        [Test]
        public void ToPropertyGroupByPrefix_CaseInsensitive_FindsVariablesRegardlessOfCase()
        {
            // Act
            var result = EnvironmentHelper.ToPropertyGroupByPrefix("mmzk_prefix_");

            // Assert
            Assert.AreEqual(2, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroup_AllVariantsReturnStringProperties()
        {
            // Arrange
            var variableNames = new[] { TestEnvVarName1 };

            // Act
            var result1 = EnvironmentHelper.ToPropertyGroup();
            var result2 = EnvironmentHelper.ToPropertyGroup(variableNames);
            var result3 = EnvironmentHelper.ToPropertyGroupByPrefix("MMZK_TEST_");

            // Assert
            Assert.IsTrue(result1.Items.All(p => p is StringProperty));
            Assert.IsTrue(result2.Items.All(p => p is StringProperty));
            Assert.IsTrue(result3.Items.All(p => p is StringProperty));
        }
    }
} 