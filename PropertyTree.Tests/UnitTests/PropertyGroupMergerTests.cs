using works.mmzk.PropertyTree;
using System;
using System.Collections.Generic;

namespace Mmzkworks.PropertyTree.Tests.UnitTests;

public class PropertyGroupMergerTests
{
    [Test]
    public void Merge_WithOverwriteStrategy_ShouldOverwriteExistingProperties()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));
        source.Add(new StringProperty("city", "Tokyo"));

        // Act
        int mergedCount = PropertyGroupMerger.Merge(target, source, MergeStrategy.Overwrite);

        // Assert
        Assert.That(mergedCount, Is.EqualTo(2));
        Assert.That(target.Items.Count, Is.EqualTo(3));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Bob"));
        Assert.That(((StringProperty)target.FindByName("city")).Value, Is.EqualTo("Tokyo"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(25));
    }

    [Test]
    public void Merge_WithSkipStrategy_ShouldSkipExistingProperties()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));
        source.Add(new StringProperty("city", "Tokyo"));

        // Act
        int mergedCount = PropertyGroupMerger.Merge(target, source, MergeStrategy.Skip);

        // Assert
        Assert.That(mergedCount, Is.EqualTo(1)); // Only city added
        Assert.That(target.Items.Count, Is.EqualTo(3));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice")); // Original value unchanged
        Assert.That(((StringProperty)target.FindByName("city")).Value, Is.EqualTo("Tokyo"));
    }

    [Test]
    public void Merge_WithRenameStrategy_ShouldRenameConflictingProperties()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));

        // Act
        int mergedCount = PropertyGroupMerger.Merge(target, source, MergeStrategy.Rename);

        // Assert
        Assert.That(mergedCount, Is.EqualTo(1));
        Assert.That(target.Items.Count, Is.EqualTo(2));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((StringProperty)target.FindByName("name_1")).Value, Is.EqualTo("Bob"));
    }

    [Test]
    public void Merge_WithThrowStrategy_ShouldThrowOnConflict()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            PropertyGroupMerger.Merge(target, source, MergeStrategy.Throw));
        
        Assert.That(exception.Message, Does.Contain("Property 'name' already exists"));
    }

    [Test]
    public void MergeToNew_ShouldCreateNewPropertyGroupWithMergedContent()
    {
        // Arrange
        var first = new PropertyGroup("First");
        first.Add(new StringProperty("name", "Alice"));
        first.Add(new IntProperty("age", 25));

        var second = new PropertyGroup("Second");
        second.Add(new StringProperty("city", "Tokyo"));
        second.Add(new BoolProperty("active", true));

        // Act
        var result = PropertyGroupMerger.MergeToNew("Merged", first, second);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Merged"));
        Assert.That(result.Items.Count, Is.EqualTo(4));
        Assert.That(((StringProperty)result.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((IntProperty)result.FindByName("age")).Value, Is.EqualTo(25));
        Assert.That(((StringProperty)result.FindByName("city")).Value, Is.EqualTo("Tokyo"));
        Assert.That(((BoolProperty)result.FindByName("active")).Value, Is.True);
    }

    [Test]
    public void MergeAll_ShouldMergeMultipleGroups()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("base", "value"));

        var source1 = new PropertyGroup("Source1");
        source1.Add(new StringProperty("name", "Alice"));

        var source2 = new PropertyGroup("Source2");
        source2.Add(new IntProperty("age", 25));

        var source3 = new PropertyGroup("Source3");
        source3.Add(new StringProperty("city", "Tokyo"));

        var sources = new[] { source1, source2, source3 };

        // Act
        int totalMerged = PropertyGroupMerger.MergeAll(target, sources);

        // Assert
        Assert.That(totalMerged, Is.EqualTo(3));
        Assert.That(target.Items.Count, Is.EqualTo(4));
        Assert.That(((StringProperty)target.FindByName("base")).Value, Is.EqualTo("value"));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(25));
        Assert.That(((StringProperty)target.FindByName("city")).Value, Is.EqualTo("Tokyo"));
    }

    [Test]
    public void PropertyGroupMerger_DirectUsage_ShouldWorkCorrectly()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("city", "Tokyo"));

        // Act - Using PropertyGroupMerger directly
        int mergedCount = PropertyGroupMerger.Merge(target, source);

        // Assert
        Assert.That(mergedCount, Is.EqualTo(1));
        Assert.That(target.Items.Count, Is.EqualTo(2));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((StringProperty)target.FindByName("city")).Value, Is.EqualTo("Tokyo"));
    }

    [Test]
    public void PropertyGroupMerger_ActionPropertyRename_ShouldPreserveAction()
    {
        // Arrange - Test that ActionProperty's Action is correctly duplicated
        var target = new PropertyGroup("Target");
        int executionCount = 0;
        Action originalActionCallback = () => executionCount++;
        
        var original = new ActionProperty("action", originalActionCallback);
        target.Add(original);

        var source = new PropertyGroup("Source");
        var duplicate = new ActionProperty("action", () => executionCount += 10);
        source.Add(duplicate);

        // Act - Merge with rename strategy (ActionProperty is duplicated)
        int mergedCount = PropertyGroupMerger.Merge(target, source, MergeStrategy.Rename);

        // Assert
        Assert.That(mergedCount, Is.EqualTo(1));
        Assert.That(target.Items.Count, Is.EqualTo(2));
        
        // Verify that the original ActionProperty's Action works correctly
        var originalActionProperty = (ActionProperty)target.FindByName("action");
        originalActionProperty.Execute();
        Assert.That(executionCount, Is.EqualTo(1), "Original Action should be executed");
        
        // Verify that the renamed ActionProperty's Action works correctly
        var renamedActionProperty = (ActionProperty)target.FindByName("action_1");
        Assert.That(renamedActionProperty, Is.Not.Null, "Renamed ActionProperty should exist");
        Assert.That(renamedActionProperty.Action, Is.Not.Null, "Action should be accessible via Action property getter");
        
        renamedActionProperty.Execute();
        Assert.That(executionCount, Is.EqualTo(11), "Renamed Action should also execute correctly");
    }

    #region LeftMerge Tests

    [Test]
    public void LeftMerge_WithMatchingProperties_ShouldOverwriteTargetWithSourceValues()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));
        target.Add(new BoolProperty("active", true));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob")); // targetと同じ名前
        source.Add(new IntProperty("age", 30));       // targetと同じ名前
        source.Add(new StringProperty("city", "Tokyo")); // targetにはない

        // Act
        int updatedCount = PropertyGroupMerger.LeftMerge(target, source);

        // Assert
        Assert.That(updatedCount, Is.EqualTo(2), "nameとageが上書きされるべき");
        Assert.That(target.Items.Count, Is.EqualTo(3), "targetの構造を保持するべき");
        
        // 上書きされたプロパティを確認
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Bob"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(30));
        
        // targetにのみ存在するプロパティが保持されることを確認
        Assert.That(((BoolProperty)target.FindByName("active")).Value, Is.EqualTo(true));
        
        // sourceにのみ存在するプロパティは追加されないことを確認
        Assert.That(target.FindByName("city"), Is.Null);
    }

    [Test]
    public void LeftMerge_WithNoMatchingProperties_ShouldNotChangeTarget()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("city", "Tokyo"));
        source.Add(new BoolProperty("active", true));

        // Act
        int updatedCount = PropertyGroupMerger.LeftMerge(target, source);

        // Assert
        Assert.That(updatedCount, Is.EqualTo(0), "一致するプロパティがないため更新されない");
        Assert.That(target.Items.Count, Is.EqualTo(2), "targetの構造は変更されない");
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(25));
        Assert.That(target.FindByName("city"), Is.Null);
        Assert.That(target.FindByName("active"), Is.Null);
    }

    [Test]
    public void LeftMerge_WithEmptyTarget_ShouldNotAddAnyProperties()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        
        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));
        source.Add(new IntProperty("age", 30));

        // Act
        int updatedCount = PropertyGroupMerger.LeftMerge(target, source);

        // Assert
        Assert.That(updatedCount, Is.EqualTo(0));
        Assert.That(target.Items.Count, Is.EqualTo(0), "空のtargetには何も追加されない");
    }

    [Test]
    public void LeftMerge_WithEmptySource_ShouldNotChangeTarget()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));

        var source = new PropertyGroup("Source");

        // Act
        int updatedCount = PropertyGroupMerger.LeftMerge(target, source);

        // Assert
        Assert.That(updatedCount, Is.EqualTo(0));
        Assert.That(target.Items.Count, Is.EqualTo(2));
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(25));
    }

    [Test]
    public void LeftMerge_WithNullArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        var validGroup = new PropertyGroup("Valid");
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMerge(null, validGroup));
        
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMerge(validGroup, null));
    }

    [Test]
    public void LeftMergeAll_WithMultipleSources_ShouldUpdateMatchingProperties()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));
        target.Add(new BoolProperty("active", true));
        target.Add(new StringProperty("city", "Kyoto"));

        var source1 = new PropertyGroup("Source1");
        source1.Add(new StringProperty("name", "Bob"));
        source1.Add(new StringProperty("country", "Japan")); // targetにはない

        var source2 = new PropertyGroup("Source2");
        source2.Add(new IntProperty("age", 30));
        source2.Add(new StringProperty("city", "Tokyo"));

        var source3 = new PropertyGroup("Source3");
        source3.Add(new StringProperty("name", "Charlie")); // 最終的にこの値になる
        source3.Add(new FloatProperty("score", 95.5f)); // targetにはない

        var sources = new[] { source1, source2, source3 };

        // Act
        int totalUpdated = PropertyGroupMerger.LeftMergeAll(target, sources);

        // Assert
        Assert.That(totalUpdated, Is.EqualTo(4), "name(2回)、age(1回)、city(1回)が更新される");
        Assert.That(target.Items.Count, Is.EqualTo(4), "targetの構造を保持");
        
        // 最終的な値を確認
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Charlie"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(30));
        Assert.That(((BoolProperty)target.FindByName("active")).Value, Is.EqualTo(true));
        Assert.That(((StringProperty)target.FindByName("city")).Value, Is.EqualTo("Tokyo"));
        
        // sourceにのみ存在するプロパティは追加されないことを確認
        Assert.That(target.FindByName("country"), Is.Null);
        Assert.That(target.FindByName("score"), Is.Null);
    }

    [Test]
    public void LeftMergeToNew_ShouldCreateNewGroupWithLeftMergedValues()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));

        var source = new PropertyGroup("Source");
        source.Add(new StringProperty("name", "Bob"));
        source.Add(new StringProperty("city", "Tokyo"));

        // Act
        var result = PropertyGroupMerger.LeftMergeToNew("MergedResult", target, source);

        // Assert
        Assert.That(result.Name, Is.EqualTo("MergedResult"));
        Assert.That(result.Items.Count, Is.EqualTo(2), "targetの構造を保持");
        Assert.That(((StringProperty)result.FindByName("name")).Value, Is.EqualTo("Bob"));
        Assert.That(((IntProperty)result.FindByName("age")).Value, Is.EqualTo(25));
        Assert.That(result.FindByName("city"), Is.Null);
        
        // 元のグループは変更されないことを確認
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
    }

    [Test]
    public void LeftMergeToNew_WithMultipleSources_ShouldCreateNewGroupWithLeftMergedValues()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        target.Add(new StringProperty("name", "Alice"));
        target.Add(new IntProperty("age", 25));
        target.Add(new BoolProperty("active", true));

        var source1 = new PropertyGroup("Source1");
        source1.Add(new StringProperty("name", "Bob"));

        var source2 = new PropertyGroup("Source2");
        source2.Add(new IntProperty("age", 30));
        source2.Add(new StringProperty("city", "Tokyo"));

        var sources = new[] { source1, source2 };

        // Act
        var result = PropertyGroupMerger.LeftMergeToNew("MergedResult", target, sources);

        // Assert
        Assert.That(result.Name, Is.EqualTo("MergedResult"));
        Assert.That(result.Items.Count, Is.EqualTo(3), "targetの構造を保持");
        Assert.That(((StringProperty)result.FindByName("name")).Value, Is.EqualTo("Bob"));
        Assert.That(((IntProperty)result.FindByName("age")).Value, Is.EqualTo(30));
        Assert.That(((BoolProperty)result.FindByName("active")).Value, Is.EqualTo(true));
        Assert.That(result.FindByName("city"), Is.Null);
        
        // 元のグループは変更されないことを確認
        Assert.That(((StringProperty)target.FindByName("name")).Value, Is.EqualTo("Alice"));
        Assert.That(((IntProperty)target.FindByName("age")).Value, Is.EqualTo(25));
    }

    [Test]
    public void LeftMergeToNew_WithNullArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        var validGroup = new PropertyGroup("Valid");
        var validSources = new[] { validGroup };
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMergeToNew("Test", null, validGroup));
        
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMergeToNew("Test", validGroup, (PropertyGroup)null));
        
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMergeToNew("Test", null, validSources));
        
        Assert.Throws<ArgumentNullException>(() =>
            PropertyGroupMerger.LeftMergeToNew("Test", validGroup, (IEnumerable<PropertyGroup>)null));
    }

    [Test]
    public void LeftMerge_WithPropertyGroupProperties_ShouldUpdateCorrectly()
    {
        // Arrange
        var target = new PropertyGroup("Target");
        var targetSubGroup = new PropertyGroup("subGroup");
        targetSubGroup.Add(new StringProperty("subName", "TargetSub"));
        target.Add(targetSubGroup);

        var source = new PropertyGroup("Source");
        var sourceSubGroup = new PropertyGroup("subGroup");
        sourceSubGroup.Add(new StringProperty("subName", "SourceSub"));
        source.Add(sourceSubGroup);

        // Act
        int updatedCount = PropertyGroupMerger.LeftMerge(target, source);

        // Assert
        Assert.That(updatedCount, Is.EqualTo(1));
        Assert.That(target.Items.Count, Is.EqualTo(1));
        
        var resultSubGroup = (PropertyGroup)target.FindByName("subGroup");
        Assert.That(resultSubGroup, Is.Not.Null);
        Assert.That(((StringProperty)resultSubGroup.FindByName("subName")).Value, Is.EqualTo("SourceSub"));
    }

    #endregion
} 