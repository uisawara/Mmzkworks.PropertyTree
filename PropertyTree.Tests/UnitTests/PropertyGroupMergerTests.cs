using works.mmzk.PropertyTree;
using System;

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
} 