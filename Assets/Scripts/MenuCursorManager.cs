using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D menuCursorTexture;
    private Vector2 menuCursorHotspot;

    private void Start()
    {
        menuCursorHotspot = new Vector2(10, 6);
    }

    private void Update()
    {
        Cursor.SetCursor(menuCursorTexture, menuCursorHotspot, CursorMode.ForceSoftware);
    }
}
