namespace works.mmzk.PropertyTree;

public static class PropertyExtensions
{
    /// <summary>
    ///     Gets the full path of a property
    ///     Example: "RootGroup.SubGroup.PropertyName"
    /// </summary>
    /// <param name="property">The target property</param>
    /// <returns>Full path string</returns>
    public static string GetFullPath(this IProperty property)
    {
        if (property == null)
            return string.Empty;

        var pathParts = new List<string>();
        var current = property;

        // Build path by traversing from current property to parents
        while (current != null)
        {
            pathParts.Insert(0, current.Name);
            current = current.Parent;
        }

        return string.Join(".", pathParts);
    }

    /// <summary>
    ///     Gets the hierarchical level of a property
    ///     Root level is 0, children are 1, grandchildren are 2...
    /// </summary>
    /// <param name="property">The target property</param>
    /// <returns>Hierarchical level</returns>
    public static int GetDepth(this IProperty property)
    {
        if (property == null)
            return -1;

        var depth = 0;
        var current = property.Parent;

        while (current != null)
        {
            depth++;
            current = current.Parent;
        }

        return depth;
    }

    /// <summary>
    ///     Determines whether a property is at a specified hierarchical level
    /// </summary>
    /// <param name="property">The target property</param>
    /// <param name="level">The hierarchical level to check</param>
    /// <returns>True if at the specified hierarchical level</returns>
    public static bool IsAtLevel(this IProperty property, int level)
    {
        return property.GetDepth() == level;
    }

    /// <summary>
    ///     Gets the parent chain of a property
    /// </summary>
    /// <param name="property">The target property</param>
    /// <returns>Array of parent chain (from root)</returns>
    public static PropertyGroup[] GetParentChain(this IProperty property)
    {
        if (property == null)
            return new PropertyGroup[0];

        var parents = new List<PropertyGroup>();
        var current = property.Parent;

        while (current != null)
        {
            parents.Insert(0, current);
            current = current.Parent;
        }

        return parents.ToArray();
    }

    #region Path-based Query Functions

    /// <summary>
    ///     Searches for a property using a path string
    ///     Example: "SubGroup1.FloatValue1"
    /// </summary>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="path">The path to search for</param>
    /// <returns>The found property, or null if not found</returns>
    public static IProperty FindByPath(this PropertyGroup group, string path)
    {
        if (group == null || string.IsNullOrEmpty(path))
            return null;

        var pathParts = path.Split('.');
        return FindByPathParts(group, pathParts, 0);
    }

    /// <summary>
    ///     Searches for a property using a path string and returns it cast to the specified type
    /// </summary>
    /// <typeparam name="T">Expected property type</typeparam>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="path">The path to search for</param>
    /// <returns>The found property, or null if not found</returns>
    public static T FindByPath<T>(this PropertyGroup group, string path) where T : class, IProperty
    {
        return FindByPath(group, path) as T;
    }

    /// <summary>
    ///     Searches for a property using a path string and throws an exception if not found
    /// </summary>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="path">The path to search for</param>
    /// <returns>The found property</returns>
    /// <exception cref="System.ArgumentException">When the property is not found</exception>
    public static IProperty GetByPath(this PropertyGroup group, string path)
    {
        var result = FindByPath(group, path);
        if (result == null)
            throw new ArgumentException($"Property not found at path: {path}");
        return result;
    }

    /// <summary>
    ///     Searches for a property using a path string, returns it cast to the specified type, and throws an exception if not
    ///     found
    /// </summary>
    /// <typeparam name="T">Expected property type</typeparam>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="path">The path to search for</param>
    /// <returns>The found property</returns>
    /// <exception cref="System.ArgumentException">When the property is not found</exception>
    public static T GetByPath<T>(this PropertyGroup group, string path) where T : class, IProperty
    {
        var result = GetByPath(group, path);
        if (result is not T typedResult)
            throw new ArgumentException($"Property at path '{path}' is not of type {typeof(T).Name}");
        return typedResult;
    }

    /// <summary>
    ///     Searches for properties using a wildcard pattern
    ///     Example: "SubGroup1.*" to get all properties in SubGroup1
    /// </summary>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="pattern">Wildcard pattern</param>
    /// <returns>List of matched properties</returns>
    public static List<IProperty> FindByPattern(this PropertyGroup group, string pattern)
    {
        var results = new List<IProperty>();
        if (group == null || string.IsNullOrEmpty(pattern))
            return results;

        CollectPropertiesByPattern(group, pattern, results);
        return results;
    }

