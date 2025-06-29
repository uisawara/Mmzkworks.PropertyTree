using System;
using System.Collections.Generic;
using System.Linq;
using works.mmzk.PropertyTree;

namespace Mmzkworks.PropertyTree.Tests.IntegrationTests;

[TestFixture]
public class PropertyGroupMergerIntegrationTests
{
    private const string TestEnvVar1 = "MMZK_INTEGRATION_TEST_1";
    private const string TestEnvVar2 = "MMZK_INTEGRATION_TEST_2";
    private const string TestEnvVar3 = "MMZK_INTEGRATION_PREFIX_VAR";
    
    [SetUp]
    public void SetUp()
    {
        // Set up test environment variables
        Environment.SetEnvironmentVariable(TestEnvVar1, "EnvValue1");
        Environment.SetEnvironmentVariable(TestEnvVar2, "EnvValue2");
        Environment.SetEnvironmentVariable(TestEnvVar3, "PrefixEnvValue");
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test environment variables
        Environment.SetEnvironmentVariable(TestEnvVar1, null);
        Environment.SetEnvironmentVariable(TestEnvVar2, null);
        Environment.SetEnvironmentVariable(TestEnvVar3, null);
    }

    [Test]
    public void MergeEnvironmentAndCommandLineArgs_WithOverwriteStrategy_ShouldMergeCorrectly()
    {
        // Arrange
        var args = new[] { "--name=Alice", "--age=25", "--" + TestEnvVar1 + "=CommandValue1" };
        
        var envGroup = EnvironmentHelper.ToPropertyGroup(new[] { TestEnvVar1, TestEnvVar2 }, "Environment");
        var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

        // Act
        var config = PropertyGroupMerger.MergeToNew("Configuration", envGroup, cmdGroup, MergeStrategy.Overwrite);

        // Assert
        Assert.That(config.Name, Is.EqualTo("Configuration"));
        Assert.That(config.Items.Count, Is.EqualTo(4)); // TestEnvVar1 (overridden), TestEnvVar2, name, age

        // Verify that command line arguments override environment variables
        var mergedVar1 = ((StringProperty)config.FindByName(TestEnvVar1));
        Assert.That(mergedVar1.Value, Is.EqualTo("CommandValue1"));
        
        var envVar2 = ((StringProperty)config.FindByName(TestEnvVar2));
        Assert.That(envVar2.Value, Is.EqualTo("EnvValue2"));
        
        var nameProperty = ((StringProperty)config.FindByName("name"));
        Assert.That(nameProperty.Value, Is.EqualTo("Alice"));
        
        var ageProperty = ((StringProperty)config.FindByName("age"));
        Assert.That(ageProperty.Value, Is.EqualTo("25"));
    }

    [Test]
    public void MergeEnvironmentAndCommandLineArgs_WithSkipStrategy_ShouldPreserveEnvironmentValues()
    {
        // Arrange
        var args = new[] { "--" + TestEnvVar1 + "=CommandValue1", "--name=Alice" };
        
        var envGroup = EnvironmentHelper.ToPropertyGroup(new[] { TestEnvVar1, TestEnvVar2 }, "Environment");
        var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

        // Act
        var config = PropertyGroupMerger.MergeToNew("Configuration", envGroup, cmdGroup, MergeStrategy.Skip);

        // Assert
        Assert.That(config.Name, Is.EqualTo("Configuration"));
        
        // Verify that environment variable values are preserved
        var mergedVar1 = ((StringProperty)config.FindByName(TestEnvVar1));
        Assert.That(mergedVar1.Value, Is.EqualTo("EnvValue1")); // Environment variable value is preserved
        
        var nameProperty = ((StringProperty)config.FindByName("name"));
        Assert.That(nameProperty.Value, Is.EqualTo("Alice")); // New properties are added
    }

