namespace FindTheIdol.Utilities.Cursor
{
    public sealed class CursorStateModifier 
    {
        public static void ModifyCursorState(bool lockCursor)
        {
            if(lockCursor)
            {                                    
                UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else
            {
                UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Confined;
                UnityEngine.Cursor.visible = true;
            }
        }
    }
}
