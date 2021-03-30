using Unity.Entities;

namespace FindTheIdol.Components.GameState
{
    public struct GameStateComponent : IComponentData
    {
        public GameStateEnum GameStateEnum;
    }
}