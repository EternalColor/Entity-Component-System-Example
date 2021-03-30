using FindTheIdol.Utilities.Cursor;

namespace FindTheIdol.MonoBehaviours.Cursor
{
    public class CursorItemTextureRenderer : UnityEngine.MonoBehaviour
    {
        private void OnGUI()
        {
            if(UnityEngine.Event.current.type.Equals(UnityEngine.EventType.Repaint))
            {   
                if(CursorInputWrapper.CursorItemTexture != null)
                {
                    UnityEngine.Graphics.DrawTexture
                    (
                        //Center the Texture around the mouse 
                        new UnityEngine.Rect
                        (
                            CursorInputWrapper.MouseCoordinateOnScreen.x - CursorInputWrapper.CursorItemTexture.width / 2, 
                            CursorInputWrapper.MouseCoordinateOnScreen.y - CursorInputWrapper.CursorItemTexture.height / 2,  
                            CursorInputWrapper.CursorItemTexture.width, 
                            CursorInputWrapper.CursorItemTexture.height
                        ),
                        CursorInputWrapper.CursorItemTexture
                    );
                }
            }
        }
    }
}
