using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    private static LevelSelectionManager _instance;
    public static LevelSelectionManager Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError("LevelSelectionManager is null");
            }
            return _instance;
        }
    }
    [SerializeField] private LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParent;
    
    private void Awake()
    {
        _instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CreateButton();
    }
    
    private void CreateButton()
    {
        for (int i = 0; i < SceneManagerPersistent.Instance.LevelCount; i++)
        {
            LevelButton button = Instantiate(buttonPrefab, buttonParent);
            button.Text.text = (i + 1).ToString();
            var index = i;
            button.Button.onClick.AddListener(() =>
            {
                SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.Level, LoadSceneMode.Additive, true, index, true);
            });
            button.SetStars(i);
        }
    }

    public void ToMainMenu()
    {
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.MainMenu, LoadSceneMode.Single, false);
    }
}
