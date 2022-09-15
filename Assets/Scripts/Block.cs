using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;
    [SerializeField] public SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _valueText;
    [SerializeField] private Sprite flatSprite;
    [SerializeField] private Sprite otherSprite;
    public Node occupiedNode;
    public Vector2 Pos => transform.position;
    public Color color => _spriteRenderer.color;
    public Transform _trasform => transform;

    public void Init(BlockType blockType)
    {
        value = blockType.value;
        _spriteRenderer.color = blockType.color;
        _valueText.text = blockType.valueString;
        if(blockType.flat)
        {
            _spriteRenderer.sprite = flatSprite;
        }
        else
        {
            _spriteRenderer.sprite = otherSprite;
        }
    }
}
