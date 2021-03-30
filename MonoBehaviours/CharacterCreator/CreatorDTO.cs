using System;
using System.Collections.Generic;
using FindTheIdol.Components.Classes;
using FindTheIdol.Components.Players;
using FindTheIdol.MonoBehaviours.Items;

namespace FindTheIdol.MonoBehaviours.CharacterCreator
{
    [Serializable]
    public class CreatorDTO : UnityEngine.MonoBehaviour
    {
        public PlayerInfo ChosenPlayerInfo;

        public List<ItemDTO> StartItems;
    }
}