using System;
using System.Collections.Generic;
using works.mmzk.PropertyTree;

namespace Mmzkworks.PropertyTree.Tests.UnitTests;

/// <summary>
/// Test class demonstrating practical usage examples of PropertyGroupMerger
/// </summary>
[TestFixture]
public class PropertyGroupMergerUsageExamplesTests
{
    [Test]
    public void Example_WebApplicationConfiguration_ShouldDemonstrateTypicalUsage()
    {
        // Arrange - Web application configuration example

        // 1. Default configuration
        var defaults = new PropertyGroup("Defaults");
        defaults.Add(new StringProperty("host", "localhost"));
        defaults.Add(new IntProperty("port", 3000));
        defaults.Add(new BoolProperty("debug", false));
        defaults.Add(new StringProperty("database-connection", "sqlite://memory"));
        defaults.Add(new IntProperty("max-connections", 10));

        // 2. Environment variables (configured in production environment)
        Environment.SetEnvironmentVariable("WEBAPP_HOST", "0.0.0.0");
        Environment.SetEnvironmentVariable("WEBAPP_PORT", "8080");
        Environment.SetEnvironmentVariable("WEBAPP_DATABASE_CONNECTION", "postgresql://prod-db:5432/app");
        Environment.SetEnvironmentVariable("WEBAPP_MAX_CONNECTIONS", "100");

        // 3. Command line arguments (development time override)
        var args = new[] { "--debug=true", "--port=9000" };

        try
        {
            // Act - Configuration building (priority: CLI > ENV > Defaults)
            var envConfig = EnvironmentHelper.ToPropertyGroupByPrefix("WEBAPP_", "Environment", removePrefix: true);
            var cliConfig = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");

            var appConfig = new PropertyGroup("WebAppConfiguration");
            PropertyGroupMerger.Merge(appConfig, defaults, MergeStrategy.Overwrite);
            PropertyGroupMerger.Merge(appConfig, envConfig, MergeStrategy.Overwrite);
            PropertyGroupMerger.Merge(appConfig, cliConfig, MergeStrategy.Overwrite);

            // Assert
            Assert.That(appConfig.Items.Count, Is.EqualTo(9)); // Defaults + environment variables + command line arguments

            // Host overridden by environment variables
            Assert.That(((StringProperty)appConfig.FindByName("HOST")).Value, Is.EqualTo("0.0.0.0")); // Environment variable
            
            // Overridden by environment variables
            Assert.That(((StringProperty)appConfig.FindByName("DATABASE_CONNECTION")).Value, 
                Is.EqualTo("postgresql://prod-db:5432/app"));
            Assert.That(((StringProperty)appConfig.FindByName("MAX_CONNECTIONS")).Value, Is.EqualTo("100"));
            
            // Final override by command line arguments
            Assert.That(((StringProperty)appConfig.FindByName("port")).Value, Is.EqualTo("9000"));
            Assert.That(((StringProperty)appConfig.FindByName("debug")).Value, Is.EqualTo("true"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("WEBAPP_HOST", null);
            Environment.SetEnvironmentVariable("WEBAPP_PORT", null);
            Environment.SetEnvironmentVariable("WEBAPP_DATABASE_CONNECTION", null);
            Environment.SetEnvironmentVariable("WEBAPP_MAX_CONNECTIONS", null);
        }
    }

    [Test]
    public void Example_DatabaseConfiguration_WithMultipleEnvironments_ShouldWorkCorrectly()
    {
        // Arrange - Configuration management example for multiple environments

        // Base configuration
        var baseConfig = new PropertyGroup("BaseConfig");
        baseConfig.Add(new StringProperty("driver", "sqlite"));
        baseConfig.Add(new IntProperty("timeout", 30));
        baseConfig.Add(new BoolProperty("auto-migrate", false));

        // Development environment configuration
        var devConfig = new PropertyGroup("Development");
        devConfig.Add(new StringProperty("connection-string", "sqlite://dev.db"));
        devConfig.Add(new BoolProperty("auto-migrate", true));
        devConfig.Add(new BoolProperty("verbose-logging", true));

        // Production environment configuration
        var prodConfig = new PropertyGroup("Production");
        prodConfig.Add(new StringProperty("driver", "postgresql"));
        prodConfig.Add(new StringProperty("connection-string", "postgresql://prod-server/db"));
        prodConfig.Add(new IntProperty("timeout", 60));
        prodConfig.Add(new IntProperty("pool-size", 20));

        // Act - Configuration building based on environment
        var developmentSettings = PropertyGroupMerger.MergeToNew("DevSettings", baseConfig, devConfig);
        var productionSettings = PropertyGroupMerger.MergeToNew("ProdSettings", baseConfig, prodConfig);

        // Assert - Development environment
        Assert.That(((StringProperty)developmentSettings.FindByName("driver")).Value, Is.EqualTo("sqlite"));
        Assert.That(((StringProperty)developmentSettings.FindByName("connection-string")).Value, Is.EqualTo("sqlite://dev.db"));
        Assert.That(((BoolProperty)developmentSettings.FindByName("auto-migrate")).Value, Is.True);
        Assert.That(((BoolProperty)developmentSettings.FindByName("verbose-logging")).Value, Is.True);
        Assert.That(((IntProperty)developmentSettings.FindByName("timeout")).Value, Is.EqualTo(30));

        // Assert - Production environment
        Assert.That(((StringProperty)productionSettings.FindByName("driver")).Value, Is.EqualTo("postgresql"));
        Assert.That(((StringProperty)productionSettings.FindByName("connection-string")).Value, 
            Is.EqualTo("postgresql://prod-server/db"));
        Assert.That(((BoolProperty)productionSettings.FindByName("auto-migrate")).Value, Is.False);
        Assert.That(((IntProperty)productionSettings.FindByName("timeout")).Value, Is.EqualTo(60));
        Assert.That(((IntProperty)productionSettings.FindByName("pool-size")).Value, Is.EqualTo(20));
    }

    [Test]
    public void Example_FeatureFlags_WithRenameStrategy_ShouldHandleConflicts()
    {
        // Arrange - Feature flags management example

        // System default flags
        var systemFlags = new PropertyGroup("SystemFlags");
        systemFlags.Add(new BoolProperty("feature-a", false));
        systemFlags.Add(new BoolProperty("feature-b", true));
        systemFlags.Add(new BoolProperty("experimental-ui", false));

        // User configuration flags
        var userFlags = new PropertyGroup("UserFlags");
        userFlags.Add(new BoolProperty("feature-a", true)); // Conflict
        userFlags.Add(new BoolProperty("feature-c", true));
        userFlags.Add(new BoolProperty("experimental-ui", true)); // Conflict

        // A/B test configuration
        var abTestFlags = new PropertyGroup("ABTestFlags");
        abTestFlags.Add(new BoolProperty("feature-a", false)); // Conflict
        abTestFlags.Add(new BoolProperty("ab-test-variant", true));

        // Act - Using rename strategy to avoid conflicts
        var allFlags = PropertyGroupMerger.MergeToNew("AllFlags", systemFlags, userFlags, MergeStrategy.Rename);
        PropertyGroupMerger.Merge(allFlags, abTestFlags, MergeStrategy.Rename);

        // Assert
        Assert.That(allFlags.Items.Count, Is.EqualTo(8)); // 3 + 3 + 2 (including renamed conflicts)

        // System flags (unchanged)
        Assert.That(((BoolProperty)allFlags.FindByName("feature-a")).Value, Is.False);
        Assert.That(((BoolProperty)allFlags.FindByName("feature-b")).Value, Is.True);
        Assert.That(((BoolProperty)allFlags.FindByName("experimental-ui")).Value, Is.False);

        // User flags (renamed conflicts)
        Assert.That(((BoolProperty)allFlags.FindByName("feature-a_1")).Value, Is.True);
        Assert.That(((BoolProperty)allFlags.FindByName("experimental-ui_1")).Value, Is.True);
        Assert.That(((BoolProperty)allFlags.FindByName("feature-c")).Value, Is.True);

        // A/B test flags (renamed conflicts)
        Assert.That(((BoolProperty)allFlags.FindByName("feature-a_2")).Value, Is.False);
        Assert.That(((BoolProperty)allFlags.FindByName("ab-test-variant")).Value, Is.True);
    }

    [Test]
    public void Example_ConfigurationValidation_ShouldDemonstrateValidation()
    {
        // Arrange - Configuration validation example
        var config = new PropertyGroup("AppConfig");
        config.Add(new StringProperty("api-key", ""));
        config.Add(new IntProperty("port", 0));
        config.Add(new StringProperty("database-url", ""));

        var overrides = new PropertyGroup("Overrides");
        overrides.Add(new StringProperty("api-key", "secret-key-123"));
        overrides.Add(new IntProperty("port", 8080));

        // Act
        PropertyGroupMerger.Merge(config, overrides, MergeStrategy.Overwrite);

        // Assert - Configuration validation
        var apiKey = ((StringProperty)config.FindByName("api-key")).Value;
        var port = ((IntProperty)config.FindByName("port")).Value;
        var databaseUrl = ((StringProperty)config.FindByName("database-url")).Value;

        Assert.That(apiKey, Is.Not.Empty, "API key is required");
        Assert.That(port, Is.GreaterThan(0), "Port number must be greater than 0");
        Assert.That(databaseUrl, Is.EqualTo(""), "Database URL is not configured"); // Detected as warning
    }

    [Test]
    public void Example_HierarchicalConfiguration_ShouldWorkWithNestedGroups()
    {
        // Arrange - Hierarchical configuration example
        var serverConfig = new PropertyGroup("Server");
        serverConfig.Add(new StringProperty("host", "localhost"));
        serverConfig.Add(new IntProperty("port", 3000));

        var databaseConfig = new PropertyGroup("Database");
        databaseConfig.Add(new StringProperty("host", "localhost"));
        databaseConfig.Add(new IntProperty("port", 5432));
        databaseConfig.Add(new StringProperty("name", "myapp"));

        var loggingConfig = new PropertyGroup("Logging");
        loggingConfig.Add(new StringProperty("level", "info"));
        loggingConfig.Add(new BoolProperty("console", true));

        // Act - Merge hierarchical configurations into one group
        var appConfig = new PropertyGroup("ApplicationConfig");
        appConfig.Add(serverConfig);
        appConfig.Add(databaseConfig);
        appConfig.Add(loggingConfig);

        // Override some configurations with environment variables
        Environment.SetEnvironmentVariable("APP_SERVER_PORT", "8080");
        Environment.SetEnvironmentVariable("APP_LOGGING_LEVEL", "debug");

        try
        {
            var envOverrides = new PropertyGroup("EnvOverrides");
            envOverrides.Add(new IntProperty("port", 8080)); // Server port
            envOverrides.Add(new StringProperty("level", "debug")); // Log level

            // Apply environment variable overrides to each section (in actual implementation, each group is processed individually)
            var serverGroup = (PropertyGroup)appConfig.FindByName("Server");
            PropertyGroupMerger.Merge(serverGroup, envOverrides, MergeStrategy.Overwrite);

            var loggingGroup = (PropertyGroup)appConfig.FindByName("Logging");
            PropertyGroupMerger.Merge(loggingGroup, envOverrides, MergeStrategy.Overwrite);

            // Assert
            Assert.That(((IntProperty)serverGroup.FindByName("port")).Value, Is.EqualTo(8080));
            Assert.That(((StringProperty)loggingGroup.FindByName("level")).Value, Is.EqualTo("debug"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("APP_SERVER_PORT", null);
            Environment.SetEnvironmentVariable("APP_LOGGING_LEVEL", null);
        }
    }

    [Test]
    public void Example_ConfigurationBuilder_ShouldDemonstrateFluentAPI()
    {
        // Arrange - Configuration builder pattern example (assuming future extension)
        
        // Act - Fluent API-style configuration building
        var config = CreateConfigurationBuilder()
            .AddDefaults()
            .AddEnvironmentVariables("MYAPP_")
            .AddCommandLineArgs(new[] { "--debug=true", "--port=5000" })
            .Build();

        // Assert
        Assert.That(config.Name, Is.EqualTo("Configuration"));
        Assert.That(config.Items.Count, Is.GreaterThan(0));
        
        // Verify that debug is enabled
        var debugProp = config.FindByName("debug") as StringProperty;
        Assert.That(debugProp?.Value, Is.EqualTo("true"));
    }

    // Helper method - Configuration builder demo
    private ConfigurationBuilder CreateConfigurationBuilder()
    {
        return new ConfigurationBuilder();
    }

    // Configuration builder demo class
    private class ConfigurationBuilder
    {
        private readonly PropertyGroup _config = new PropertyGroup("Configuration");

        public ConfigurationBuilder AddDefaults()
        {
            var defaults = new PropertyGroup("Defaults");
            defaults.Add(new StringProperty("host", "localhost"));
            defaults.Add(new IntProperty("port", 3000));
            defaults.Add(new BoolProperty("debug", false));
            
            PropertyGroupMerger.Merge(_config, defaults, MergeStrategy.Overwrite);
            return this;
        }

        public ConfigurationBuilder AddEnvironmentVariables(string prefix)
        {
            var envConfig = EnvironmentHelper.ToPropertyGroupByPrefix(prefix, "Environment", removePrefix: true);
            PropertyGroupMerger.Merge(_config, envConfig, MergeStrategy.Overwrite);
            return this;
        }

        public ConfigurationBuilder AddCommandLineArgs(string[] args)
        {
            var cmdConfig = CommandLineArgsHelper.ToPropertyGroup(args, "CommandLine");
            PropertyGroupMerger.Merge(_config, cmdConfig, MergeStrategy.Overwrite);
            return this;
        }

        public PropertyGroup Build()
        {
            return _config;
        }
    }
} 