using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Need to Migrate to a Proper place
public class CursorHandler : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    void Start()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(16, 16), CursorMode.Auto);
    }
    void Update()
    {
        
    }
}
