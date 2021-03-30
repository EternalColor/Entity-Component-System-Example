using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Items;
using FindTheIdol.MonoBehaviours.Items;
using FindTheIdol.Utilities.Cursor;
using FindTheIdol.Utilities.LayerMask;

namespace FindTheIdol.MonoBehaviours.UI
{
    //At the moment only 1 object of 1 type can be injected so have to use this proxy
    public class HUDInjectorProxy : UnityEngine.MonoBehaviour
    {
        public UnityEngine.UI.Text HealthValueText { get; set; }

        public UnityEngine.UI.Text ManaValueText  { get; set; }

        public UnityEngine.UI.Text LevelValueText  { get; set; }

        public ItemSlotCollectionDTO ItemSlotCollectionDTO { get; set; } 

        public ItemSlotCollectionDTO WeaponSlotCollectionDTO { get; set; }

        public ItemSlotCollectionDTO ArmorSlotCollectionDTO { get; set; }

        public ItemSlotCollectionDTO ShieldSlotCollectionDTO { get; set; }

        public ItemSlotCollectionDTO ArrowSlotCollectionDTO { get; set; }

        public ItemSlotCollectionDTO RingSlotCollectionDTO { get; set; }

        public UnityEngine.MeshFilter ItemSlotMeshFilter { get; set; }

        public UnityEngine.MeshRenderer ItemSlotMeshRenderer { get; set; }

        public UnityEngine.Camera ItemSlotCamera { get; set; }

        public UnityEngine.RectTransform ItemSlotMeshRendererRectTransform { get; set; }

        public UnityEngine.GameObject InventorySlotHolderGameObject { get => this.inventorySlotHolderGameObject; set => this.inventorySlotHolderGameObject = value; }

        public UnityEngine.GameObject CharacterSlotHolderGameObject { get => this.characterSlotHolderGameObject; set => this.characterSlotHolderGameObject = value; }
        
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject inventorySlotHolderGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject characterSlotHolderGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject healthValueGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject manaValueGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject levelValueGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject itemSlotCameraGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject itemSlotMeshRendererGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject itemSlotMeshFilterGameObject;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] itemSlotGameObjects;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] weaponSlotGameObjects;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] armorSlotGameObjects;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] shieldSlotGameObjects;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] arrowSlotGameObjects;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject[] ringSlotGameObjects;

        [UnityEngine.SerializeField]
        private ItemSlotButtonHandler itemSlotButtonHandler;

        [UnityEngine.SerializeField]
        private UnityEngine.Camera mainCamera;

        private void Start() 
        {
            //Get the UI components and "inject" them into the ecs (UI Systems) so we can control them there
            this.HealthValueText = this.healthValueGameObject.GetComponent<UnityEngine.UI.Text>();
            this.ManaValueText = this.manaValueGameObject.GetComponent<UnityEngine.UI.Text>();
            this.LevelValueText = this.levelValueGameObject.GetComponent<UnityEngine.UI.Text>();    

            //Draw item infront of camera and generate render texture assigned to raw image in ui
            this.ItemSlotCollectionDTO = new ItemSlotCollectionDTO
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.itemSlotGameObjects.Length; ++i)
            {
                //Item slots that are always visible
                this.ItemSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.itemSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.itemSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler = new ItemSlotButtonHandler();

            //Add ItemSlotButtonHandler to all item slot buttons
            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Item, this.ItemSlotCollectionDTO);

            this.ItemSlotMeshFilter = this.itemSlotMeshFilterGameObject.GetComponent<UnityEngine.MeshFilter>();
            this.ItemSlotMeshRenderer = this.itemSlotMeshRendererGameObject.GetComponent<UnityEngine.MeshRenderer>();
            this.ItemSlotMeshRendererRectTransform = this.itemSlotMeshRendererGameObject.GetComponent<UnityEngine.RectTransform>();
            this.ItemSlotCamera = this.itemSlotCameraGameObject.GetComponent<UnityEngine.Camera>();

            //Disable the camera and only enable it to take "screenshots" of the items
            this.ItemSlotCamera.enabled = false;

            //Setup Character slots
            this.WeaponSlotCollectionDTO = new ItemSlotCollectionDTO()
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.weaponSlotGameObjects.Length; ++i)
            {
                this.WeaponSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.weaponSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.weaponSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Weapon, this.WeaponSlotCollectionDTO);

            this.ArmorSlotCollectionDTO = new ItemSlotCollectionDTO()
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.armorSlotGameObjects.Length; ++i)
            {
                this.ArmorSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.armorSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.armorSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Armor, this.ArmorSlotCollectionDTO);

            this.ShieldSlotCollectionDTO = new ItemSlotCollectionDTO()
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.shieldSlotGameObjects.Length; ++i)
            {
                this.ShieldSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.shieldSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.shieldSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Shield, this.ShieldSlotCollectionDTO);

            this.ArrowSlotCollectionDTO = new ItemSlotCollectionDTO()
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.arrowSlotGameObjects.Length; ++i)
            {
                this.ArrowSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.arrowSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.arrowSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Arrow, this.ArrowSlotCollectionDTO);

            this.RingSlotCollectionDTO = new ItemSlotCollectionDTO()
            {
                ItemSlotDTOs = new List<ItemSlotDTO>()
            };

            for(int i = 0; i < this.ringSlotGameObjects.Length; ++i)
            {
                this.RingSlotCollectionDTO.ItemSlotDTOs.Add(new ItemSlotDTO
                {
                    ItemSlotRawImage = this.ringSlotGameObjects[i].GetComponent<UnityEngine.UI.RawImage>(),
                    ItemSlotButton = this.ringSlotGameObjects[i].GetComponent<UnityEngine.UI.Button>(),
                    Index = i,
                    HasItem = false
                });
            }

            this.itemSlotButtonHandler.RegisterItemSlotCollectionDTO(ItemSlotType.Ring, this.RingSlotCollectionDTO);
        }

        private void OnDestroy() 
        {
            this.itemSlotButtonHandler.UnRegisterAllItemSlotCollectionDTO();
        }
    }
}
