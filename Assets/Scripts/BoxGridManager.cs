using System.Collections.Generic;
using UnityEngine;

public class BoxGridManager : MonoBehaviour
{
    public GameObject boxPrefab;
    public int rows = 5;
    public int columns = 4;
    public int layers = 2;

    public Vector3 spacingOffset = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 gridOffset = Vector3.zero;

    public bool reduceRowsPerLayer = true;
    public bool reduceColumnsPerLayer = true;
    public int rowReductionAmount = 1;
    public int columnReductionAmount = 1;
    public float rotationProbability = 0.3f;

    private Vector3 boxSize;

    private LevelManager levelManager;
    private List<Box> boxList = new List<Box>();

    private void Start()
    {
        levelManager = LevelManager.Instance;
    }

    public void GenerateBoxGrid(LevelData currentLevelData)
    {
        ClearGrid();

        int currentRows = currentLevelData.rows;
        int currentColumns = currentLevelData.cols;
        layers = currentLevelData.gridLayers;
        reduceRowsPerLayer = currentLevelData.gridRowsReduced;
        reduceColumnsPerLayer = currentLevelData.gridColumnsReduced;

        GameObject[] boxes = currentLevelData.boxPrefabs;
        ColorData[] levelColorData = currentLevelData.boxColors;

        if (boxes == null || boxes.Length == 0)
        {
            Debug.LogError("No prefabs provided in the boxes list!");
            return;
        }

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        int tempRows = currentRows;
        int tempColumns = currentColumns;

        for (int layer = 0; layer < layers; layer++)
        {
            float currentZPosition = 0f;

            for (int row = 0; row < tempRows; row++)
            {
                float currentXPosition = 0f;
                float maxBoxDepthInRow = 0f;

                for (int column = 0; column < tempColumns; column++)
                {
                    GameObject boxPrefab = boxes[Random.Range(0, boxes.Length)];
                    if (!boxPrefab.TryGetComponent(out BoxCollider boxCollider))
                    {
                        Debug.LogError($"Box prefab '{boxPrefab.name}' is missing a BoxCollider!");
                        continue;
                    }

                    bool shouldRotate = Random.value < rotationProbability;
                    Vector3 rotatedSize = shouldRotate
                        ? new Vector3(boxCollider.size.z, boxCollider.size.y, boxCollider.size.x)
                        : boxCollider.size;

                    Vector3 boxPosition = new Vector3(
                        currentXPosition + rotatedSize.x / 2f,
                        layer * (rotatedSize.y + spacingOffset.y),
                        currentZPosition + rotatedSize.z / 2f
                    );

                    minBounds = Vector3.Min(minBounds, boxPosition - rotatedSize / 2f);
                    maxBounds = Vector3.Max(maxBounds, boxPosition + rotatedSize / 2f);

                    currentXPosition += rotatedSize.x + spacingOffset.x;
                    maxBoxDepthInRow = Mathf.Max(maxBoxDepthInRow, rotatedSize.z);
                }

                currentZPosition += maxBoxDepthInRow + spacingOffset.z;
            }

            if (reduceRowsPerLayer) tempRows = Mathf.Max(1, tempRows - rowReductionAmount);
            if (reduceColumnsPerLayer) tempColumns = Mathf.Max(1, tempColumns - columnReductionAmount);
        }

        Vector3 gridCenterOffset = (minBounds + maxBounds) / 2f;
        gridCenterOffset.y = minBounds.y;
        gridCenterOffset -= gridOffset;

        tempRows = currentRows;
        tempColumns = currentColumns;

        for (int layer = 0; layer < layers; layer++)
        {
            float currentZPosition = 0f;

            for (int row = 0; row < tempRows; row++)
            {
                float currentXPosition = 0f;
                float maxBoxDepthInRow = 0f;

                for (int column = 0; column < tempColumns; column++)
                {
                    GameObject boxPrefab = boxes[Random.Range(0, boxes.Length)];
                    if (!boxPrefab.TryGetComponent(out BoxCollider boxCollider))
                    {
                        Debug.LogError($"Box prefab '{boxPrefab.name}' is missing a BoxCollider!");
                        continue;
                    }

                    bool shouldRotate = Random.value < rotationProbability;
                    Quaternion rotation = shouldRotate ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;

                    Vector3 rotatedSize = shouldRotate
                        ? new Vector3(boxCollider.size.z, boxCollider.size.y, boxCollider.size.x)
                        : boxCollider.size;

                    Vector3 position = new Vector3(
                        currentXPosition + rotatedSize.x / 2f - gridCenterOffset.x,
                        layer * (rotatedSize.y + spacingOffset.y) - gridCenterOffset.y,
                        currentZPosition + rotatedSize.z / 2f - gridCenterOffset.z
                    );

                    Box box = Instantiate(boxPrefab, position, rotation, transform).GetComponent<Box>();
                    box.SetColor(levelColorData[Random.Range(0, levelColorData.Length)].color);
                    boxList.Add(box);

                    currentXPosition += rotatedSize.x + spacingOffset.x;
                    maxBoxDepthInRow = Mathf.Max(maxBoxDepthInRow, rotatedSize.z);
                }

                currentZPosition += maxBoxDepthInRow + spacingOffset.z;
            }

            if (reduceRowsPerLayer) tempRows = Mathf.Max(1, tempRows - rowReductionAmount);
            if (reduceColumnsPerLayer) tempColumns = Mathf.Max(1, tempColumns - columnReductionAmount);
        }

        levelManager.cansGridManager.GenerateCans(boxList);
    }