    [Test]
    public void MergePrefixedEnvironmentWithCommandLineArgs_ShouldWorkCorrectly()
    {
        // Arrange
        var args = new[] { "--app-name=MyApp", "--app-version=1.0" };
        
        var envGroup = EnvironmentHelper.ToPropertyGroupByPrefix("MMZK_INTEGRATION_", "Environment", removePrefix: true);
        var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

        // Act
        var config = PropertyGroupMerger.MergeToNew("AppConfiguration", envGroup, cmdGroup);

        // Assert
        Assert.That(config.Name, Is.EqualTo("AppConfiguration"));
        
        // Environment variable with prefix removed
        var prefixVar = ((StringProperty)config.FindByName("PREFIX_VAR"));
        Assert.That(prefixVar.Value, Is.EqualTo("PrefixEnvValue"));
        
        // Command line arguments
        var appName = ((StringProperty)config.FindByName("app-name"));
        Assert.That(appName.Value, Is.EqualTo("MyApp"));
        
        var appVersion = ((StringProperty)config.FindByName("app-version"));
        Assert.That(appVersion.Value, Is.EqualTo("1.0"));
    }

    [Test]
    public void MergeMultipleSourcesWithDifferentStrategies_ShouldWorkCorrectly()
    {
        // Arrange
        var args = new[] { "--debug=true", "--" + TestEnvVar1 + "=CommandValue" };
        
        var defaultsGroup = new PropertyGroup("Defaults");
        defaultsGroup.Add(new StringProperty("timeout", "30"));
        defaultsGroup.Add(new StringProperty("debug", "false"));
        defaultsGroup.Add(new StringProperty(TestEnvVar1, "DefaultValue"));

        var envGroup = EnvironmentHelper.ToPropertyGroup(new[] { TestEnvVar1, TestEnvVar2 }, "Environment");
        var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

        // Act - Merge incrementally
        var config = new PropertyGroup("Configuration");
        
        // 1. Add default values
        PropertyGroupMerger.Merge(config, defaultsGroup, MergeStrategy.Overwrite);
        
        // 2. Override with environment variables
        PropertyGroupMerger.Merge(config, envGroup, MergeStrategy.Overwrite);
        
        // 3. Final override with command line arguments
        PropertyGroupMerger.Merge(config, cmdGroup, MergeStrategy.Overwrite);

        // Assert
        Assert.That(config.Items.Count, Is.EqualTo(4)); // timeout, debug, TestEnvVar1, TestEnvVar2

        // Priority: CommandLine > Environment > Defaults
        var timeoutProperty = ((StringProperty)config.FindByName("timeout"));
        Assert.That(timeoutProperty.Value, Is.EqualTo("30")); // Default value
        
        var debugProperty = ((StringProperty)config.FindByName("debug"));
        Assert.That(debugProperty.Value, Is.EqualTo("true")); // Command line argument
        
        var testVar1Property = ((StringProperty)config.FindByName(TestEnvVar1));
        Assert.That(testVar1Property.Value, Is.EqualTo("CommandValue")); // Command line argument
        
        var testVar2Property = ((StringProperty)config.FindByName(TestEnvVar2));
        Assert.That(testVar2Property.Value, Is.EqualTo("EnvValue2")); // Environment variable
    }

    [Test]
    public void MergeWithRenameStrategy_ShouldHandleConflictsCorrectly()
    {
        // Arrange
        var args = new[] { "--config-file=app.config", "--log-level=debug" };
        
        var configGroup = new PropertyGroup("BaseConfig");
        configGroup.Add(new StringProperty("config-file", "default.config"));
        configGroup.Add(new StringProperty("database-url", "localhost"));

        var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

        // Act
        var config = PropertyGroupMerger.MergeToNew("Configuration", configGroup, cmdGroup, MergeStrategy.Rename);

        // Assert
        Assert.That(config.Items.Count, Is.EqualTo(4)); // config-file, database-url, config-file_1, log-level

        // Original properties
        var originalConfigFile = ((StringProperty)config.FindByName("config-file"));
        Assert.That(originalConfigFile.Value, Is.EqualTo("default.config"));
        
        // Renamed properties
        var renamedConfigFile = ((StringProperty)config.FindByName("config-file_1"));
        Assert.That(renamedConfigFile.Value, Is.EqualTo("app.config"));
        
        // Non-conflicting properties
        var databaseUrl = ((StringProperty)config.FindByName("database-url"));
        Assert.That(databaseUrl.Value, Is.EqualTo("localhost"));
        
        var logLevel = ((StringProperty)config.FindByName("log-level"));
        Assert.That(logLevel.Value, Is.EqualTo("debug"));
    }

