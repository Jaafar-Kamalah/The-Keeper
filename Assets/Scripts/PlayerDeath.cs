using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    private Animator anim;
    private bool hasDied = false;
    [SerializeField] private AudioSource deathSound;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") && !hasDied)
        {
            PlayerDeath();
        }
    }
    private void PlayerDeath()
    {
        LockMovement();
        anim.SetTrigger("death");
        hasDied = true;
        deathSound.Play();
    }

    private void LockMovement()
    {
        GetComponent<Movement>().enabled = false;

    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
