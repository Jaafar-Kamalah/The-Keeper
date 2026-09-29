using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ItemCollection : MonoBehaviour
{
    public int ammoCount = 0;

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private AudioSource ammoPickupSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Shell"))
        {
            ammoPickupSound.Play();
            Destroy(collision.gameObject);
            ammoCount++;
            UpdateshotgunAmmo();
        }
    }

    public void UpdateshotgunAmmo()
    {
        ammoText.text = ammoCount.ToString();
    }
}
