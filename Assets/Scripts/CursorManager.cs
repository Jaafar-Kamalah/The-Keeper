using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D greenCursorTexture;
    [SerializeField] private Texture2D redCursorTexture;
    [SerializeField] private Texture2D menuCursorTexture;
    [SerializeField] private GameObject player;
    private bool canShoot;
    private Vector2 cursorHotspot;
    private Vector2 menuCursorHotspot;

    private void Start()
    {
        cursorHotspot = new Vector2(greenCursorTexture.width / 2, greenCursorTexture.height / 2);
        menuCursorHotspot = new Vector2(10, 6);
        HandleCursorColor();
    }

    private void Update()
    {
        HandleCursorColor();
    }

    private void HandleCursorColor()
    {
        if (PauseMenu.isPaused ||StartMenu.isMenu)
        {
            Cursor.SetCursor(menuCursorTexture, menuCursorHotspot, CursorMode.ForceSoftware);
        }
        else
        {
            canShoot = player.GetComponent<Movement>().canShoot;
            if (canShoot)
            {
                Cursor.SetCursor(greenCursorTexture, cursorHotspot, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(redCursorTexture, cursorHotspot, CursorMode.ForceSoftware);
            }
        }
    }
}
