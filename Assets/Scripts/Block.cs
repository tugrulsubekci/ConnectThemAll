using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;
    [SerializeField] public SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _valueText;
    public Node occupiedNode;

    private Transform _transform => transform;
    public Vector2 Pos => transform.position;
    public Color color => _spriteRenderer.color;
    public Transform _trasform => transform;

    public void Init(BlockType blockType)
    {
        value = blockType.value;
        _spriteRenderer.color = blockType.color;
        _valueText.text = blockType.value.ToString();
    }
}
