using DG.Tweening;
using UnityEngine;

public class NoMoreAnim : MonoBehaviour
{
    [SerializeField] private float shakeStrength; 
    private void OnEnable()
    {
        transform.DOShakePosition(2,shakeStrength).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    private void OnDisable()
    {
        transform.DOKill();
    }
}
