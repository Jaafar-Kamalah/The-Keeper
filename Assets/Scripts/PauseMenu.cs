using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public static bool isPaused = false;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource pauseSound;
    [SerializeField] private AudioSource resumSound;
    [SerializeField] private AudioSource clickSound;
    private float soundDelay = 0.1f;

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        backgroundMusic.Pause();
        pauseSound.Play();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        backgroundMusic.Play();
        resumSound.Play();
    }

    public void RestartGame()
    {
        clickSound.Play();
        StartCoroutine(RestartGameWithDelay());
    }

    private IEnumerator RestartGameWithDelay()
    {
        yield return WaitForSoundDelay();
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        clickSound.Play();
        StartCoroutine(LoadMainMenuWithDelay());
    }
    private IEnumerator LoadMainMenuWithDelay()
    {
        yield return WaitForSoundDelay();
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        clickSound.Play();
        StartCoroutine(ExitGameWithDelay());
    }
    private IEnumerator ExitGameWithDelay()
    {
        yield return WaitForSoundDelay();
        Application.Quit();
    }

    private IEnumerator WaitForSoundDelay()
    {
        float elapsedTime = 0f;
        // Wait for the sound delay while keeping Time.timeScale at 0
        while (elapsedTime < soundDelay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
