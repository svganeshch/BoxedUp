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
        if (currentLevel == levelsData.Length - 1) return;

        Level += 1;

        //ClearLevel
        
        SetCurrentLevelData();

        packageController.RestartCoroutine();

        OnLevelChange.Invoke(Level + 1);
    }
}
