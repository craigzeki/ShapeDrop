//Author: Craig Zeki
//Student ID: zek21003166

//Based on Youtube tutorial https://youtube.com/playlist?list=PLX-uZVK_0K_5vK5wDQ2p2gFFg-uIkuJQA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
    public GameObject achievementList;

    [SerializeField] Sprite neutral, highlight;

    private Image sprite;

    // Called before start
    private void Awake()
    {
        sprite = GetComponent<Image>();
    }

    public void Click()
    {
        if (sprite.sprite == neutral)
        {
            sprite.sprite = highlight;
            achievementList.SetActive(true);
        }
        else
        {
            sprite.sprite = neutral;
            achievementList.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
