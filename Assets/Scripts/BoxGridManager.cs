using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxGridManager : MonoBehaviour
{
    public int rows;
    public int cols;
    public float spacing = 0.1f;
    public float padding = 0.5f;
    public List<GameObject> boxes;
    public Color gridColor = Color.green;

    private List<GameObject> boxesInstantiated = new List<GameObject>();
    private bool[,] gridOccupied;
    private Vector3[,] gridPositions;

    public void GenerateGrid(LevelData currentLevelData, List<GameObject> currentLevelBoxes, ColorData[] levelColorData)
    {
        rows = currentLevelData.rows;
        cols = currentLevelData.cols;

        boxes = currentLevelBoxes.OrderBy(_ => Random.value).ToList();
        levelColorData = levelColorData.OrderBy(_ => Random.value).ToArray();

        Vector3 gridSize = GetComponent<MeshRenderer>().bounds.size;

        gridSize.x -= 2 * padding;
        gridSize.z -= 2 * padding;

        float cellWidth = (gridSize.x - (cols - 1) * spacing) / cols;
        float cellHeight = (gridSize.z - (rows - 1) * spacing) / rows;

        Vector3 gridStartingPos = transform.position - gridSize / 2
                                  + new Vector3(padding, 0, padding);

        gridPositions = new Vector3[rows, cols];
        gridOccupied = new bool[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = gridStartingPos.x + (col * (cellWidth + spacing)) + (cellWidth / 2);
                float z = gridStartingPos.z + (row * (cellHeight + spacing)) + (cellHeight / 2);
                gridPositions[row, col] = new Vector3(x, gridStartingPos.y, z);
                gridOccupied[row, col] = false;
            }
        }

        ArrangeBoxes(levelColorData);
    }

    void ArrangeBoxes(ColorData[] levelColorData)
    {
        foreach (GameObject boxPrefab in boxes)
        {
            GameObject box = Instantiate(boxPrefab);
            IBox boxInfo = box.GetComponent<IBox>();
            boxesInstantiated.Add(box);

            if (boxInfo == null)
            {
                Debug.LogError("Box prefab must implement the IBox interface!");
                Destroy(box);
                continue;
            }

            boxInfo.SetColor(levelColorData[Random.Range(0, levelColorData.Length)].color);

            int boxSize = boxInfo.GetSize();

            bool boxPlaced = false;

            for (int row = 0; row < rows; row++)
            {
                if (boxPlaced) break;
                for (int col = 0; col < cols; col++)
                {
                    if (gridOccupied[row, col]) continue;

                    box.transform.position = gridPositions[row, col];
                    gridOccupied[row, col] = true;
                    boxPlaced = true;
                    break;
                }
            }

            if (!boxPlaced)
            {
                Debug.LogWarning("No space available for the box.");
                Destroy(box);
            }
        }
    }

    public void ClearGrid()
    {
        foreach (var box in boxesInstantiated)
        {
            Destroy(box);
        }
    }

    private void OnDrawGizmos()
    {
        if (rows == 0 || cols == 0 || spacing == 0) return;

        Gizmos.color = gridColor;

        Vector3 gridSize = GetComponent<MeshRenderer>().bounds.size;

        gridSize.x -= 2 * padding;
        gridSize.z -= 2 * padding;

        float cellWidth = (gridSize.x - (cols - 1) * spacing) / cols;
        float cellHeight = (gridSize.z - (rows - 1) * spacing) / rows;

        Vector3 gridStartingPos = transform.position - GetComponent<MeshRenderer>().bounds.size / 2
                                  + new Vector3(padding, 0, padding);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = gridStartingPos.x + (col * (cellWidth + spacing)) + (cellWidth / 2);
                float z = gridStartingPos.z + (row * (cellHeight + spacing)) + (cellHeight / 2);

                Vector3 position = new Vector3(x, gridStartingPos.y, z);
                Gizmos.DrawSphere(position, 0.5f);

                Gizmos.DrawWireCube(position, new Vector3(cellWidth, 0.1f, cellHeight));
            }
        }
    }
}
