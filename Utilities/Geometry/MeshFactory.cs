using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace FindTheIdol.Utilities.Geometry
{
    public sealed class MeshFactory 
    {
        //Cache cube meshes (float = xScale, float = yScale, float = zScale)
        private IDictionary<Tuple<float, float, float>, UnityEngine.Mesh> cachedCubeMeshes;

        private UnityEngine.Mesh cachedArrowMesh;

        public UnityEngine.Mesh GetOrCreateCube(float xScale, float yScale, float zScale)
        {
            lock(padlock)
            {
                if(this.cachedCubeMeshes == null)
                {
                    this.cachedCubeMeshes = new Dictionary<Tuple<float, float, float>, UnityEngine.Mesh>();
                }

                Tuple<float, float, float> key = new Tuple<float, float, float>(xScale, yScale, zScale);

                if(!this.cachedCubeMeshes.ContainsKey(key))
                {
                    //First create primitive geometry then apply scale
                    UnityEngine.Mesh cubeMesh = new UnityEngine.Mesh();
                    UnityEngine.Vector3[] vertices = 
                    {
                        new float3(0, yScale, 0),
                        new float3(0, 0, 0),
                        new float3(xScale, yScale, 0),
                        new float3(xScale, 0, 0),

                        new float3(0, 0, zScale),
                        new float3(xScale, 0, zScale),
                        new float3(0, yScale, zScale),
                        new float3(xScale, yScale, zScale),

                        new float3(0, yScale, 0),
                        new float3(xScale, yScale, 0),

                        new float3(0, yScale, 0),
                        new float3(0, yScale, zScale),

                        new float3(xScale, yScale, 0),
                        new float3(xScale, yScale, zScale),
                    };
                    cubeMesh.vertices = vertices;

                    int[] triangles = 
                    {
                        0, 2, 1, // front
                        1, 2, 3,
                        4, 5, 6, // back
                        5, 7, 6,
                        6, 7, 8, //top
                        7, 9 ,8, 
                        1, 3, 4, //bottom
                        3, 5, 4,
                        1, 11,10,// left
                        1, 4, 11,
                        3, 12, 5,//right
                        5, 12, 13
                    };
                    cubeMesh.triangles = triangles;

                    UnityEngine.Vector2[] uv = 
                    {
                        new float2(0, 0.66f),
                        new float2(0.25f, 0.66f),
                        new float2(0, 0.33f),
                        new float2(0.25f, 0.33f),

                        new float2(0.5f, 0.66f),
                        new float2(0.5f, 0.33f),
                        new float2(0.75f, 0.66f),
                        new float2(0.75f, 0.33f),

                        new float2(1, 0.66f),
                        new float2(1, 0.33f),

                        new float2(0.25f, 1),
                        new float2(0.5f, 1),

                        new float2(0.25f, 0),
                        new float2(0.5f, 0),
                    };	

                    cubeMesh.Optimize();
                    cubeMesh.RecalculateNormals();

                    cubeMesh.RecalculateBounds();

                    this.cachedCubeMeshes.Add(key, cubeMesh);
                }

                return this.cachedCubeMeshes[key];
            }
        }

        public UnityEngine.Mesh GetOrCreateArrowMesh()
        {
            lock(padlock)
            {
                if(this.cachedArrowMesh == null)
                {
                    this.cachedArrowMesh = UnityEngine.Resources.Load("Material/Weapons/Consumables/Arrow", typeof(UnityEngine.Mesh)) as UnityEngine.Mesh;
                }

                return this.cachedArrowMesh;
            }
        }

        private MeshFactory()
        {
        }

        private static readonly object padlock = new object();

        private static MeshFactory instance = null;

        public static MeshFactory Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new MeshFactory();
                    }
                    
                    return instance;
                }
            }
        }
    }
}