using System;
using System.Collections.Generic;
using System.Linq;

namespace works.mmzk.PropertyTree;

/// <summary>
/// Duplication handling method when merging PropertyGroups
/// </summary>
public enum MergeStrategy
{
    /// <summary>
    /// Overwrites existing properties
    /// </summary>
    Overwrite,
    
    /// <summary>
    /// Skips existing properties
    /// </summary>
    Skip,
    
    /// <summary>
    /// Throws an exception on duplication
    /// </summary>
    Throw,
    
    /// <summary>
    /// Renames and adds on duplication (e.g., "name" -> "name_1")
    /// </summary>
    Rename
}

/// <summary>
/// Helper class that provides PropertyGroup merge functionality
/// </summary>
public static class PropertyGroupMerger
{
    /// <summary>
    /// Merges another PropertyGroup into the specified PropertyGroup
    /// </summary>
    /// <param name="target">Target PropertyGroup for merge</param>
    /// <param name="source">Source PropertyGroup for merge</param>
    /// <param name="strategy">Handling method for duplications</param>
    /// <returns>Number of merged properties</returns>
    public static int Merge(PropertyGroup target, PropertyGroup source, MergeStrategy strategy = MergeStrategy.Overwrite)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        int mergedCount = 0;
        
        foreach (var property in source.Items)
        {
            if (TryMergeProperty(target, property, strategy))
            {
                mergedCount++;
            }
        }
        
