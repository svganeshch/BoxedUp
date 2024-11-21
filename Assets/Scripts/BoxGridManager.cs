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

        float gridWidth = 0f;
        float gridDepth = 0f;

        Vector3 startPosition = Vector3.zero;

        for (int layer = 0; layer < layers; layer++)
        {
            for (int row = 0; row < currentRows; row++)
            {
                for (int column = 0; column < currentColumns; column++)
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

                    gridWidth = currentColumns * rotatedSize.x + (currentColumns - 1) * spacingOffset.x;
                    gridDepth = currentRows * rotatedSize.z + (currentRows - 1) * spacingOffset.z;

                    startPosition = new Vector3(
                        -gridWidth / 2f + rotatedSize.x / 2f,
                        rotatedSize.y / 2f + layer * (rotatedSize.y + spacingOffset.y),
                        -gridDepth / 2f + rotatedSize.z / 2f
                    ) + gridOffset;

                    Vector3 position = new Vector3(
                        startPosition.x + column * (rotatedSize.x + spacingOffset.x),
                        startPosition.y,
                        startPosition.z + row * (rotatedSize.z + spacingOffset.z)
                    );

                    Box box = Instantiate(boxPrefab, position, rotation, transform).GetComponent<Box>();
                    box.SetColor(levelColorData[Random.Range(0, levelColorData.Length)].color);
                    boxList.Add(box);
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