    /// <summary>
    ///     Gets all properties with the specified path prefix
    /// </summary>
    /// <param name="group">The PropertyGroup to search in</param>
    /// <param name="prefix">Path prefix</param>
    /// <returns>List of properties matching the prefix</returns>
    public static List<IProperty> FindByPrefix(this PropertyGroup group, string prefix)
    {
        var results = new List<IProperty>();
        if (group == null || string.IsNullOrEmpty(prefix))
            return results;

        CollectPropertiesByPrefix(group, prefix, results);
        return results;
    }

    /// <summary>
    ///     Gets all properties as a flat list
    /// </summary>
    /// <param name="group">The target PropertyGroup</param>
    /// <returns>List of all properties</returns>
    public static List<IProperty> GetAllProperties(this PropertyGroup group)
    {
        var results = new List<IProperty>();
        if (group == null)
            return results;

        CollectAllProperties(group, results);
        return results;
    }

    #endregion

    #region Private Helper Methods

    private static IProperty FindByPathParts(PropertyGroup group, string[] pathParts, int index)
    {
        return FindByPathParts(group, pathParts, index, new HashSet<PropertyGroup>());
    }

    private static IProperty FindByPathParts(PropertyGroup group, string[] pathParts, int index,
        HashSet<PropertyGroup> visitedGroups)
    {
        if (index >= pathParts.Length)
            return null;

        // Detect circular reference
        if (visitedGroups.Contains(group)) return null; // Stop processing if group has already been visited

        visitedGroups.Add(group);

        var currentPart = pathParts[index];

        // Search for property within current group
        foreach (var item in group.Items)
            if (item.Name == currentPart)
            {
                // Return the property if this is the last part
                if (index == pathParts.Length - 1)
                {
                    visitedGroups.Remove(group);
                    return item;
                }

                // If there are still parts remaining, search recursively as PropertyGroup
                if (item is PropertyGroup subGroup)
                {
                    var result = FindByPathParts(subGroup, pathParts, index + 1, visitedGroups);
                    visitedGroups.Remove(group);
                    return result;
                }

                // Path is invalid if not a PropertyGroup
                visitedGroups.Remove(group);
                return null;
            }

        visitedGroups.Remove(group);
        return null;
    }

    private static void CollectPropertiesByPattern(PropertyGroup group, string pattern, List<IProperty> results)
    {
        var patternParts = pattern.Split('.');
        CollectPropertiesByPatternParts(group, patternParts, 0, results, new HashSet<PropertyGroup>());
    }

    private static void CollectPropertiesByPatternParts(PropertyGroup group, string[] patternParts, int index,
        List<IProperty> results, HashSet<PropertyGroup> visitedGroups)
    {
        if (index >= patternParts.Length)
            return;

        // Detect circular reference
        if (visitedGroups.Contains(group)) return; // Stop processing if group has already been visited

        visitedGroups.Add(group);

        var currentPattern = patternParts[index];

        foreach (var item in group.Items)
            if (IsPatternMatch(item.Name, currentPattern))
            {
                if (index == patternParts.Length - 1)
                    // Add to results if matched the last pattern
                    results.Add(item);
                else if (item is PropertyGroup subGroup)
                    // Search recursively if there are still patterns remaining
                    CollectPropertiesByPatternParts(subGroup, patternParts, index + 1, results, visitedGroups);
            }

        visitedGroups.Remove(group);
    }

    private static bool IsPatternMatch(string name, string pattern)
    {
        if (pattern == "*")
            return true;

        return name == pattern;
    }

    private static void CollectPropertiesByPrefix(PropertyGroup group, string prefix, List<IProperty> results)
    {
        CollectAllProperties(group, results);
        results.RemoveAll(p => !p.GetFullPath().StartsWith(prefix));
    }

    private static void CollectAllProperties(PropertyGroup group, List<IProperty> results)
    {
        CollectAllProperties(group, results, new HashSet<PropertyGroup>());
    }

    private static void CollectAllProperties(PropertyGroup group, List<IProperty> results,
        HashSet<PropertyGroup> visitedGroups)
    {
        // Detect circular reference
        if (visitedGroups.Contains(group)) return; // Stop processing if group has already been visited

        visitedGroups.Add(group);

        foreach (var item in group.Items)
        {
            results.Add(item);
            if (item is PropertyGroup subGroup) CollectAllProperties(subGroup, results, visitedGroups);
        }

        visitedGroups.Remove(group);
    }

    #endregion
}