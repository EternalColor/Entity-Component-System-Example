using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Players;
using FindTheIdol.Utilities.Cursor;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(InputClearSystem))]
    [BurstCompile]
    public class InventoryOpenCloseSystem : SystemBase
    {   
        //During this time its not possible to change the state of the menu (open/close it) -> otherwise the menu opens and closes too fast
        private const float TimeoutInterval = 0.2f;

        //Update upon first iteration
        private float currentTime = TimeoutInterval;

        protected override void OnUpdate()
        {
            this.currentTime += Time.DeltaTime;

            bool hasChanged = false;
            
            if(this.currentTime >= TimeoutInterval)
            {
                this.Entities
                .WithAll<PlayerTag>()
                .ForEach((DynamicBuffer<InputData> inputBuffer, ref PlayerInfo playerInfo) => 
                {
                    for(int i = 0; i < inputBuffer.Length; ++i)
                    {
                        InputData input = inputBuffer[i];

                        if(input.OpenCloseInventory)
                        {
                            //If its turned on and user presses the 0pen/Close input again, then close the menu
                            if(playerInfo.OpenInventoryFromInputData)
                            {
                                playerInfo.OpenInventoryFromInputData = false;
                                hasChanged = true;

                                //When closing the inventory lock the cursor
                                CursorStateModifier.ModifyCursorState(true);
                            }
                            //Else its turned off and user presses the 0pen/Close input again, then open the menu
                            else
                            {
                                playerInfo.OpenInventoryFromInputData = true;
                                hasChanged = true;

                                //As long as the inventory is open allow the cursor to be moved and visible
                                CursorStateModifier.ModifyCursorState(false);
                            }
                        }
                    }
                }).Run();
            }

            if(hasChanged)
            {
                this.currentTime = 0f;
            }
        }
    }
}