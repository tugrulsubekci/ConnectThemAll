using UnityEngine;
using UnityEngine.UI;

public class Cancel : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClosePopup);
    }
    private void ClosePopup()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
