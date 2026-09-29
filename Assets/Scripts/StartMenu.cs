using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Animator menuPlayerAnim;
    public static bool isMenu = true;

    public void StartGame()
    {
        menuPlayerAnim.SetTrigger("shoot");
        Invoke("StartGameWithDelay", 1.1f);
    }
    private void StartGameWithDelay()
    {
        isMenu = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
