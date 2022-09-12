using DG.Tweening;
using UnityEngine;

public class Tap : MonoBehaviour
{
    [HideInInspector]
    public Vector3 startPos = new Vector3(0.1f, -0.5f, 0);
    [SerializeField] private Vector3[] path = new Vector3[4];
    public Transform _transform => transform;
    public void MoveFirst()
    {
        _transform.DOMoveX(4.1f, 2).SetLoops(-1).From(startPos);
    }
    public void MoveSecond()
    {
        _transform.position = startPos;
        _transform.DOMove(path[0], 0.5f).OnComplete(() =>
        {
            _transform.DOMove(path[1], 0.5f).OnComplete(() =>
            {
                _transform.DOMove(path[2], 0.5f).OnComplete(() =>
                {
                    _transform.DOMove(path[3], 0.5f).OnComplete(() =>
                    {
                        MoveSecond();
                    });
                });
            });
        });
    }

    public void StopTapAnimation()
    {
        _transform.DOKill();
        gameObject.SetActive(false);
    }

    public void PlayTapAnimation(TutorialNumber tutorialNumber)
    {
        gameObject.SetActive(true);
        if(tutorialNumber == TutorialNumber.First)
        {
            MoveFirst();
        }
        else
        {
            MoveSecond();
        }
    }
}
public enum TutorialNumber
{
    First,
    Second
}