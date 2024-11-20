using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private int currentLevel = 0;

    public BoxGridManager boxGridManager;
    public CansGridManager cansGridManager;
    public SlotsPlatformManager slotsPlatformManager;
    public PackageController packageController;

    public int Level
    {
        set => currentLevel = value;
        get => currentLevel;
    }

    public LevelData[] levelsData;

    public UnityEvent<int> OnLevelChange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        boxGridManager = FindAnyObjectByType<BoxGridManager>();
        cansGridManager = FindAnyObjectByType<CansGridManager>();
        slotsPlatformManager = FindAnyObjectByType<SlotsPlatformManager>();
        packageController = FindAnyObjectByType<PackageController>();
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

    public void NextLevel()
    {
        if (currentLevel == levelsData.Length - 1) return;

        Level += 1;

        //ClearLevel
        boxGridManager.ClearGrid();
        cansGridManager.ClearCans();
        
        GenerateCurrentLevelData();

        packageController.RestartCoroutine();

        OnLevelChange.Invoke(Level + 1);
    }
}
