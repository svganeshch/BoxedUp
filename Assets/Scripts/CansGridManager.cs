using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CansGridManager : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public GameObject canPrefab;
    public float spacing = 0.5f;

    public List<Can> cans = new List<Can>();

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    public void GenerateCans(List<Box> boxes)
    {
        ClearCans();

        foreach (Box box in boxes)
        {
            for (int i = 0; i < box.GetSize(); i++)
            {
                Can can = Instantiate(canPrefab).GetComponent<Can>();
                can.SetColor(box.GetColor());

                cans.Add(can);
            }
        }
        cans = cans.OrderBy(_ => Random.value).ToList();

        ArrangeCans();
    }

    public void ArrangeCans()
    {
        if (cans.Count <= 0)
        {
            Debug.LogWarning("No cans to place!!");
            return;
        }

        float[] segmentLengths;
        float totalLength = CalculateLineLength(out segmentLengths);
        float currentDistance = 0f;

        foreach (var can in cans)
        {
            Vector3 position = GetPositionOnLine(segmentLengths, currentDistance);
            can.transform.position = position;

            currentDistance += spacing;
        }
    }

    float CalculateLineLength(out float[] segmentLengths)
    {
        int segmentCount = lineRenderer.positionCount - 1;
        segmentLengths = new float[segmentCount];
        float totalLength = 0f;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 start = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i));
            Vector3 end = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i + 1));
            float segmentLength = Vector3.Distance(start, end);

            segmentLengths[i] = segmentLength;
            totalLength += segmentLength;
        }
         
        return totalLength;
    }

    Vector3 GetPositionOnLine(float[] segmentLengths, float distance)
    {
        float accumulatedDistance = 0f;

        for (int i = 0; i < segmentLengths.Length; i++)
        {
            if (accumulatedDistance + segmentLengths[i] >= distance)
            {
                float remainingDistance = distance - accumulatedDistance;

                Vector3 start = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i));
                Vector3 end = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i + 1));

                return Vector3.Lerp(start, end, remainingDistance / segmentLengths[i]);
            }

            accumulatedDistance += segmentLengths[i];
        }

        return lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(lineRenderer.positionCount - 1));
    }

    public void ClearCans()
    {
        foreach (var can in cans)
        {
            if (can != null)
                Destroy(can.gameObject);
        }

        cans.Clear();
    }
}
