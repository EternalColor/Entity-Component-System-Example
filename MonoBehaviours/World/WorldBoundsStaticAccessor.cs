using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.World;
using FindTheIdol.Utilities.Attributes;
using Unity.Mathematics;

namespace FindTheIdol.MonoBehaviours.World
{
    //Use this as "proxy" so the world bound component can be read in multiple authoring scripts
    public class WorldBoundsStaticAccessor : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject Map;

        [UnityEngine.SerializeField]
        private float gridCellSize;

        [UnityEngine.SerializeField]
        private int2 gridSize;

        [Evil("In order to use the WorldBounds Component in multiple authoring scripts you have to make it static, just making it public will result in null reference exception")]
        public static WorldBounds WorldBounds;

        private void Awake() 
        {
            UnityEngine.Bounds mapBoundsWorldSpace = this.Map.GetComponent<UnityEngine.MeshRenderer>().bounds;

            WorldBounds = new WorldBounds 
            {
                //The world scale must be an integer value, or you will encounter floating point errors with the grid system
                XZGridSize = gridSize,
                //LEFT BOTTOM CORNER
                XZGridWorldSpaceMin = new int2
                (
                    //Only works with symmetrical shapes and assumes its centered at 0/0/0
                    (int)math.floor(-mapBoundsWorldSpace.extents.x),
                    (int)math.floor(-mapBoundsWorldSpace.extents.z)
                ),
                //TOP RIGHT CORNER
                XZGridWorldSpaceMax = new int2
                (
                    //Only works with symmetrical shapes
                    //Only works with symmetrical shapes and assumes its centered at 0/0/0
                    (int)math.floor(mapBoundsWorldSpace.extents.x),
                    (int)math.floor(mapBoundsWorldSpace.extents.z)  
                ),
                WorldSpaceCellSize = gridCellSize
            };
        }
    }
}
