using UnityEngine;

public class FreezeAxis : MonoBehaviour
{
    private Transform parentTranform;
    private void Start()
    {
        parentTranform = transform.parent;
    }
    void Update()
    {
        if (parentTranform != null)
        {
            if (parentTranform.localScale.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (parentTranform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
