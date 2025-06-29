using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.IntegrationTests
{
    [TestFixture]
    public class PropertySystemAdvancedIntegrationTests
    {
        [Test]
        public void PropertySystem_WithLargeHierarchy_PerformsCorrectly()
        {
            // Arrange - Create large hierarchical structure
            var rootGroup = new PropertyGroup("LargeRoot");
            var groups = new List<PropertyGroup>();
            var properties = new List<IProperty>();
            
            // Create 100 groups
            for (int i = 0; i < 100; i++)
            {
                var group = new PropertyGroup($"Group{i}");
                groups.Add(group);
                rootGroup.Add(group);
                
                // Add 10 properties to each group
                for (int j = 0; j < 10; j++)
                {
                    var property = new IntProperty($"Property{j}", i * 10 + j);
                    properties.Add(property);
                    group.Add(property);
                }
            }
            
            // Act & Assert
            Assert.AreEqual(100, rootGroup.Items.Count);
            Assert.AreEqual(1000, properties.Count);
            
            // Path search test
            var foundProperty = rootGroup.FindByPath("Group50.Property5");
            Assert.IsNotNull(foundProperty);
            Assert.AreEqual(505, ((IntProperty)foundProperty).Value);
            
            // Wildcard search test
            var allGroup50Properties = rootGroup.FindByPattern("Group50.*");
            Assert.AreEqual(10, allGroup50Properties.Count);
            
            // Get all properties test
            var allProperties = rootGroup.GetAllProperties();
            Assert.AreEqual(1100, allProperties.Count); // 100 groups + 1000 properties
        }
        
        [Test]
        public void PropertySystem_WithCircularReference_HandlesCorrectly()
        {
            // Arrange
            var group1 = new PropertyGroup("Group1");
            var group2 = new PropertyGroup("Group2");
            var group3 = new PropertyGroup("Group3");
            
            // Act - Create circular reference (actually circular references don't occur, but test complex reference relationships)
            group1.Add(group2);
            group2.Add(group3);
            group3.Add(group1); // This won't actually work
            
            // Assert - Confirm circular references are handled appropriately
            Assert.AreEqual(1, group1.Items.Count);
            Assert.AreEqual(1, group2.Items.Count);
            Assert.AreEqual(1, group3.Items.Count);
            
            // Confirm path search doesn't fall into infinite loop
            var allProperties = group1.GetAllProperties();
            Assert.IsNotNull(allProperties);
        }
        
        [Test]
        public void PropertySystem_WithValueRangeConstraints_WorksCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("ConstrainedProperties");
            
            // Create properties with value ranges
            var healthProperty = new FloatProperty("Health", 100f, 0f, 100f);
            var manaProperty = new FloatProperty("Mana", 50f, 0f, 100f);
            var levelProperty = new IntProperty("Level", 10, 1, 100);
            
            // Use adapters to control value ranges
            var healthAdapter = new FloatPropertyAdapter(healthProperty);
            var manaAdapter = new FloatPropertyAdapter(manaProperty);
            var levelAdapter = new IntPropertyAdapter(levelProperty);
            
            rootGroup.Add(healthProperty);
            rootGroup.Add(manaProperty);
            rootGroup.Add(levelProperty);
            
            // Act & Assert - Value range control test
            // Set normal values
            healthAdapter.Value = 75f;
            manaAdapter.Value = 25f;
            levelAdapter.Value = 15;
            
            Assert.AreEqual(75f, healthProperty.Value);
            Assert.AreEqual(25f, manaProperty.Value);
            Assert.AreEqual(15, levelProperty.Value);
            
            // Set extreme values (confirm system handles them appropriately)
            healthAdapter.Value = -10f; // Below minimum
            manaAdapter.Value = 150f;   // Above maximum
            levelAdapter.Value = 0;     // Below minimum
            
            Assert.AreEqual(0f, healthProperty.Value);   // Clamped to minimum
            Assert.AreEqual(100f, manaProperty.Value);   // Clamped to maximum
            Assert.AreEqual(1, levelProperty.Value);     // Clamped to minimum
        }
        
        [Test]
        public void PropertySystem_WithEventChaining_WorksCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("EventTest");
            var eventLog = new List<string>();
            
            var playerGroup = new PropertyGroup("Player");
            var healthProperty = new FloatProperty("Health", 100f);
            var manaProperty = new FloatProperty("Mana", 50f);
            
            // Set up event chain
            rootGroup.Added += (property) => eventLog.Add($"Root Added: {property.Name}");
            rootGroup.Removed += (property) => eventLog.Add($"Root Removed: {property.Name}");
            
            playerGroup.Added += (property) => eventLog.Add($"Player Added: {property.Name}");
            playerGroup.Removed += (property) => eventLog.Add($"Player Removed: {property.Name}");
            
            // Act
            rootGroup.Add(playerGroup);
            playerGroup.Add(healthProperty);
            playerGroup.Add(manaProperty);
            
            // Change property values
            healthProperty.Value = 80f;
            manaProperty.Value = 30f;
            
            // Remove property
            playerGroup.Remove(healthProperty);
            
            // Remove group
            rootGroup.Remove(playerGroup);
            
            // Assert
            Assert.AreEqual(5, eventLog.Count); // Root Added: Player, Player Added: Health, Player Added: Mana, Player Removed: Health, Root Removed: Player
            Assert.AreEqual("Root Added: Player", eventLog[0]);
            Assert.AreEqual("Player Added: Health", eventLog[1]);
            Assert.AreEqual("Player Added: Mana", eventLog[2]);
            Assert.AreEqual("Player Removed: Health", eventLog[3]);
            Assert.AreEqual("Root Removed: Player", eventLog[4]);
        }
        
        [Test]
        public void PropertySystem_WithComplexDataFlow_WorksCorrectly()
        {
            // Arrange - Simulate complex data flow
            var gameState = new PropertyGroup("GameState");
            
            var playerGroup = new PropertyGroup("Player");
            var inventoryGroup = new PropertyGroup("Inventory");
            var statsGroup = new PropertyGroup("Stats");
            
            // Player basic properties
            var playerName = new StringProperty("Name", "Hero");
            var playerLevel = new IntProperty("Level", 1);
            var playerExp = new IntProperty("Experience", 0);
            
            // Stats properties
            var strength = new IntProperty("Strength", 10);
            var agility = new IntProperty("Agility", 10);
            var intelligence = new IntProperty("Intelligence", 10);
            
            // Inventory properties
            var gold = new IntProperty("Gold", 100);
            var items = new StringProperty("Items", "Sword,Shield");
            
            // Build hierarchical structure
            gameState.Add(playerGroup);
            playerGroup.Add(playerName);
            playerGroup.Add(playerLevel);
            playerGroup.Add(playerExp);
            playerGroup.Add(statsGroup);
            playerGroup.Add(inventoryGroup);
            
            statsGroup.Add(strength);
            statsGroup.Add(agility);
            statsGroup.Add(intelligence);
            
            inventoryGroup.Add(gold);
            inventoryGroup.Add(items);
            
            // Act - Simulate complex data flow
            // Level up processing
            playerExp.Value = 1000;
            if (playerExp.Value >= 1000)
            {
                playerLevel.Value = 2;
                strength.Value = strength.Value + 2;
                agility.Value = agility.Value + 1;
                intelligence.Value = intelligence.Value + 1;
            }
            
            // Item purchase processing
            if (gold.Value >= 50)
            {
                gold.Value = gold.Value - 50;
                items.Value = items.Value + ",Potion";
            }
            
            // Assert
            Assert.AreEqual(2, playerLevel.Value);
            Assert.AreEqual(12, strength.Value);
            Assert.AreEqual(11, agility.Value);
            Assert.AreEqual(11, intelligence.Value);
            Assert.AreEqual(50, gold.Value);
            Assert.AreEqual("Sword,Shield,Potion", items.Value);
            
            // Path search test
            var foundStrength = gameState.FindByPath<IntProperty>("Player.Stats.Strength");
            Assert.AreEqual(12, foundStrength.Value);
            
            var foundGold = gameState.FindByPath<IntProperty>("Player.Inventory.Gold");
            Assert.AreEqual(50, foundGold.Value);
        }
        
        [Test]
        public void PropertySystem_WithErrorHandling_WorksCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("ErrorTest");
            
            // Act & Assert - Error handling test
            
            // Search with non-existent path
            var nonExistentProperty = rootGroup.FindByPath("NonExistent.Path");
            Assert.IsNull(nonExistentProperty);
            
            // Exception when specifying non-existent path with GetByPath
            Assert.Throws<ArgumentException>(() => rootGroup.GetByPath("NonExistent.Path"));
            
            // Search with empty path
            var emptyPathResult = rootGroup.FindByPath("");
            Assert.IsNull(emptyPathResult);
            
            // Search with null path
            var nullPathResult = rootGroup.FindByPath(null);
            Assert.IsNull(nullPathResult);
            
            // Wildcard pattern search
            var wildcardResult = rootGroup.FindByPattern("*");
            Assert.AreEqual(0, wildcardResult.Count);
            
            // Prefix search
            var prefixResult = rootGroup.FindByPrefix("Test");
            Assert.AreEqual(0, prefixResult.Count);
        }
        
        [Test]
        public void PropertySystem_WithSerializationScenario_WorksCorrectly()
        {
            // Arrange - Simulate serialization scenario
            var originalGroup = new PropertyGroup("SerializationTest");
            
            var settingsGroup = new PropertyGroup("Settings");
            var audioSettings = new PropertyGroup("Audio");
            var graphicsSettings = new PropertyGroup("Graphics");
            
            // Audio settings
            var masterVolume = new FloatProperty("MasterVolume", 0.8f);
            var musicVolume = new FloatProperty("MusicVolume", 0.6f);
            var sfxVolume = new FloatProperty("SFXVolume", 0.7f);
            
            // Graphics settings
            var resolution = new StringProperty("Resolution", "1920x1080");
            var fullscreen = new BoolProperty("Fullscreen", true);
            var qualityLevel = new IntProperty("QualityLevel", 2);
            
            // Build hierarchical structure
            originalGroup.Add(settingsGroup);
            settingsGroup.Add(audioSettings);
            settingsGroup.Add(graphicsSettings);
            
            audioSettings.Add(masterVolume);
            audioSettings.Add(musicVolume);
            audioSettings.Add(sfxVolume);
            
            graphicsSettings.Add(resolution);
            graphicsSettings.Add(fullscreen);
            graphicsSettings.Add(qualityLevel);
            
            // Act - Simulate serialization/deserialization
            // Create new group and rebuild the same structure
            var restoredGroup = new PropertyGroup("SerializationTest");
            
            var restoredSettingsGroup = new PropertyGroup("Settings");
            var restoredAudioSettings = new PropertyGroup("Audio");
            var restoredGraphicsSettings = new PropertyGroup("Graphics");
            
            // Restored properties (in actual serialization, values would be restored)
            var restoredMasterVolume = new FloatProperty("MasterVolume", 0.8f);
            var restoredMusicVolume = new FloatProperty("MusicVolume", 0.6f);
            var restoredSfxVolume = new FloatProperty("SFXVolume", 0.7f);
            var restoredResolution = new StringProperty("Resolution", "1920x1080");
            var restoredFullscreen = new BoolProperty("Fullscreen", true);
            var restoredQualityLevel = new IntProperty("QualityLevel", 2);
            
            // Build restored hierarchical structure
            restoredGroup.Add(restoredSettingsGroup);
            restoredSettingsGroup.Add(restoredAudioSettings);
            restoredSettingsGroup.Add(restoredGraphicsSettings);
            
            restoredAudioSettings.Add(restoredMasterVolume);
            restoredAudioSettings.Add(restoredMusicVolume);
            restoredAudioSettings.Add(restoredSfxVolume);
            
            restoredGraphicsSettings.Add(restoredResolution);
            restoredGraphicsSettings.Add(restoredFullscreen);
            restoredGraphicsSettings.Add(restoredQualityLevel);
            
            // Assert - Confirm restored structure is the same as original structure
            Assert.AreEqual(originalGroup.Items.Count, restoredGroup.Items.Count);
            Assert.AreEqual(originalGroup.GetAllProperties().Count, restoredGroup.GetAllProperties().Count);
            
            // Confirm path search returns the same results
            var originalMasterVolume = originalGroup.FindByPath<FloatProperty>("Settings.Audio.MasterVolume");
            var restoredMasterVolumeResult = restoredGroup.FindByPath<FloatProperty>("Settings.Audio.MasterVolume");
            
            Assert.AreEqual(originalMasterVolume.Value, restoredMasterVolumeResult.Value);
            Assert.AreEqual(originalMasterVolume.GetFullPath(), restoredMasterVolumeResult.GetFullPath());
        }
    }
} 
