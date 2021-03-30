using FindTheIdol.Components.Deletion;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.Deletion
{
    [BurstCompile]
    //all entities used by this system need to be synchronized. This represents a sync point
    [AlwaysSynchronizeSystem]
    public class DeletionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            this.Entities
            .WithAll<DeletionTag>()
            .ForEach((Entity entity) => 
            {
                this.EntityManager.DestroyEntity(entity);
            });
        }
    }
}
