using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Buttons;
using FindTheIdol.Components.Classes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.Weapons;
using FindTheIdol.MonoBehaviours.Items;
using FindTheIdol.Utilities.Constants;
using FindTheIdol.Utilities.Cursor;
using UnityEngine.SceneManagement;

namespace FindTheIdol.MonoBehaviours.CharacterCreator
{
    public class CharacterCreatorButtonHandler : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject ClassSelectionPanelGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject CharacterStatisticsPanelGameObject;    

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button warriorClassButton;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text warriorClassLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button rangerClassButton;

         [UnityEngine.SerializeField]
        private UnityEngine.UI.Text rangeClassLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button mageClassButton;

         [UnityEngine.SerializeField]
        private UnityEngine.UI.Text mageClassLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button forsakenClassButton;
        
        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text forsakenClassLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text strengthValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] courageLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text dexterityValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] agilityLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text constitutionValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] toughnessLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text intelligenceValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] intelligenceLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text willpowerValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] willpowerLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text craftingValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button[] craftingLowerRaiseButtons;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button characterStatisticsDoneButton;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Button characterStatisticsBackButton;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text pointsLeftValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text healthValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text manaValueLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject pointsLeftWarningLabelGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.RawImage chosenClassRawImage;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text chosenClassTypeLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject creatorDTOGameobject;

        [UnityEngine.SerializeField]
        private UnityEngine.UI.Text startingItemsListText;

        [UnityEngine.SerializeField]
        private ItemCatalog itemCatalog;

        private CreatorDTO creatorDTO;

        private int pointsLeft;

        private void Start()
        {
            //Give the creator dto to the next scene
            DontDestroyOnLoad(this.creatorDTOGameobject);
            this.creatorDTO = this.creatorDTOGameobject.GetComponent<CreatorDTO>();

            //Start the Character Creator in the Class Selection
            this.GoToFirstPanel();

            this.warriorClassButton.onClick.AddListener(delegate { HandleClassButton(ClassType.Warrior); });
            this.rangerClassButton.onClick.AddListener(delegate { HandleClassButton(ClassType.Ranger); });
            this.mageClassButton.onClick.AddListener(delegate { HandleClassButton(ClassType.Mage); });
            this.forsakenClassButton.onClick.AddListener(delegate { HandleClassButton(ClassType.Forsaken); });

            //Index 0 is the lower button, index 1 is raise button
            this.courageLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Courage); });
            this.courageLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Courage); });

            this.agilityLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Agility); });
            this.agilityLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Agility); });

            this.toughnessLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Toughness); });
            this.toughnessLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Toughness); });

            this.intelligenceLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Intelligence); });
            this.intelligenceLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Intelligence); });

            this.willpowerLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Willpower); });
            this.willpowerLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Willpower); });

            this.craftingLowerRaiseButtons[0].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Lower, ref this.creatorDTO.ChosenPlayerInfo.Crafting); });
            this.craftingLowerRaiseButtons[1].onClick.AddListener(delegate { HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType.Raise, ref this.creatorDTO.ChosenPlayerInfo.Crafting); });

            this.characterStatisticsBackButton.onClick.AddListener(GoToFirstPanel);

            this.characterStatisticsDoneButton.onClick.AddListener(HandleCharacterStatisticsDoneButton);
        }

        private void OnGUI() 
        {
            this.healthValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Health.ToString();
            this.manaValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Mana.ToString();

            this.pointsLeftValueLabel.text = this.pointsLeft.ToString();

            this.strengthValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Courage.ToString();
            this.dexterityValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Agility.ToString();
            this.constitutionValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Toughness.ToString();
            this.intelligenceValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Intelligence.ToString();
            this.willpowerValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Willpower.ToString();
            this.craftingValueLabel.text = this.creatorDTO.ChosenPlayerInfo.Crafting.ToString();
        }

        private void HandleClassButton(ClassType classType)
        {
            this.creatorDTO.ChosenPlayerInfo.ClassType = classType;

            switch(this.creatorDTO.ChosenPlayerInfo.ClassType)
            {
                case ClassType.Warrior:
                    this.pointsLeft = GameRelatedConstants.WarriorPointsLeft;
                    this.chosenClassRawImage.texture = this.warriorClassButton.GetComponent<UnityEngine.UI.Image>().mainTexture;
                    this.chosenClassTypeLabel.text = this.warriorClassLabel.text;
                    this.CopyPresetInfoToCreatorDTO(in GameRelatedConstants.PresetWarriorStatsInfo);
                    this.CopyPresetItemsToCreatorDTO(in GameRelatedConstants.PresetWarriorItemTypes, in GameRelatedConstants.PresetWarriorWeaponTypes, in GameRelatedConstants.PresetWarriorShieldTypes);        
                    break;
                
                case ClassType.Ranger:
                    this.pointsLeft = GameRelatedConstants.RangerPointsLeft;
                    this.chosenClassRawImage.texture = this.rangerClassButton.GetComponent<UnityEngine.UI.Image>().mainTexture;
                    this.chosenClassTypeLabel.text = this.rangeClassLabel.text;
                    this.CopyPresetInfoToCreatorDTO(in GameRelatedConstants.PresetRangerStatsInfo);
                    this.CopyPresetItemsToCreatorDTO(in GameRelatedConstants.PresetRangerItemTypes, in GameRelatedConstants.PresetRangerWeaponTypes, in GameRelatedConstants.PresetRangerShieldTypes);  
                    break;

                case ClassType.Mage:
                    this.pointsLeft = GameRelatedConstants.MagePointsLeft;
                    this.chosenClassRawImage.texture = this.mageClassButton.GetComponent<UnityEngine.UI.Image>().mainTexture;
                    this.chosenClassTypeLabel.text = this.mageClassLabel.text;
                    this.CopyPresetInfoToCreatorDTO(in GameRelatedConstants.PresetMageStatsInfo);
                    this.CopyPresetItemsToCreatorDTO(GameRelatedConstants.PresetMageItemTypes, in GameRelatedConstants.PresetMageWeaponTypes, in GameRelatedConstants.PresetMageShieldTypes);  
                    break;

                case ClassType.Forsaken:
                    this.pointsLeft = GameRelatedConstants.ForsakenPointsLeft;
                    this.chosenClassRawImage.texture = this.forsakenClassButton.GetComponent<UnityEngine.UI.Image>().mainTexture;
                    this.chosenClassTypeLabel.text = this.forsakenClassLabel.text;
                    this.CopyPresetInfoToCreatorDTO(in GameRelatedConstants.PresetForsakenStatsInfo);
                    this.CopyPresetItemsToCreatorDTO(in GameRelatedConstants.PresetForsakenItemTypes, in GameRelatedConstants.PresetForsakenWeaponTypes, in GameRelatedConstants.PresetForsakenShieldTypes);  
                    break;
                //No default case because we compare to a enum
            }

            //Proceed to character statistics panel
            this.ClassSelectionPanelGameObject.SetActive(false);
            this.CharacterStatisticsPanelGameObject.SetActive(true);
        }

        private void HandleCharacterStatisticsLowerRaiseButton(CharacterStatisticsLowerRaiseButtonType buttonType, ref int playerStat)
        {
            switch(buttonType)
            {
                case CharacterStatisticsLowerRaiseButtonType.Lower:
                    if(playerStat - 1 >= GameRelatedConstants.MinStatValue)
                    {
                        ++this.pointsLeft;
                        --playerStat;
                    }
                    break;
                case CharacterStatisticsLowerRaiseButtonType.Raise:
                    if(this.pointsLeft > 0 && playerStat + 1 <= GameRelatedConstants.MaxStatValue)
                    {
                        --this.pointsLeft;
                        ++playerStat;
                    }
                    break;

                //No default case because we compare to a enum
            }
        }

        private void HandleCharacterStatisticsDoneButton()
        {
            if(this.pointsLeft == 0)
            {
                SceneManager.LoadScene("AutumnForest");
                CursorStateModifier.ModifyCursorState(true);
            }
            else
            {
                this.pointsLeftWarningLabelGameObject.SetActive(true);
            }
        }

        private void GoToFirstPanel()
        {
            this.ClassSelectionPanelGameObject.SetActive(true);
            this.CharacterStatisticsPanelGameObject.SetActive(false);

            //Reset creator dto
            this.creatorDTO.ChosenPlayerInfo = default;
            this.creatorDTO.StartItems = default;
            
            this.startingItemsListText.text = string.Empty;

            this.pointsLeftWarningLabelGameObject.SetActive(false);
        }

        private void CopyPresetInfoToCreatorDTO(in PlayerInfo presetInfo)
        {
            this.creatorDTO.ChosenPlayerInfo.Health = presetInfo.Health;
            this.creatorDTO.ChosenPlayerInfo.Mana = presetInfo.Mana;
            this.creatorDTO.ChosenPlayerInfo.Level = presetInfo.Level;
            this.creatorDTO.ChosenPlayerInfo.Courage = presetInfo.Courage;
            this.creatorDTO.ChosenPlayerInfo.Agility = presetInfo.Agility;
            this.creatorDTO.ChosenPlayerInfo.Toughness = presetInfo.Toughness;
            this.creatorDTO.ChosenPlayerInfo.Intelligence = presetInfo.Intelligence;
            this.creatorDTO.ChosenPlayerInfo.Willpower = presetInfo.Willpower;
            this.creatorDTO.ChosenPlayerInfo.Crafting = presetInfo.Crafting;
        }

        private void CopyPresetItemsToCreatorDTO(in ItemType[] presetItemTypes, in WeaponType[] presetWeaponTypes, in ShieldType[] presetShieldTypes)
        {
            if(this.creatorDTO.StartItems == null)
            {
                this.creatorDTO.StartItems = new List<ItemDTO>();
            }

            this.startingItemsListText.text += $"Items: ";
            foreach(ItemType itemType in presetItemTypes)
            {
                ItemDTO item = this.itemCatalog.PremadeItems.Where(itm => itm.ItemInfo.PreservedItemType == itemType).First();
                this.startingItemsListText.text += $"{item.ItemInfo.PreservedItemType.ToString()}, ";
                this.creatorDTO.StartItems.Add(item);
            }

            this.startingItemsListText.text += $"Weapons: ";
            foreach(WeaponType weaponType in presetWeaponTypes)
            {
                ItemDTO weapon = this.itemCatalog.PremadeWeapons.Where(wpn => wpn.ItemInfo.PreservedWeaponType == weaponType).First();
                this.startingItemsListText.text += $"{weapon.ItemInfo.PreservedWeaponType.ToString()}, ";
                this.creatorDTO.StartItems.Add(weapon);
            }

            this.startingItemsListText.text += $"Shields: ";
            foreach(ShieldType shieldType in presetShieldTypes)
            {
                ItemDTO shield = this.itemCatalog.PremadeWeapons.Where(wpn => wpn.ItemInfo.PreservedShieldType == shieldType).First();
                this.startingItemsListText.text += $"{shield.ItemInfo.PreservedShieldType.ToString()}, ";
                this.creatorDTO.StartItems.Add(shield);
            }

            //Remove the last ", " symbol
            this.startingItemsListText.text = this.startingItemsListText.text.Substring(0, this.startingItemsListText.text.LastIndexOf(", "));
        }
    }
}
