using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace FindTheIdol.Utilities.Physics.Collision
{
    public class CollisionHelper
    {  
        public static unsafe bool ColliderCast(float3 rayFrom, float3 rayTo, CollisionWorld collisionWorld, PhysicsCollider collider)
        {
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = (Collider*)collider.Value.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = rayFrom,
                End = rayTo
            };

            return collisionWorld.CastCollider(input);
        }

        public static unsafe bool ColliderCast(float3 rayFrom, float3 rayTo, quaternion rotation, CollisionWorld collisionWorld, PhysicsCollider collider)
        {
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = (Collider*)collider.Value.GetUnsafePtr(),
                Orientation = rotation,
                Start = rayFrom,
                End = rayTo
            };

            return collisionWorld.CastCollider(input);
        }
        
        public static unsafe Entity ColliderCast(float3 rayFrom, float3 rayTo, quaternion rotation, CollisionWorld collisionWorld, PhysicsCollider collider, PhysicsWorld physicsWorld)
        {
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = (Collider*)collider.Value.GetUnsafePtr(),
                Orientation = rotation,
                Start = rayFrom,
                End = rayTo
            };

            ColliderCastHit hit = new ColliderCastHit();

            if(collisionWorld.CastCollider(input, out hit))
            {
                return physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            }

            return Entity.Null;
        }

        public static unsafe bool ColliderCastAllTargets(float3 rayFrom, float3 rayTo, quaternion rotation, CollisionWorld collisionWorld, PhysicsCollider collider, ref NativeList<Entity> allHitEntities)
        {
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = (Collider*)collider.Value.GetUnsafePtr(),
                Orientation = rotation,
                Start = rayFrom,
                End = rayTo
            };
            
            NativeList<ColliderCastHit> colliderCastHits = new NativeList<ColliderCastHit>(Allocator.Temp);
            
            if(collisionWorld.CastCollider(input, ref colliderCastHits))
            {
                //Extract entities
                for(int i = 0; i < colliderCastHits.Length; ++i)
                {
                    allHitEntities.Add(colliderCastHits[i].Entity);
                }

                return true;
            }

            return false;
        }

        public static unsafe bool BoxCast(float3 rayFrom, float3 rayTo, CollisionWorld collisionWorld, BoxGeometry boxGeometry, CollisionFilter collisionFilter)
        {
            BlobAssetReference<Collider> boxCollider = BoxCollider.Create(boxGeometry, collisionFilter);

            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = (Collider*)boxCollider.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = rayFrom,
                End = rayTo
            };

            return collisionWorld.CastCollider(input);
        }
    }
}