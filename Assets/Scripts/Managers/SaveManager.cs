using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string LevelKeyFormat = "Level_{0}_Stars";
    public static bool ReadHowToPlay { get; set; } = false;

    public static void SaveStars(int levelIndex, int starCount)
    {
        int currentStarCount = LoadStars(levelIndex);
        if (starCount <= currentStarCount) return;
        PlayerPrefs.SetInt(string.Format(LevelKeyFormat, levelIndex), starCount);
    }
    
    public static int LoadStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(string.Format(LevelKeyFormat, levelIndex), 0);
    }
    
    public static void DeleteStars(int levelIndex)
    {
        PlayerPrefs.DeleteKey(string.Format(LevelKeyFormat, levelIndex));
    }
    
    [Button("Delete All Stars")]
    public static void DeleteAllStars()
    {
        for (int i = 0; i < SceneManagerPersistent.Instance.LevelCount; i++)
        {
            DeleteStars(i);
        }
    }
}


