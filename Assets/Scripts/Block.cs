using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int value;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _valueText;

    public void Init(TypeOfBlock blockType)
    {
        value = blockType.value;
        _spriteRenderer.color = blockType.color;
        _valueText.text = blockType.value.ToString();
    }
}
