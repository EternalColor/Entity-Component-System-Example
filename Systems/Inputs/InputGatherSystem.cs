using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Utilities.Cursor;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.Inputs
{
    [BurstCompile]
    [UpdateBefore(typeof(InputClearSystem))]
    public class InputGatherSystem : ComponentSystem
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }

        protected override void OnUpdate()
        {
            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();

            //Get Player Input
            DynamicBuffer<InputData> inputBuffer = this.GetBufferFromEntity<InputData>()[playerEntity];

            InputData input = default;

            input.MouseX = CursorInputWrapper.MouseXNormalized;
            input.MouseY = CursorInputWrapper.MouseYNormalized;

            input.WeaponAttack = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Mouse0);
            input.ShieldBlock = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Mouse1);

            input.PickupItem = UnityEngine.Input.GetKey(UnityEngine.KeyCode.E);
            input.Jump = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space);
            input.OpenCloseInventory = UnityEngine.Input.GetKey(UnityEngine.KeyCode.F);

            input.Horizontal = UnityEngine.Input.GetAxis("Horizontal");
            input.Vertical = UnityEngine.Input.GetAxis("Vertical");
            input.RunButtonIsPressed = UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift);
            
            inputBuffer.Add(input);
        }
    }
}
