using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer => GetComponent<LineRenderer>();
    public void DrawLine(Vector2 startPoint, Vector2 endPoint)
    {
        lineRenderer.SetPosition(0,startPoint);
        lineRenderer.SetPosition(1,endPoint);
    }

    public void ChangeColor(Color newColor)
    {
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
    }
}
