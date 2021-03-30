namespace FindTheIdol.MonoBehaviours.Obstacles
{
    public class ObstacleDTO 
    {
        public UnityEngine.Transform Transform;
        public UnityEngine.Mesh Mesh;
        public UnityEngine.Material[] Materials;    

        public UnityEngine.Bounds WorldSpaceBounds;

        public UnityEngine.BoxCollider OverrideBoxCollider;
    }
}