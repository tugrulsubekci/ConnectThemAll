using UnityEngine;
using DG.Tweening;

public class Node : MonoBehaviour
{
    public Block occupiedBlock;
    public Vector2 Pos => transform.position;

    private Transform _trasform => transform;

    public void MoveTo(Vector3 deltaMove)
    {
        _trasform.localPosition += deltaMove;
    }
}
