using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject,3);
    }
}
