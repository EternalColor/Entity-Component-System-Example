using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FindTheIdol.Utilities.Physics.Collision
{
    public sealed class CollisionGeomeotrySingleton
    {
        //Cache geometries (float3 = size, float3 = center)
        private IDictionary<Tuple<float3, float3>, BoxGeometry> boxGeometries;

        //Default method
        public BoxGeometry CreateOrGetBoxGeometry(float3 size)
        {
            lock(padlock)
            {
                if(this.boxGeometries == null)
                {
                    this.boxGeometries = new Dictionary<Tuple<float3,float3>, BoxGeometry>();
                }

                Tuple<float3, float3> key = new Tuple<float3, float3>(size, float3.zero);

                if(!this.boxGeometries.ContainsKey(key))
                {
                    this.boxGeometries.Add(key, new BoxGeometry { Size = key.Item1, Center = key.Item2, Orientation = quaternion.identity });
                }

                return this.boxGeometries[key];
            }
        }

        //Overload, sadly can not use default overload because float3.zero is not a compile-time constant
        public BoxGeometry CreateOrGetBoxGeometry(float3 size, float3 center)
        {
            lock(padlock)
            {
                if(this.boxGeometries == null)
                {
                    this.boxGeometries = new Dictionary<Tuple<float3,float3>, BoxGeometry>();
                }

                Tuple<float3, float3> key = new Tuple<float3, float3>(size, center);

                if(!this.boxGeometries.ContainsKey(key))
                {
                    this.boxGeometries.Add(key, new BoxGeometry { Size = key.Item1, Center = key.Item2, Orientation = quaternion.identity });
                }

                return this.boxGeometries[key];
            }
        }

        private CollisionGeomeotrySingleton()
        {
        }

        private static readonly object padlock = new object();

        private static CollisionGeomeotrySingleton instance = null;

        public static CollisionGeomeotrySingleton Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new CollisionGeomeotrySingleton();
                    }
                    
                    return instance;
                }
            }
        }
    }
}
