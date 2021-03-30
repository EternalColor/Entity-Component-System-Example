using FindTheIdol.Utilities.Constants;
using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.Inputs
{
    [InternalBufferCapacity(GameRelatedConstants.InputBufferSize)]
    public struct InputData : IBufferElementData
    {
        public bool Jump;

        public bool PickupItem;

        public bool OpenCloseInventory;

        public float Horizontal;

        public float Vertical;

        public bool RunButtonIsPressed;

        public bool WeaponAttack;

        public bool ShieldBlock;

        public float MouseX;  

        public float MouseY;
    }
}
