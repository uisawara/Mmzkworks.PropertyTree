using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.UnitTests
{
    [TestFixture]
    public class CommandLineArgsHelperTests
    {
        [Test]
        public void ToPropertyGroup_WithNullArgs_ReturnsEmptyPropertyGroup()
        {
            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup((string[])null);

            // Assert
            Assert.AreEqual("CommandLineArgs", result.Name);
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroup_WithEmptyArgs_ReturnsEmptyPropertyGroup()
        {
            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(new string[0]);

            // Assert
            Assert.AreEqual("CommandLineArgs", result.Name);
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroup_WithCustomGroupName_CreatesPropertyGroupWithCustomName()
        {
            // Arrange
            var args = new[] { "--test=value" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args, "CustomArgs");

            // Assert
            Assert.AreEqual("CustomArgs", result.Name);
        }

        [Test]
        public void ToPropertyGroup_WithDoubleHyphenKeyValueFormat_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "--config=production", "--verbose=true", "--port=8080" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(3, result.Items.Count);

            var configProp = result.Items.FirstOrDefault(p => p.Name == "config") as StringProperty;
            var verboseProp = result.Items.FirstOrDefault(p => p.Name == "verbose") as StringProperty;
            var portProp = result.Items.FirstOrDefault(p => p.Name == "port") as StringProperty;

            Assert.IsNotNull(configProp);
            Assert.AreEqual("production", configProp.Value);

            Assert.IsNotNull(verboseProp);
            Assert.AreEqual("true", verboseProp.Value);

            Assert.IsNotNull(portProp);
            Assert.AreEqual("8080", portProp.Value);
        }

        [Test]
        public void ToPropertyGroup_WithSlashColonFormat_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "/config:debug", "/output:file.txt" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(2, result.Items.Count);

            var configProp = result.Items.FirstOrDefault(p => p.Name == "config") as StringProperty;
            var outputProp = result.Items.FirstOrDefault(p => p.Name == "output") as StringProperty;

            Assert.IsNotNull(configProp);
            Assert.AreEqual("debug", configProp.Value);

            Assert.IsNotNull(outputProp);
            Assert.AreEqual("file.txt", outputProp.Value);
        }

        [Test]
        public void ToPropertyGroup_WithSingleHyphenKeyValueFormat_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "-v", "verbose", "-f", "filename.txt" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(2, result.Items.Count);

            var vProp = result.Items.FirstOrDefault(p => p.Name == "v") as StringProperty;
            var fProp = result.Items.FirstOrDefault(p => p.Name == "f") as StringProperty;

            Assert.IsNotNull(vProp);
            Assert.AreEqual("verbose", vProp.Value);

            Assert.IsNotNull(fProp);
            Assert.AreEqual("filename.txt", fProp.Value);
        }

        [Test]
        public void ToPropertyGroup_WithSingleHyphenWithoutValue_ParsesWithEmptyValue()
        {
            // Arrange
            var args = new[] { "-v", "-f" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(2, result.Items.Count);

            var vProp = result.Items.FirstOrDefault(p => p.Name == "v") as StringProperty;
            var fProp = result.Items.FirstOrDefault(p => p.Name == "f") as StringProperty;

            Assert.IsNotNull(vProp);
            Assert.AreEqual(string.Empty, vProp.Value);

            Assert.IsNotNull(fProp);
            Assert.AreEqual(string.Empty, fProp.Value);
        }

        [Test]
        public void ToPropertyGroup_WithPositionalArgs_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "input.txt", "output.txt", "third.txt" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(3, result.Items.Count);

            var arg0 = result.Items.FirstOrDefault(p => p.Name == "Arg0") as StringProperty;
            var arg1 = result.Items.FirstOrDefault(p => p.Name == "Arg1") as StringProperty;
            var arg2 = result.Items.FirstOrDefault(p => p.Name == "Arg2") as StringProperty;

            Assert.IsNotNull(arg0);
            Assert.AreEqual("input.txt", arg0.Value);

            Assert.IsNotNull(arg1);
            Assert.AreEqual("output.txt", arg1.Value);

            Assert.IsNotNull(arg2);
            Assert.AreEqual("third.txt", arg2.Value);
        }

        [Test]
        public void ToPropertyGroup_WithMixedFormats_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "--config=prod", "-v", "true", "/output:file.txt", "input.txt" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(args);

            // Assert
            Assert.AreEqual(4, result.Items.Count);

            var configProp = result.Items.FirstOrDefault(p => p.Name == "config") as StringProperty;
            var vProp = result.Items.FirstOrDefault(p => p.Name == "v") as StringProperty;
            var outputProp = result.Items.FirstOrDefault(p => p.Name == "output") as StringProperty;
            var arg4 = result.Items.FirstOrDefault(p => p.Name == "Arg4") as StringProperty;

            Assert.IsNotNull(configProp);
            Assert.AreEqual("prod", configProp.Value);

            Assert.IsNotNull(vProp);
            Assert.AreEqual("true", vProp.Value);

            Assert.IsNotNull(outputProp);
            Assert.AreEqual("file.txt", outputProp.Value);

            Assert.IsNotNull(arg4);
            Assert.AreEqual("input.txt", arg4.Value);
        }

        [Test]
        public void ToPropertyGroupByPosition_WithMatchingArgsAndNames_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "input.txt", "output.txt", "config.json" };
            var names = new[] { "InputFile", "OutputFile", "ConfigFile" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroupByPosition(args, names);

            // Assert
            Assert.AreEqual(3, result.Items.Count);

            var inputProp = result.Items.FirstOrDefault(p => p.Name == "InputFile") as StringProperty;
            var outputProp = result.Items.FirstOrDefault(p => p.Name == "OutputFile") as StringProperty;
            var configProp = result.Items.FirstOrDefault(p => p.Name == "ConfigFile") as StringProperty;

            Assert.IsNotNull(inputProp);
            Assert.AreEqual("input.txt", inputProp.Value);

            Assert.IsNotNull(outputProp);
            Assert.AreEqual("output.txt", outputProp.Value);

            Assert.IsNotNull(configProp);
            Assert.AreEqual("config.json", configProp.Value);
        }

        [Test]
        public void ToPropertyGroupByPosition_WithMoreArgsThanNames_OnlyProcessesMatchingCount()
        {
            // Arrange
            var args = new[] { "input.txt", "output.txt", "extra.txt" };
            var names = new[] { "InputFile", "OutputFile" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroupByPosition(args, names);

            // Assert
            Assert.AreEqual(2, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroupByPosition_WithNullArgs_ReturnsEmptyPropertyGroup()
        {
            // Arrange
            var names = new[] { "InputFile", "OutputFile" };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroupByPosition(null, names);

            // Assert
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public void ToPropertyGroup_WithDictionary_ParsesCorrectly()
        {
            // Arrange
            var argDict = new Dictionary<string, string>
            {
                { "config", "production" },
                { "verbose", "true" },
                { "port", "8080" }
            };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(argDict);

            // Assert
            Assert.AreEqual(3, result.Items.Count);

            var configProp = result.Items.FirstOrDefault(p => p.Name == "config") as StringProperty;
            var verboseProp = result.Items.FirstOrDefault(p => p.Name == "verbose") as StringProperty;
            var portProp = result.Items.FirstOrDefault(p => p.Name == "port") as StringProperty;

            Assert.IsNotNull(configProp);
            Assert.AreEqual("production", configProp.Value);

            Assert.IsNotNull(verboseProp);
            Assert.AreEqual("true", verboseProp.Value);

            Assert.IsNotNull(portProp);
            Assert.AreEqual("8080", portProp.Value);
        }

        [Test]
        public void ToPropertyGroup_WithDictionaryNullValue_UsesEmptyString()
        {
            // Arrange
            var argDict = new Dictionary<string, string>
            {
                { "config", null },
                { "verbose", "true" }
            };

            // Act
            var result = CommandLineArgsHelper.ToPropertyGroup(argDict);

            // Assert
            var configProp = result.Items.FirstOrDefault(p => p.Name == "config") as StringProperty;
            Assert.IsNotNull(configProp);
            Assert.AreEqual(string.Empty, configProp.Value);
        }

        [Test]
        public void ParseToDictionary_WithVariousFormats_ParsesCorrectly()
        {
            // Arrange
            var args = new[] { "--config=prod", "-v", "true", "/output:file.txt", "input.txt" };

            // Act
            var result = CommandLineArgsHelper.ParseToDictionary(args);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("prod", result["config"]);
            Assert.AreEqual("true", result["v"]);
            Assert.AreEqual("file.txt", result["output"]);
            Assert.AreEqual("input.txt", result["Arg4"]);
        }

        [Test]
        public void ParseToDictionary_WithNullArgs_ReturnsEmptyDictionary()
        {
            // Act
            var result = CommandLineArgsHelper.ParseToDictionary(null);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseToDictionary_WithEmptyArgs_ReturnsEmptyDictionary()
        {
            // Act
            var result = CommandLineArgsHelper.ParseToDictionary(new string[0]);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void AllMethods_ReturnStringProperties()
        {
            // Arrange
            var args = new[] { "--test=value" };
            var dict = new Dictionary<string, string> { { "test", "value" } };
            var names = new[] { "TestArg" };

            // Act
            var result1 = CommandLineArgsHelper.ToPropertyGroup(args);
            var result2 = CommandLineArgsHelper.ToPropertyGroupByPosition(args, names);
            var result3 = CommandLineArgsHelper.ToPropertyGroup(dict);

            // Assert
            Assert.IsTrue(result1.Items.All(p => p is StringProperty));
            Assert.IsTrue(result2.Items.All(p => p is StringProperty));
            Assert.IsTrue(result3.Items.All(p => p is StringProperty));
        }
    }
} 