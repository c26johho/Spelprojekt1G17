using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DebugLightConePolygon : MonoBehaviour
{
    [SerializeField] private LightCone lightCone;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
    }

    private void LateUpdate()
    {
        if (lightCone == null || lightCone.hits == null)
            return;

        DrawLines(lightCone.hits);
    }

    private void DrawLines(Vector3[] hits)
    {
        lineRenderer.positionCount = hits.Length;

        for (int i = 0; i < hits.Length; i++)
        {
            lineRenderer.SetPosition(i, hits[i]);
        }
    }
}