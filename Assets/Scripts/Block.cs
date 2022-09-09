using DG.Tweening;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _valueText;
    public Node occupiedNode;

    private Transform _transform => transform;
    public Vector2 Pos => transform.position;
    public Color color => _spriteRenderer.color;

    public void Init(BlockType blockType)
    {
        value = blockType.value;
        _spriteRenderer.color = blockType.color;
        _valueText.text = blockType.value.ToString();
    }
}
