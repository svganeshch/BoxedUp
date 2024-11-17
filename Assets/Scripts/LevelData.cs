using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int rows;
    public int cols;
    public int gridHeight;

    public GameObject[] boxPrefabs;
    public ColorData[] boxColors;
}
