using DG.Tweening;
using UnityEngine;

public class Tap : MonoBehaviour
{
    public Vector3 startPos = new Vector3(0.1f, -1.5f, 0);
    [SerializeField] private Vector3[] secondPath = new Vector3[4];
    [SerializeField] private Vector3[] thirdPath = new Vector3[4];
    public Transform _transform => transform;
    public void MoveFirst()
    {
        _transform.DOMoveX(4.1f, 2).SetLoops(-1).From(startPos);
    }
    public void MoveSecond()
    {
        _transform.position = startPos;
        _transform.DOMove(secondPath[0], 0.5f).OnComplete(() =>
        {
            _transform.DOMove(secondPath[1], 0.5f).OnComplete(() =>
            {
                _transform.DOMove(secondPath[2], 0.5f).OnComplete(() =>
                {
                    _transform.DOMove(secondPath[3], 0.5f).OnComplete(() =>
                    {
                        MoveSecond();
                    });
                });
            });
        });
    }
    public void MoveThird()
    {
        _transform.DOMove(thirdPath[0], 0f).OnComplete(() =>
        {
            _transform.DOMove(thirdPath[1], 1.25f).OnComplete(() =>
            {
                _transform.DOMove(thirdPath[2], 0.25f).OnComplete(() =>
                {
                    _transform.DOMove(thirdPath[3], 1.25f).OnComplete(() =>
                    {
                        MoveThird();
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
        else if(tutorialNumber == TutorialNumber.Second)
        {
            MoveSecond();
        }
        else
        {
            MoveThird();
        }
    }
}
public enum TutorialNumber
{
    First,
    Second,
    Third
}