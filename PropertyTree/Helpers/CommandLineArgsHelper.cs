namespace works.mmzk.PropertyTree;

public static class CommandLineArgsHelper
{
    /// <summary>
    /// Converts command line arguments to PropertyGroup
    /// Supports --key=value, -key value, /key:value formats
    /// </summary>
    /// <param name="args">Array of command line arguments</param>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <returns>PropertyGroup containing command line arguments</returns>
    public static PropertyGroup ToPropertyGroup(string[] args, string groupName = "CommandLineArgs")
    {
        var propertyGroup = new PropertyGroup(groupName);
        
        if (args == null || args.Length == 0)
            return propertyGroup;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            
            // --key=value or /key:value format
            if (arg.StartsWith("--") && arg.Contains("="))
            {
                var parts = arg.Substring(2).Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    var stringProperty = new StringProperty(parts[0], parts[1]);
                    propertyGroup.Add(stringProperty);
                }
            }
            else if (arg.StartsWith("/") && arg.Contains(":"))
            {
                var parts = arg.Substring(1).Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var stringProperty = new StringProperty(parts[0], parts[1]);
                    propertyGroup.Add(stringProperty);
                }
            }
            // -key value format
            else if (arg.StartsWith("-") && !arg.StartsWith("--"))
            {
                var key = arg.Substring(1);
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("-")) ? args[++i] : string.Empty;
                var stringProperty = new StringProperty(key, value);
                propertyGroup.Add(stringProperty);
            }
            // Positional arguments
            else if (!arg.StartsWith("-") && !arg.StartsWith("/"))
            {
                var stringProperty = new StringProperty($"Arg{i}", arg);
                propertyGroup.Add(stringProperty);
            }
        }

        return propertyGroup;
    }

    /// <summary>
    /// Converts command line arguments to PropertyGroup based on position
    /// </summary>
    /// <param name="args">Array of command line arguments</param>
    /// <param name="argumentNames">List of argument names for each position</param>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <returns>PropertyGroup containing position-based command line arguments</returns>
    public static PropertyGroup ToPropertyGroupByPosition(string[] args, IList<string> argumentNames, string groupName = "CommandLineArgs")
    {
        var propertyGroup = new PropertyGroup(groupName);
        
        if (args == null || argumentNames == null)
            return propertyGroup;

        for (int i = 0; i < Math.Min(args.Length, argumentNames.Count); i++)
        {
            var stringProperty = new StringProperty(argumentNames[i], args[i]);
            propertyGroup.Add(stringProperty);
        }

        return propertyGroup;
    }

    /// <summary>
    /// Converts command line arguments to PropertyGroup in Dictionary format
    /// </summary>
    /// <param name="argDictionary">Dictionary of argument keys and values</param>
    /// <param name="groupName">Name of the PropertyGroup to create</param>
    /// <returns>PropertyGroup containing Dictionary arguments</returns>
    public static PropertyGroup ToPropertyGroup(IDictionary<string, string> argDictionary, string groupName = "CommandLineArgs")
    {
        var propertyGroup = new PropertyGroup(groupName);
        
        if (argDictionary == null)
            return propertyGroup;

        foreach (var kvp in argDictionary)
        {
            var stringProperty = new StringProperty(kvp.Key, kvp.Value ?? string.Empty);
            propertyGroup.Add(stringProperty);
        }

        return propertyGroup;
    }

    /// <summary>
    /// Parses command line arguments and converts to Dictionary
    /// </summary>
    /// <param name="args">Array of command line arguments</param>
    /// <returns>Dictionary of parsed arguments</returns>
    public static Dictionary<string, string> ParseToDictionary(string[] args)
    {
        var result = new Dictionary<string, string>();
        
        if (args == null || args.Length == 0)
            return result;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            
            // --key=value or /key:value format
            if (arg.StartsWith("--") && arg.Contains("="))
            {
                var parts = arg.Substring(2).Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    result[parts[0]] = parts[1];
                }
            }
            else if (arg.StartsWith("/") && arg.Contains(":"))
            {
                var parts = arg.Substring(1).Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    result[parts[0]] = parts[1];
                }
            }
            // -key value format
            else if (arg.StartsWith("-") && !arg.StartsWith("--"))
            {
                var key = arg.Substring(1);
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("-")) ? args[++i] : string.Empty;
                result[key] = value;
            }
            // Positional arguments
            else if (!arg.StartsWith("-") && !arg.StartsWith("/"))
            {
                result[$"Arg{i}"] = arg;
            }
        }

        return result;
    }
} 