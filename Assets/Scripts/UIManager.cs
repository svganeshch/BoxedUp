using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    LevelManager levelManager;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    private void Start()
    {
        SetLevel(levelManager.Level + 1);

        levelManager.OnLevelChange = new UnityEvent<int>();
        levelManager.OnLevelChange.AddListener(SetLevel);
    }

    public void SetLevel(int level)
    {
        levelText.text = level.ToString();

        Debug.Log("current level to set : " + level);
    }

    public void OnSkipLevel()
    {
        levelManager.NextLevel();
    }

    public void OnLevelRefresh()
    {
        levelManager.RefreshLevel();
    }
}
