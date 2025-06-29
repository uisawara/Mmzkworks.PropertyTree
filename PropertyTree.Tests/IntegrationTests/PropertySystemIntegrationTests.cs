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
    public class PropertySystemIntegrationTests
    {
        [Test]
        public void ComplexPropertyHierarchy_WithAllPropertyTypes_WorksCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("Root");
            
            // Create subgroups
            var playerGroup = new PropertyGroup("Player");
            var enemyGroup = new PropertyGroup("Enemy");
            var uiGroup = new PropertyGroup("UI");
            
            // Player-related properties
            var playerHealth = new FloatProperty("Health", 100f);
            var playerSpeed = new FloatProperty("Speed", 5.5f);
            var playerName = new StringProperty("Name", "Hero");
            var playerIsAlive = new BoolProperty("IsAlive", true);
            var playerLevel = new IntProperty("Level", 10);
            var playerAttack = new ActionProperty("Attack");
            
            // Enemy-related properties
            var enemyHealth = new FloatProperty("Health", 50f);
            var enemyType = new EnumProperty<EnemyType>("Type", EnemyType.Goblin);
            var enemyIsBoss = new BoolProperty("IsBoss", false);
            
            // UI-related properties
            var uiVisible = new BoolProperty("Visible", true);
            var uiScale = new FloatProperty("Scale", 1.0f);
            
            // Act - Build hierarchical structure
            rootGroup.Add(playerGroup);
            rootGroup.Add(enemyGroup);
            rootGroup.Add(uiGroup);
            
            playerGroup.Add(playerHealth);
            playerGroup.Add(playerSpeed);
            playerGroup.Add(playerName);
            playerGroup.Add(playerIsAlive);
            playerGroup.Add(playerLevel);
            playerGroup.Add(playerAttack);
            
            enemyGroup.Add(enemyHealth);
            enemyGroup.Add(enemyType);
            enemyGroup.Add(enemyIsBoss);
            
            uiGroup.Add(uiVisible);
            uiGroup.Add(uiScale);
            
            // Assert - Verify hierarchical structure
            Assert.AreEqual(3, rootGroup.Items.Count);
            Assert.AreEqual(6, playerGroup.Items.Count);
            Assert.AreEqual(3, enemyGroup.Items.Count);
            Assert.AreEqual(2, uiGroup.Items.Count);
            
            // Verify parent-child relationships
            Assert.AreEqual(rootGroup, playerGroup.Parent);
            Assert.AreEqual(playerGroup, playerHealth.Parent);
            Assert.AreEqual("Root.Player.Health", playerHealth.GetFullPath());
            Assert.AreEqual("Root.Enemy.Type", enemyType.GetFullPath());
            Assert.AreEqual("Root.UI.Scale", uiScale.GetFullPath());
        }
        
        [Test]
        public void PropertyExtensions_WithComplexHierarchy_WorkCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("Game");
            
            var combatGroup = new PropertyGroup("Combat");
            var playerGroup = new PropertyGroup("Player");
            var weaponGroup = new PropertyGroup("Weapon");
            
            var playerHealth = new FloatProperty("Health", 100f);
            var playerMana = new FloatProperty("Mana", 50f);
            var weaponDamage = new IntProperty("Damage", 25);
            var weaponDurability = new FloatProperty("Durability", 100f);
            
            // Build hierarchical structure
            rootGroup.Add(combatGroup);
            combatGroup.Add(playerGroup);
            combatGroup.Add(weaponGroup);
            
            playerGroup.Add(playerHealth);
            playerGroup.Add(playerMana);
            weaponGroup.Add(weaponDamage);
            weaponGroup.Add(weaponDurability);
            
            // Act & Assert - Path-based search test
            var foundHealth = rootGroup.FindByPath("Combat.Player.Health");
            Assert.AreEqual(playerHealth, foundHealth);
            
            var foundDamage = rootGroup.FindByPath<IntProperty>("Combat.Weapon.Damage");
            Assert.AreEqual(weaponDamage, foundDamage);
            
            // Wildcard search test
            var allCombatProperties = rootGroup.FindByPattern("Combat.*");
            Assert.AreEqual(2, allCombatProperties.Count); // Player, Weapon
            
            var allPlayerProperties = rootGroup.FindByPattern("Combat.Player.*");
            Assert.AreEqual(2, allPlayerProperties.Count); // Health, Mana
            
            // Prefix search test
            var combatPrefixProperties = rootGroup.FindByPrefix("Game.Combat");
            Assert.AreEqual(7, combatPrefixProperties.Count); // Combat, Player, Weapon, Health, Mana, Damage, Durability
            
            // Get all properties test
            var allProperties = rootGroup.GetAllProperties();
            Assert.AreEqual(7, allProperties.Count); // Game, Combat, Player, Health, Mana, Weapon, Damage, Durability
        }
        
        [Test]
        public void PropertyAdapters_WithValueProperties_WorkCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("Settings");
            
            var audioGroup = new PropertyGroup("Audio");
            var graphicsGroup = new PropertyGroup("Graphics");
            
            var masterVolume = new FloatProperty("MasterVolume", 0.8f);
            var musicVolume = new FloatProperty("MusicVolume", 0.6f);
            var sfxVolume = new FloatProperty("SFXVolume", 0.7f);
            var fullscreen = new BoolProperty("Fullscreen", true);
            var resolution = new StringProperty("Resolution", "1920x1080");
            var qualityLevel = new IntProperty("QualityLevel", 2);
            
            // Create adapters
            var masterVolumeAdapter = new FloatPropertyAdapter(masterVolume);
            var musicVolumeAdapter = new FloatPropertyAdapter(musicVolume);
            var sfxVolumeAdapter = new FloatPropertyAdapter(sfxVolume);
            var fullscreenAdapter = new BoolPropertyAdapter(fullscreen);
            var resolutionAdapter = new StringPropertyAdapter(resolution);
            var qualityLevelAdapter = new IntPropertyAdapter(qualityLevel);
            
            // Build hierarchical structure
            rootGroup.Add(audioGroup);
            rootGroup.Add(graphicsGroup);
            
            audioGroup.Add(masterVolume);
            audioGroup.Add(musicVolume);
            audioGroup.Add(sfxVolume);
            
            graphicsGroup.Add(fullscreen);
            graphicsGroup.Add(resolution);
            graphicsGroup.Add(qualityLevel);
            
            // Act & Assert - Adapter operation test
            Assert.AreEqual(0.8f, masterVolumeAdapter.Value);
            Assert.AreEqual(0.6f, musicVolumeAdapter.Value);
            Assert.AreEqual(true, fullscreenAdapter.Value);
            Assert.AreEqual("1920x1080", resolutionAdapter.Value);
            Assert.AreEqual(2, qualityLevelAdapter.Value);
            
            // Value change test
            masterVolumeAdapter.Value = 0.9f;
            Assert.AreEqual(0.9f, masterVolume.Value);
            
            fullscreenAdapter.Value = false;
            Assert.AreEqual(false, fullscreen.Value);
            
            resolutionAdapter.Value = "2560x1440";
            Assert.AreEqual("2560x1440", resolution.Value);
            
            qualityLevelAdapter.Value = 3;
            Assert.AreEqual(3, qualityLevel.Value);
        }
        
        [Test]
        public void PropertyEvents_WithComplexHierarchy_WorkCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("GameState");
            var addedProperties = new List<IProperty>();
            var removedProperties = new List<IProperty>();
            
            rootGroup.Added += (property) => addedProperties.Add(property);
            rootGroup.Removed += (property) => removedProperties.Add(property);
            
            var playerGroup = new PropertyGroup("Player");
            var playerHealth = new FloatProperty("Health", 100f);
            var playerMana = new FloatProperty("Mana", 50f);
            
            // Act
            rootGroup.Add(playerGroup);
            playerGroup.Add(playerHealth);
            playerGroup.Add(playerMana);
            
            // Change property values
            playerHealth.Value = 80f;
            playerMana.Value = 30f;
            
            // Remove property
            playerGroup.Remove(playerHealth);
            
            // Assert
            Assert.AreEqual(1, addedProperties.Count); // Only playerGroup
            Assert.AreEqual(0, removedProperties.Count); // No remove events fired on rootGroup
            Assert.AreEqual(playerGroup, addedProperties[0]);
            // playerHealth removal is monitored by playerGroup's event, not rootGroup
            
            // Value changes are correctly reflected
            Assert.AreEqual(80f, playerHealth.Value);
            Assert.AreEqual(30f, playerMana.Value);
        }
        
        [Test]
        public void PropertyGroup_WithDynamicModification_WorksCorrectly()
        {
            // Arrange
            var rootGroup = new PropertyGroup("DynamicTest");
            var group1 = new PropertyGroup("Group1");
            var group2 = new PropertyGroup("Group2");
            
            var prop1 = new IntProperty("Prop1", 1);
            var prop2 = new IntProperty("Prop2", 2);
            var prop3 = new IntProperty("Prop3", 3);
            
            // Act - Dynamic addition, removal, and moving
            rootGroup.Add(group1);
            group1.Add(prop1);
            group1.Add(prop2);
            
            // Move to group
            rootGroup.Add(group2);
            group1.Remove(prop1);
            group2.Add(prop1);
            
            // Add new property
            group2.Add(prop3);
            
            // Remove group
            rootGroup.Remove(group1);
            
            // Assert
            Assert.AreEqual(1, rootGroup.Items.Count); // Only group2
            Assert.AreEqual(2, group2.Items.Count); // prop1, prop3
            Assert.AreEqual(group2, prop1.Parent);
            Assert.AreEqual(group2, prop3.Parent);
            Assert.AreEqual(group1, prop2.Parent); // Even though group1 is removed, prop2 is not explicitly removed
        }
        
        [Test]
        public void PropertyExtensions_WithDeepNesting_WorkCorrectly()
        {
            // Arrange - Create deep nested structure
            var root = new PropertyGroup("Root");
            var level1 = new PropertyGroup("Level1");
            var level2 = new PropertyGroup("Level2");
            var level3 = new PropertyGroup("Level3");
            var level4 = new PropertyGroup("Level4");
            var level5 = new PropertyGroup("Level5");
            
            var deepProperty = new StringProperty("DeepProperty", "DeepValue");
            
            // Build deep nesting
            root.Add(level1);
            level1.Add(level2);
            level2.Add(level3);
            level3.Add(level4);
            level4.Add(level5);
            level5.Add(deepProperty);
            
            // Act & Assert
            Assert.AreEqual("Root.Level1.Level2.Level3.Level4.Level5.DeepProperty", deepProperty.GetFullPath());
            Assert.AreEqual(6, deepProperty.GetDepth());
            
            var parentChain = deepProperty.GetParentChain();
            Assert.AreEqual(6, parentChain.Length);
            Assert.AreEqual(root, parentChain[0]);
            Assert.AreEqual(level1, parentChain[1]);
            Assert.AreEqual(level2, parentChain[2]);
            Assert.AreEqual(level3, parentChain[3]);
            Assert.AreEqual(level4, parentChain[4]);
            Assert.AreEqual(level5, parentChain[5]);
            
            // Path search test
            var foundProperty = root.FindByPath("Level1.Level2.Level3.Level4.Level5.DeepProperty");
            Assert.AreEqual(deepProperty, foundProperty);
            
            // Wildcard search test
            var allLevel3Properties = root.FindByPattern("Level1.Level2.Level3.*");
            Assert.AreEqual(1, allLevel3Properties.Count); // Only level4
            
            // Get all properties test
            var allProperties = root.GetAllProperties();
            Assert.AreEqual(6, allProperties.Count); // Level1, Level2, Level3, Level4, Level5, DeepProperty
        }
        
        // Test enum type
        public enum EnemyType
        {
            Goblin,
            Orc,
            Dragon,
            Boss
        }
    }
} 
