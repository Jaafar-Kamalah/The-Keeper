using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage scrollingImage;
    [SerializeField] private float horizontalScrolling, verticalScrolling;

    void Update()
    {
        scrollingImage.uvRect = new Rect(scrollingImage.uvRect.position + new Vector2(horizontalScrolling, verticalScrolling) * Time.deltaTime, scrollingImage.uvRect.size);
    }
}
