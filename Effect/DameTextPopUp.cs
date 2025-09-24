using UnityEngine;
using TMPro;
public class DameTextPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPopUp;
    [SerializeField] private float lifeTime = 0.25f;
    [SerializeField] private float speedMove = 5f;
    [SerializeField] private string preChar;
    [SerializeField] private string postChar;
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    private void Update()
    {
        transform.Translate(Vector3.up * speedMove * Time.deltaTime);
    }
    public void SetUpTextPopUp(float value)
    {
        int valueToInt = Mathf.FloorToInt(value);
        textPopUp.text = preChar + valueToInt.ToString() + postChar;
    }
}
