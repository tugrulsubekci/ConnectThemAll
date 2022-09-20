using DG.Tweening;
using UnityEngine;

public class TapToPlayAnim : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(1.2f, 1).SetLoops(-1,LoopType.Yoyo);
    }
    private void OnDisable()
    {
        transform.DOKill();
    }
}
