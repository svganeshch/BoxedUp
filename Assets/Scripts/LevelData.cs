using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int rows;
    public int cols;
    public int gridLayers = 1;
    public bool gridRowsReduced = false;
    public bool gridColumnsReduced = false;

    public GameObject[] boxPrefabs;
    public ColorData[] boxColors;
}
