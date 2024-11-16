using UnityEngine;
using UnityEngine.UIElements;

public class BoxGridManager : MonoBehaviour
{
    public int rows;
    public int cols;
    public GameObject boxPrefab;

    private Vector3[,] gridPositions;

    private GameObject grid;

    private void Start()
    {
        grid = gameObject;

        GenerateGrid(rows, cols);
        AssignObjectsToGrid();
    }

    public void GenerateGrid(int rows, int cols)
    {
        Vector3 gridSize = grid.GetComponent<MeshRenderer>().bounds.size;

        float gridWidth = gridSize.x / rows;
        float gridHeight = gridSize.z / cols;

        Vector3 gridStartingPos = grid.transform.position - gridSize / 2;

        gridPositions = new Vector3[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = gridStartingPos.x + (col * gridWidth) + (gridWidth / 2);
                float z = gridStartingPos.z + (row * gridHeight) + (gridHeight / 2);
                gridPositions[row, col] = new Vector3(x, gridStartingPos.y, z);
            }
        }
    }

    void AssignObjectsToGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Instantiate(boxPrefab, gridPositions[row, col], Quaternion.identity);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (gridPositions == null) return;

        Gizmos.color = Color.green;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Gizmos.DrawWireCube(gridPositions[row, col], new Vector3(1, 0.1f, 1));
            }
        }
    }

}
