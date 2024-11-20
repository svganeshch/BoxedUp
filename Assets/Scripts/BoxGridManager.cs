using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxGridManager : MonoBehaviour
{
    public int rows;
    public int cols;
    public int layers = 1;
    public float spacing = 0.1f;
    public float padding = 0.5f;
    public float layerHeight = 1.0f;
    public bool reduceRows = false;
    public bool reduceCols = false;
    public List<GameObject> boxes;
    public Color gridColor = Color.green;

    private List<GameObject> boxesInstantiated = new List<GameObject>();
    private bool[,,] gridOccupied;
    private Vector3[,,] gridPositions;

    private CansGridManager cansGridManager;

    private void Awake()
    {
        cansGridManager = FindAnyObjectByType<CansGridManager>();
    }

    public void GenerateGrid(LevelData currentLevelData, List<GameObject> currentLevelBoxes, ColorData[] levelColorData)
    {
        rows = currentLevelData.rows;
        cols = currentLevelData.cols;
        layers = currentLevelData.gridLayers;
        reduceRows = currentLevelData.gridRowsReduced;
        reduceCols = currentLevelData.gridColumnsReduced;

        boxes = currentLevelBoxes.OrderBy(_ => Random.value).ToList();
        levelColorData = levelColorData.OrderBy(_ => Random.value).ToArray();

        Vector3 gridSize = GetComponent<MeshRenderer>().bounds.size;

        gridSize.x -= 2 * padding;
        gridSize.z -= 2 * padding;

        int maxRows = rows;
        int maxCols = cols;

        gridPositions = new Vector3[maxRows, maxCols, layers];
        gridOccupied = new bool[maxRows, maxCols, layers];

        for (int layer = 0; layer < layers; layer++)
        {
            int currentRows = maxRows - (reduceRows ? layer : 0);
            int currentCols = maxCols - (reduceCols ? layer : 0);

            float cellWidth = (gridSize.x - (currentCols - 1) * spacing) / currentCols;
            float cellHeight = (gridSize.z - (currentRows - 1) * spacing) / currentRows;

            Vector3 gridStartingPos = transform.position - gridSize / 2
                                      + new Vector3(padding, 0, padding)
                                      + new Vector3(0, layer * layerHeight, 0);

            for (int row = 0; row < currentRows; row++)
            {
                for (int col = 0; col < currentCols; col++)
                {
                    float x = gridStartingPos.x + (col * (cellWidth + spacing)) + (cellWidth / 2);
                    float z = gridStartingPos.z + (row * (cellHeight + spacing)) + (cellHeight / 2);
                    gridPositions[row, col, layer] = new Vector3(x, gridStartingPos.y, z);
                    gridOccupied[row, col, layer] = false;
                }
            }
        }

        ArrangeBoxes(levelColorData);
    }

    void ArrangeBoxes(ColorData[] levelColorData)
    {
        foreach (GameObject boxPrefab in boxes)
        {
            GameObject box = Instantiate(boxPrefab);
            IPackageItem boxInfo = box.GetComponent<IPackageItem>();
            boxesInstantiated.Add(box);

            if (boxInfo == null)
            {
                Debug.LogError("Box prefab must implement the IPackageItem interface!");
                Destroy(box);
                continue;
            }

            boxInfo.SetColor(levelColorData[Random.Range(0, levelColorData.Length)].color);

            bool boxPlaced = false;

            for (int layer = 0; layer < layers; layer++)
            {
                if (boxPlaced) break;

                int currentRows = rows - (reduceRows ? layer : 0);
                int currentCols = cols - (reduceCols ? layer : 0);

                for (int row = 0; row < currentRows; row++)
                {
                    if (boxPlaced) break;
                    for (int col = 0; col < currentCols; col++)
                    {
                        if (gridOccupied[row, col, layer]) continue;

                        box.transform.position = gridPositions[row, col, layer];
                        gridOccupied[row, col, layer] = true;
                        boxPlaced = true;
                        break;
                    }
                }
            }

            if (!boxPlaced)
            {
                Debug.LogWarning("No space available for the box.");
                Destroy(box);
            }
        }

        cansGridManager.GenerateCans(boxesInstantiated);
    }

    public void ClearGrid()
    {
        foreach (var box in boxesInstantiated)
        {
            Destroy(box);
        }

        boxesInstantiated.Clear();
    }

    private void OnDrawGizmos()
    {
        if (rows == 0 || cols == 0 || layers == 0 || spacing == 0) return;

        Gizmos.color = gridColor;

        Vector3 gridSize = GetComponent<MeshRenderer>().bounds.size;

        gridSize.x -= 2 * padding;
        gridSize.z -= 2 * padding;

        for (int layer = 0; layer < layers; layer++)
        {
            int currentRows = rows - (reduceRows ? layer : 0);
            int currentCols = cols - (reduceCols ? layer : 0);

            float cellWidth = (gridSize.x - (currentCols - 1) * spacing) / currentCols;
            float cellHeight = (gridSize.z - (currentRows - 1) * spacing) / currentRows;

            Vector3 gridStartingPos = transform.position - gridSize / 2
                                      + new Vector3(padding, 0, padding)
                                      + new Vector3(0, layer * layerHeight, 0);

            for (int row = 0; row < currentRows; row++)
            {
                for (int col = 0; col < currentCols; col++)
                {
                    float x = gridStartingPos.x + (col * (cellWidth + spacing)) + (cellWidth / 2);
                    float z = gridStartingPos.z + (row * (cellHeight + spacing)) + (cellHeight / 2);
                    float y = gridStartingPos.y;

                    Vector3 position = new Vector3(x, y, z);
                    Gizmos.DrawSphere(position, 0.5f);

                    Gizmos.DrawWireCube(position, new Vector3(cellWidth, 0.1f, cellHeight));
                }
            }
        }
    }
}
