using UnityEngine;
using UnityEngine.UI;

public class SetActiveFalse : MonoBehaviour
{
    private Button _button;
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClosePopup);
    }
    private void ClosePopup()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
