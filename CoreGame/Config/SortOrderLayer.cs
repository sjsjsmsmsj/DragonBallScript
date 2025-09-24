using UnityEngine;

public class SortOrderLayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void LateUpdate()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
    }
}