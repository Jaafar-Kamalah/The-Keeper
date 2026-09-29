using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource shootingSound;
    [SerializeField] private AudioSource jumpingSound;

    private void playShootingSound()
    {
        shootingSound.Play();
    }

    private void playJumpingSound()
    {
        jumpingSound.Play();
    }
}