        return mergedCount;
    }

    /// <summary>
    /// Merges multiple PropertyGroups into the specified PropertyGroup
    /// </summary>
    /// <param name="target">Target PropertyGroup for merge</param>
    /// <param name="sources">Collection of source PropertyGroups for merge</param>
    /// <param name="strategy">Handling method for duplications</param>
    /// <returns>Number of merged properties</returns>
    public static int MergeAll(PropertyGroup target, IEnumerable<PropertyGroup> sources, MergeStrategy strategy = MergeStrategy.Overwrite)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        
        int totalMerged = 0;
        
        foreach (var source in sources)
        {
            totalMerged += Merge(target, source, strategy);
        }
        
        return totalMerged;
    }

    /// <summary>
    /// Merges multiple PropertyGroups and creates a new PropertyGroup
    /// </summary>
    /// <param name="name">Name of the new PropertyGroup</param>
    /// <param name="sources">Collection of PropertyGroups to merge</param>
    /// <param name="strategy">Handling method for duplications</param>
    /// <returns>New merged PropertyGroup</returns>
    public static PropertyGroup MergeToNew(string name, IEnumerable<PropertyGroup> sources, MergeStrategy strategy = MergeStrategy.Overwrite)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        
        var result = new PropertyGroup(name);
        MergeAll(result, sources, strategy);
        return result;
    }

    /// <summary>
    /// Merges two PropertyGroups and creates a new PropertyGroup
    /// </summary>
    /// <param name="name">Name of the new PropertyGroup</param>
    /// <param name="first">First PropertyGroup</param>
    /// <param name="second">Second PropertyGroup</param>
    /// <param name="strategy">Handling method for duplications</param>
    /// <returns>New merged PropertyGroup</returns>
    public static PropertyGroup MergeToNew(string name, PropertyGroup first, PropertyGroup second, MergeStrategy strategy = MergeStrategy.Overwrite)
    {
        var result = new PropertyGroup(name);
        Merge(result, first, strategy);
        Merge(result, second, strategy);
        return result;
    }

    /// <summary>
    /// 左統合を実行します。targetの構造を基準にして、sourceに同じ名前のプロパティがあった場合のみtargetを上書きします
    /// </summary>
    /// <param name="target">マージ先のPropertyGroup（この構造を基準にする）</param>
    /// <param name="source">マージ元のPropertyGroup</param>
    /// <returns>上書きされたプロパティの数</returns>
    public static int LeftMerge(PropertyGroup target, PropertyGroup source)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        int updatedCount = 0;
        
        // targetの各プロパティについて、sourceに同じ名前のプロパティがあるかチェック
        foreach (var targetProperty in target.Items.ToList()) // ToListでコピーを作成して反復中の変更を回避
        {
            var sourceProperty = source.FindByName(targetProperty.Name);
            if (sourceProperty != null)
            {
                // 同じ名前のプロパティが見つかった場合、targetのプロパティを上書き
                target.Remove(targetProperty);
                target.Add(sourceProperty);
                updatedCount++;
            }
        }
        
        return updatedCount;
    }

    /// <summary>
    /// 複数のPropertyGroupで左統合を実行します
    /// </summary>
    /// <param name="target">マージ先のPropertyGroup（この構造を基準にする）</param>
    /// <param name="sources">マージ元のPropertyGroupのコレクション</param>
    /// <returns>上書きされたプロパティの数</returns>
    public static int LeftMergeAll(PropertyGroup target, IEnumerable<PropertyGroup> sources)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        
        int totalUpdated = 0;
        
        foreach (var source in sources)
        {
            totalUpdated += LeftMerge(target, source);
        }
        
        return totalUpdated;
    }

    /// <summary>
    /// 左統合を実行して新しいPropertyGroupを作成します
    /// </summary>
    /// <param name="name">新しいPropertyGroupの名前</param>
    /// <param name="target">構造の基準となるPropertyGroup</param>
    /// <param name="source">上書き元のPropertyGroup</param>
    /// <returns>左統合された新しいPropertyGroup</returns>
    public static PropertyGroup LeftMergeToNew(string name, PropertyGroup target, PropertyGroup source)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        var result = new PropertyGroup(name);
        
        // まずtargetの構造をコピー
        Merge(result, target, MergeStrategy.Overwrite);
        
        // 左統合を実行
        LeftMerge(result, source);
        
        return result;
    }

    /// <summary>
    /// 複数のPropertyGroupで左統合を実行して新しいPropertyGroupを作成します
    /// </summary>
    /// <param name="name">新しいPropertyGroupの名前</param>
    /// <param name="target">構造の基準となるPropertyGroup</param>
    /// <param name="sources">上書き元のPropertyGroupのコレクション</param>
    /// <returns>左統合された新しいPropertyGroup</returns>
    public static PropertyGroup LeftMergeToNew(string name, PropertyGroup target, IEnumerable<PropertyGroup> sources)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        
        var result = new PropertyGroup(name);
        
        // まずtargetの構造をコピー
        Merge(result, target, MergeStrategy.Overwrite);
        
        // 左統合を実行
        LeftMergeAll(result, sources);
        
        return result;
    }

    private static bool TryMergeProperty(PropertyGroup target, IProperty property, MergeStrategy strategy)
    {
        var existingProperty = target.FindByName(property.Name);
        
        if (existingProperty == null)
        {
            // Simply add if no duplication
            target.Add(property);
            return true;
        }
        
        // Handle duplication cases
        switch (strategy)
        {
            case MergeStrategy.Overwrite:
                // Remove existing property and add new property
                target.Remove(existingProperty);
                target.Add(property);
                return true;
                
            case MergeStrategy.Skip:
                // Keep existing property as is
                return false;
                
            case MergeStrategy.Throw:
                throw new InvalidOperationException($"Property '{property.Name}' already exists.");
                
            case MergeStrategy.Rename:
                // Rename and add
                var newName = GenerateUniqueName(target, property.Name);
                var renamedProperty = CreateRenamedProperty(property, newName);
                target.Add(renamedProperty);
                return true;
                
            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }
    }

    private static string GenerateUniqueName(PropertyGroup target, string originalName)
    {
        int counter = 1;
        string newName;
        
        do
        {
            newName = $"{originalName}_{counter}";
            counter++;
        } while (target.HasProperty(newName));
        
        return newName;
    }

    private static IProperty CreateRenamedProperty(IProperty original, string newName)
    {
        // Create new instance based on property type
        switch (original)
        {
            case StringProperty stringProp:
                return new StringProperty(newName, stringProp.Value);
            case IntProperty intProp:
                return new IntProperty(newName, intProp.Value);
            case FloatProperty floatProp:
                return new FloatProperty(newName, floatProp.Value);
            case BoolProperty boolProp:
                return new BoolProperty(newName, boolProp.Value);
            case PropertyGroup groupProp:
                return new PropertyGroup(newName, groupProp.Items);
            case ActionProperty actionProp:
                return new ActionProperty(newName, actionProp.Action);
            default:
                throw new NotSupportedException($"Renaming of property type '{original.GetType().Name}' is not supported.");
        }
    }
} 