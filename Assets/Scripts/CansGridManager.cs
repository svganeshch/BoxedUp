using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CansGridManager : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public GameObject canPrefab;
    public float spacing = 0.5f;
    public float moveDuration = 0.15f;

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

    public void ArrangeCans(bool animate = false)
    {
        if (cans.Count <= 0)
        {
            Debug.LogWarning("No cans to arrange!");
            return;
        }

        float[] segmentLengths;
        float totalLength = CalculateLineLength(out segmentLengths);
        float currentDistance = 0f;

        for (int i = 0; i < cans.Count; i++)
        {
            if (cans[i] == null)
            {
                cans.RemoveAt(i);
                i--;
                continue;
            }

            Vector3 targetPosition = GetPositionOnLine(segmentLengths, currentDistance);
            if (animate)
            {
                StartCoroutine(MoveCanToPosition(cans[i], targetPosition));
            }
            else
            {
                cans[i].transform.position = targetPosition;
            }

            currentDistance += spacing;
        }
    }

    private IEnumerator MoveCanToPosition(Can can, Vector3 targetPosition)
    {
        if (can == null) yield break;

        Vector3 startPosition = can.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            if (can == null) yield break;

            can.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (can != null)
        {
            can.transform.position = targetPosition;
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
