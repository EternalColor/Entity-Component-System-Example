using FindTheIdol.Components.Items;
using Unity.Mathematics;

namespace FindTheIdol.Utilities.Cursor
{
    public sealed class CursorInputWrapper
    {
        public static float MouseXNormalized => UnityEngine.Input.GetAxis("Mouse X");
        public static float MouseYNormalized => UnityEngine.Input.GetAxis("Mouse Y"); 

        public static float2 MouseCoordinateOnScreen => UnityEngine.Event.current.mousePosition;

        public static UnityEngine.Texture CursorItemTexture { get; set; }

        public static InventoryItem OriginalInventoryItem { get; set; }
    }
}
