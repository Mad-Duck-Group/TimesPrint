using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite noStarSprite;
    [SerializeField] private Sprite starSprite;
    
    public Button Button => button;
    public TMP_Text Text => text;

    public void SetStars(int index)
    {
        int starCount = SaveManager.LoadStars(index);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = i < starCount ? starSprite : noStarSprite;
        }
    }
}