    public void ClearGrid()
    {
        foreach (var box in boxList)
        {
            if (box != null)
                Destroy(box.gameObject);
        }

        boxList.Clear();
    }

    private void OnDrawGizmos()
    {
        if (boxPrefab == null || !boxPrefab.TryGetComponent(out BoxCollider boxCollider)) return;

        Vector3 gizmoBoxSize = boxCollider.size;
        Gizmos.color = Color.yellow;

        int currentRows = rows;
        int currentColumns = columns;

        float gridWidth = currentColumns * gizmoBoxSize.x + (currentColumns - 1) * spacingOffset.x;
        float gridDepth = currentRows * gizmoBoxSize.z + (currentRows - 1) * spacingOffset.z;

        Vector3 startPosition = new Vector3(
            -gridWidth / 2f + gizmoBoxSize.x / 2f,
            gizmoBoxSize.y / 2f,
            -gridDepth / 2f + gizmoBoxSize.z / 2f
        ) + gridOffset;

        for (int layer = 0; layer < layers; layer++)
        {
            for (int row = 0; row < currentRows; row++)
            {
                for (int column = 0; column < currentColumns; column++)
                {
                    Vector3 position = new Vector3(
                        startPosition.x + column * (gizmoBoxSize.x + spacingOffset.x),
                        startPosition.y + layer * (gizmoBoxSize.y + spacingOffset.y),
                        startPosition.z + row * (gizmoBoxSize.z + spacingOffset.z)
                    );

                    Gizmos.DrawWireCube(position, gizmoBoxSize);
                }
            }

            if (reduceRowsPerLayer)
            {
                currentRows = Mathf.Max(1, currentRows - rowReductionAmount);
            }

            if (reduceColumnsPerLayer)
            {
                currentColumns = Mathf.Max(1, currentColumns - columnReductionAmount);
            }

            gridWidth = currentColumns * gizmoBoxSize.x + (currentColumns - 1) * spacingOffset.x;
            gridDepth = currentRows * gizmoBoxSize.z + (currentRows - 1) * spacingOffset.z;

            startPosition = new Vector3(
                -gridWidth / 2f + gizmoBoxSize.x / 2f,
                gizmoBoxSize.y / 2f,
                -gridDepth / 2f + gizmoBoxSize.z / 2f
            ) + gridOffset;
        }
    }
}