using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0;
    private BoxGridManager boxGridManager;

    public int Level
    {
        set => currentLevel = value;
        get => currentLevel;
    }

    public LevelData[] levelsData;

    private void Awake()
    {
        boxGridManager = FindAnyObjectByType<BoxGridManager>();
    }

    private void Start()
    {
        GenerateCurrentLevelData();
    }

    void GenerateCurrentLevelData()
    {
        LevelData levelData = levelsData[currentLevel];
        ColorData[] levelColorData = levelData.boxColors;

        GameObject[] levelBoxPrefabs = levelData.boxPrefabs;
        int levelGridSize = levelData.rows * levelData.cols;

        List<GameObject> levelBoxes = new List<GameObject>();
        
        for (int i = 0; i < levelGridSize; i++)
        {
            GameObject box = levelBoxPrefabs[Random.Range(0, levelBoxPrefabs.Length)];

            levelBoxes.Add(box);
        }

        boxGridManager.GenerateGrid(levelData, levelBoxes, levelColorData);
    }
}
