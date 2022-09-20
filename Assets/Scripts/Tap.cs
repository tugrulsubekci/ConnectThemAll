using DG.Tweening;
using UnityEngine;

public class Tap : MonoBehaviour
{
    public Vector3 startPos = new Vector3(0.1f, -1.5f, 0);
    [SerializeField] private Vector3[] secondPath = new Vector3[4];
    public Transform Transform => transform;
    public void MoveFirst()
    {
        Transform.DOMoveX(4.1f, 2).SetLoops(-1).From(startPos);
    }
    public void MoveSecond()
    {
        Transform.position = startPos;
        Transform.DOMove(secondPath[0], 0.5f).OnComplete(() =>
        {
            Transform.DOMove(secondPath[1], 0.5f).OnComplete(() =>
            {
                Transform.DOMove(secondPath[2], 0.5f).OnComplete(() =>
                {
                    Transform.DOMove(secondPath[3], 0.5f).OnComplete(() =>
                    {
                        MoveSecond();
                    });
                });
            });
        });
    }
    public void MoveThird()
    {
        Transform.position = startPos;
        Transform.DOMoveX(4.1f, 2).SetLoops(-1).From(startPos);
    }

    public void StopTapAnimation()
    {
        Transform.DOKill();
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