    [Test]
    public void CreateApplicationConfiguration_RealWorldScenario_ShouldWorkCorrectly()
    {
        // Arrange - Real-world application configuration scenario
        Environment.SetEnvironmentVariable("APP_DATABASE_URL", "prod://database");
        Environment.SetEnvironmentVariable("APP_LOG_LEVEL", "info");
        Environment.SetEnvironmentVariable("APP_PORT", "8080");

        var defaultsGroup = new PropertyGroup("Defaults");
        defaultsGroup.Add(new StringProperty("database-url", "sqlite://memory"));
        defaultsGroup.Add(new StringProperty("log-level", "warning"));
        defaultsGroup.Add(new StringProperty("port", "3000"));
        defaultsGroup.Add(new StringProperty("debug", "false"));

        var args = new[] { "--port=9000", "--debug=true" };

        try
        {
            // Act
            var envGroup = EnvironmentHelper.ToPropertyGroupByPrefix("APP_", "Environment", removePrefix: true);
            var cmdGroup = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

            // Configuration priority: Command line > Environment variables > Defaults
            var appConfig = new PropertyGroup("ApplicationConfiguration");
            PropertyGroupMerger.Merge(appConfig, defaultsGroup, MergeStrategy.Overwrite);
            PropertyGroupMerger.Merge(appConfig, envGroup, MergeStrategy.Overwrite);
            PropertyGroupMerger.Merge(appConfig, cmdGroup, MergeStrategy.Overwrite);

                    // Assert
        Assert.That(appConfig.Items.Count, Is.EqualTo(7)); // Defaults + environment variables + command line arguments

        var databaseUrl = ((StringProperty)appConfig.FindByName("DATABASE_URL"));
        Assert.That(databaseUrl.Value, Is.EqualTo("prod://database")); // Environment variable

        var logLevel = ((StringProperty)appConfig.FindByName("LOG_LEVEL"));
        Assert.That(logLevel.Value, Is.EqualTo("info")); // Environment variable

        var port = ((StringProperty)appConfig.FindByName("port"));
        Assert.That(port.Value, Is.EqualTo("9000")); // Command line argument (highest priority)

        var debug = ((StringProperty)appConfig.FindByName("debug"));
        Assert.That(debug.Value, Is.EqualTo("true")); // Command line argument
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("APP_DATABASE_URL", null);
            Environment.SetEnvironmentVariable("APP_LOG_LEVEL", null);
            Environment.SetEnvironmentVariable("APP_PORT", null);
        }
    }

    [Test]
    public void MergeWithComplexTypes_ShouldPreservePropertyTypes()
    {
        // Arrange
        var configGroup = new PropertyGroup("Config");
        configGroup.Add(new IntProperty("max-connections", 100));
        configGroup.Add(new BoolProperty("enable-ssl", true));
        configGroup.Add(new FloatProperty("timeout-seconds", 30.5f));

        var envGroup = new PropertyGroup("Environment");
        envGroup.Add(new StringProperty("database-host", "localhost"));

        // Act
        var merged = PropertyGroupMerger.MergeToNew("MergedConfig", configGroup, envGroup);

        // Assert
        Assert.That(merged.Items.Count, Is.EqualTo(4));

        // Verify that types are preserved
        Assert.That(merged.FindByName("max-connections"), Is.InstanceOf<IntProperty>());
        Assert.That(merged.FindByName("enable-ssl"), Is.InstanceOf<BoolProperty>());
        Assert.That(merged.FindByName("timeout-seconds"), Is.InstanceOf<FloatProperty>());
        Assert.That(merged.FindByName("database-host"), Is.InstanceOf<StringProperty>());

        // Verify that values are correct
        Assert.That(((IntProperty)merged.FindByName("max-connections")).Value, Is.EqualTo(100));
        Assert.That(((BoolProperty)merged.FindByName("enable-ssl")).Value, Is.True);
        Assert.That(((FloatProperty)merged.FindByName("timeout-seconds")).Value, Is.EqualTo(30.5f));
        Assert.That(((StringProperty)merged.FindByName("database-host")).Value, Is.EqualTo("localhost"));
    }
} 