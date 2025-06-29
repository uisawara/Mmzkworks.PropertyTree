using System.Collections;

namespace works.mmzk.PropertyTree;

public static class EnvironmentHelper
{
    /// <summary>
    /// Converts environment variables to PropertyGroup
    /// </summary>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <returns>PropertyGroup containing environment variables</returns>
    public static PropertyGroup ToPropertyGroup(string groupName = "Environment")
    {
        var propertyGroup = new PropertyGroup(groupName);
        var environmentVariables = Environment.GetEnvironmentVariables();

        foreach (DictionaryEntry entry in environmentVariables)
        {
            var key = entry.Key?.ToString();
            var value = entry.Value?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(key))
            {
                var stringProperty = new StringProperty(key, value);
                propertyGroup.Add(stringProperty);
            }
        }

        return propertyGroup;
    }

    /// <summary>
    /// Converts only specified environment variable names to PropertyGroup
    /// </summary>
    /// <param name="variableNames">List of environment variable names to retrieve</param>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <returns>PropertyGroup containing specified environment variables</returns>
    public static PropertyGroup ToPropertyGroup(IEnumerable<string> variableNames, string groupName = "Environment")
    {
        var propertyGroup = new PropertyGroup(groupName);

        foreach (var variableName in variableNames)
        {
            var value = Environment.GetEnvironmentVariable(variableName) ?? string.Empty;
            var stringProperty = new StringProperty(variableName, value);
            propertyGroup.Add(stringProperty);
        }

        return propertyGroup;
    }

    /// <summary>
    /// Filters environment variables by prefix and converts to PropertyGroup
    /// </summary>
    /// <param name="prefix">Environment variable name prefix</param>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <param name="removePrefix">Whether to remove prefix from property names</param>
    /// <returns>PropertyGroup containing filtered environment variables</returns>
    public static PropertyGroup ToPropertyGroupByPrefix(string prefix, string groupName = "Environment", bool removePrefix = false)
    {
        var propertyGroup = new PropertyGroup(groupName);
        var environmentVariables = Environment.GetEnvironmentVariables();

        foreach (DictionaryEntry entry in environmentVariables)
        {
            var key = entry.Key?.ToString();
            var value = entry.Value?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(key) && key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var propertyName = removePrefix && key.Length > prefix.Length 
                    ? key.Substring(prefix.Length) 
                    : key;
                
                var stringProperty = new StringProperty(propertyName, value);
                propertyGroup.Add(stringProperty);
            }
        }

        return propertyGroup;
    }
} 