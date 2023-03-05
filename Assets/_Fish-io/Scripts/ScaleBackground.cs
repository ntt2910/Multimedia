using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBackground : MonoBehaviour
{
    [SerializeField] Camera cam;
    public float scaleValue = 2f;
    public SpriteRenderer spriteRenderer;
    
    private void LateUpdate()
    {
        float height = cam.orthographicSize *scaleValue;
        float width = height * Screen.width / Screen.height; // basically height * screen aspect ratio

        Sprite s = spriteRenderer.sprite;
        float unitWidth = s.textureRect.width / s.pixelsPerUnit;
        float unitHeight = s.textureRect.height / s.pixelsPerUnit;

        spriteRenderer.transform.localScale = new Vector3(width / unitWidth, height / unitHeight);
    }
}
