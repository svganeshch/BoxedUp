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
        SetCurrentLevelData();
    }

    void SetCurrentLevelData()
    {
        LevelData levelData = levelsData[currentLevel];

        boxGridManager.GenerateBoxGrid(levelData);
    }

    public void NextLevel()
    {
        if (currentLevel >= levelsData.Length - 1)
        {
            boxGridManager.GenerateBoxGrid(GenerateRandomLevel());

            Level += 1;
            packageController.RestartCoroutine();
            OnLevelChange.Invoke(Level + 1);

            return;
        }

        Level += 1;

        //ClearLevel
        
        SetCurrentLevelData();

        packageController.RestartCoroutine();

        OnLevelChange.Invoke(Level + 1);
    }

    private LevelData GenerateRandomLevel()
    {
        LevelData randomLevel = (LevelData) ScriptableObject.CreateInstance("LevelData");

        randomLevel.rows = Random.Range(2, 5);
        randomLevel.cols = Random.Range(2, 5);
        randomLevel.gridLayers = Random.Range(2, 4);
        randomLevel.gridRowsReduced = Random.value < 0.5f;
        randomLevel.gridColumnsReduced = Random.value < 0.5f;

        randomLevel.boxPrefabs = levelsData[levelsData.Length - 1].boxPrefabs;
        randomLevel.boxColors = levelsData[levelsData.Length - 1].boxColors;

        return randomLevel;
    }
}
