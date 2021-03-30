using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FindTheIdol.Components.Altar;
using FindTheIdol.Components.Animations;
using FindTheIdol.Components.GameState;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Music;
using FindTheIdol.Components.Obstacles;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Seeds;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.UI;
using FindTheIdol.Components.Weapons;
using FindTheIdol.Components.World;
using FindTheIdol.Utilities.Attributes;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.Components.Archetypes
{
    public static class ArchetypeExtensions
    {
        public static EntityArchetype CreatePlayerArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetPlayerComponentTypes(entityManager));
        }

        public static ComponentType GetPlayerIdentifier(this EntityManager entityManager)
        {
            return typeof(PlayerTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetPlayerComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetDynamicPhysicsTypes(entityManager))
                .Concat(new ComponentType[] { GetPlayerIdentifier(entityManager), typeof(PlayerInfo), typeof(PlayerBodyAnchorComponent) }).ToArray();
        }

        public static EntityArchetype CreateWorldBoundsArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetWorldBoundsComponentTypes(entityManager));
        }

        public static ComponentType GetWorldBoundsIdentifier(this EntityManager entityManager)
        {
            return typeof(WorldBounds);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetWorldBoundsComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetWorldBoundsIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateItemArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetItemComponentTypes(entityManager));
        }

        public static EntityArchetype CreateItemDirectlyToInventoryArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetItemDirectlyToInventoryIdentifierComponentTypes(entityManager));
        }

        public static ComponentType GetItemIdentifier(this EntityManager entityManager)
        {
            return typeof(ItemTag);
        }

        public static ComponentType[] GetPickupableItemIdentifier(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetItemIdentifier(entityManager), typeof(ItemPickupableTag) }).ToArray();
        }

        public static ComponentType GetPickupableItemIdentifierFlat(this EntityManager entityManager)
        {
            return typeof(ItemPickupableTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetItemComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(new ComponentType[] { GetItemIdentifier(entityManager), typeof(ItemInfo) }).ToArray();
        }

        public static ComponentType GetItemDirectlyToInventoryIdentifierFlat(this EntityManager entityManager)
        {
            return typeof(ItemDirectlyToInventoryTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetItemDirectlyToInventoryIdentifierComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetRenderableTypes(entityManager))
                .Concat(new ComponentType[] { GetItemDirectlyToInventoryIdentifierFlat(entityManager), typeof(ItemInfo) }).ToArray();
        }

        public static EntityArchetype CreateMonsterArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetMonsterComponentTypes(entityManager));
        }

        public static ComponentType GetMonsterIdentifier(this EntityManager entityManager)
        {
            return typeof(MonsterTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetMonsterComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetPathfindingTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(new ComponentType[] { GetMonsterIdentifier(entityManager), typeof(MonsterInfo) }).ToArray();
        }

        public static EntityArchetype CreateSeedArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetSeedComponentTypes(entityManager));
        }

        public static ComponentType GetSeedIdentifier(this EntityManager entityManager)
        {
            return typeof(Seed);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetSeedComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetSeedIdentifier(entityManager),  }).ToArray();
        }

        public static EntityArchetype CreatePathnodeArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetPathnodeComponentTypes(entityManager));
        }

        public static ComponentType GetPathnodeIdentifier(this EntityManager entityManager)
        {
            return typeof(PathNode);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetPathnodeComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetPathnodeIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateObstacleArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetObstacleComponentTypes(entityManager));
        }

        public static ComponentType GetObstacleIdentifier(this EntityManager entityManager)
        {
            return typeof(ObstacleTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetObstacleComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(new ComponentType[] { GetObstacleIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateUIInjectorArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetUIInjectorComponentTypes(entityManager));
        }

        public static ComponentType GetUIInjectorIdentifier(this EntityManager entityManager)
        {
            return typeof(ItemInfoWorldSpaceLabelInjectorTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetUIInjectorComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetObstacleIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateAltarArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetAltarComponentTypes(entityManager));
        }

        public static ComponentType GetAltarIdentifier(this EntityManager entityManager)
        {
            return typeof(AltarTag);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetAltarComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(new ComponentType[] { GetAltarIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateGameStateArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetGameStateComponentTypes(entityManager));
        }

        public static ComponentType GetGameStateIdentifier(this EntityManager entityManager)
        {
            return typeof(GameStateComponent);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetGameStateComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetGameStateIdentifier(entityManager) }).ToArray();
        }

        public static EntityArchetype CreateEquippedWeaponArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetEquippedWeaponComponentTypes(entityManager));
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetEquippedWeaponComponentTypes(this EntityManager entityManager)
        {
            return GetEquippedComponentTypes(entityManager)
                .Concat(new ComponentType[] { GetEquippedWeaponIdentifier(entityManager), typeof(ItemInfo) }).ToArray();
        }

        public static ComponentType GetEquippedWeaponIdentifier(this EntityManager entityManager)
        {
            return typeof(WeaponEquippedTag);
        }

        public static EntityArchetype CreateRangedAttackAnimationArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetRangedAttackAnimationComponentTypes(entityManager));
        }
        
        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetRangedAttackAnimationComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(GetDynamicPhysicsTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(new ComponentType [] { GetRangedAttackAnimationIdentifier(entityManager) }).ToArray();
        }

        public static ComponentType GetRangedAttackAnimationIdentifier(this EntityManager entityManager)
        {
            return typeof(ArrowAnimationTag);
        }

        public static EntityArchetype CreateMagicAttackAnimationArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetMagicAttackAnimationComponentTypes(entityManager));
        }
        
        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetMagicAttackAnimationComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(new ComponentType [] { GetMagicAttackAnimationIdentifier(entityManager) }).ToArray();
        }

        public static ComponentType GetMagicAttackAnimationIdentifier(this EntityManager entityManager)
        {
            return typeof(MagicAttackAnimationTag);
        }

        public static EntityArchetype CreateEquippedShieldArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetEquippedShieldComponentTypes(entityManager));
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetEquippedShieldComponentTypes(this EntityManager entityManager)
        {
            return GetEquippedComponentTypes(entityManager)
                .Concat(new ComponentType[] { GetEquippedShieldIdentifier(entityManager), typeof(ItemInfo) }).ToArray();
        }

        public static ComponentType GetEquippedShieldIdentifier(this EntityManager entityManager)
        {
            return typeof(ShieldEquippedTag);
        }

        private static List<ComponentType> GetEquippedComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(GetSpacialTypes(entityManager))
                .Concat(GetRenderableTypes(entityManager))
                .Concat(GetStaticPhysicsTypes(entityManager))
                .Concat(GetNonUniformScalingTypes(entityManager)).ToList();
        }

        public static EntityArchetype CreateSFXArchetype(this EntityManager entityManager)
        {
            return entityManager.CreateArchetype(GetSFXComponentTypes(entityManager));
        }

        public static ComponentType GetSFXIdentifier(this EntityManager entityManager)
        {
            return typeof(SFXComponent);
        }

        [Evil("Please dont use this in ECS Systems unless explicitly needed, this is intended for serialization! This creates overhead due to loading all Components associated with Archetype!")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentType[] GetSFXComponentTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
                .Concat(new ComponentType[] { GetSFXIdentifier(entityManager) }).ToArray();
        }


        public static List<ComponentType> GetRenderableTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>() 
            {
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(WorldRenderBounds)
            };
        }

        public static List<ComponentType> GetSpacialTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>() 
            {
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Rotation)
            };
        }
        
        //Mainly used to scale entities after GameObject.Transform.lossyScale/localScale
        //Also used for irregular shape scaling (otherwise you can just use the scale(float) component instead of nonuniformscaling(float3))
        public static List<ComponentType> GetNonUniformScalingTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>() 
            {
                typeof(NonUniformScale)
            };
        }

        public static List<ComponentType> GetPathfindingTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
            {
                typeof(PathfindingParameters),
                typeof(PathFollow)
            };
        }

        public static List<ComponentType> GetStaticPhysicsTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
            {
                typeof(PhysicsCollider)
            };
        }

        public static List<ComponentType> GetDynamicPhysicsTypes(this EntityManager entityManager)
        {
            return new List<ComponentType>()
            {
                typeof(PhysicsCollider),
                typeof(PhysicsMass),
                typeof(PhysicsVelocity),
                typeof(PhysicsDamping)
            };
        }
    }
}
