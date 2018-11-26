using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HealCircle : MonoBehaviour
{

    public int vertexcount = 40;
    public float linewidth = 0.1f;
    public float radius = 0.3f;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = (Color.green);
        lineRenderer.endColor = Color.green;
        SetupCircle();
    }

    public void SetupCircle()
    {
        lineRenderer.widthMultiplier = linewidth;
        lineRenderer.positionCount = vertexcount;
        float dtheta = (2f * Mathf.PI) / vertexcount;
        float theta = 0f;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3((radius * Mathf.Cos(theta)), (radius * Mathf.Sin(theta)), 0f);
            lineRenderer.SetPosition(i, pos);
            theta += dtheta;
        }

    }
}
