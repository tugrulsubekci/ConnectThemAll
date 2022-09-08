using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int value;
    private SpriteRenderer _spriteRenderer => transform.GetChild(0).GetComponent<SpriteRenderer>();

    public void InitBlock(GameManager.BlockType blockType)
    {
        value = blockType.value;
        _spriteRenderer.color = blockType.color;
    }
}
