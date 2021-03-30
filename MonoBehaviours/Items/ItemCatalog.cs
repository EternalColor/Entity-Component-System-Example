namespace FindTheIdol.MonoBehaviours.Items
{
    public class ItemCatalog : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField]
        private ItemDTO[] premadeItems;

        [UnityEngine.SerializeField]
        private ItemDTO[] premadeWeapons;

        public ItemDTO[] PremadeItems => premadeItems;

        public ItemDTO[] PremadeWeapons => premadeWeapons;
    }
}