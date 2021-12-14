using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement
{
    private string name;
    private string description;
    private bool unlocked;
    private int points;
    private int spriteIndex;
    private GameObject achievementRef;

    private List<Achievement> dependencies = new List<Achievement>();

    private string child;


    public Achievement(string name, string description, int points, int spriteIndex, GameObject achievementRef)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Description = description ?? throw new ArgumentNullException(nameof(description));
        this.Unlocked = false;
        this.Points = points;
        this.SpriteIndex = spriteIndex;
        this.achievementRef = achievementRef != null ? achievementRef : throw new ArgumentNullException(nameof(achievementRef));

        LoadAchievemet();
    }

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public bool Unlocked { get => unlocked; set => unlocked = value; }
    public int Points { get => points; set => points = value; }
    public int SpriteIndex { get => spriteIndex; set => spriteIndex = value; }
    public string Child { get => child; set => child = value; }

    public void AddDependency(Achievement dependency)
    {
        dependencies.Add(dependency);
    }

    public bool EarnAchievement()
    {
        if(!Unlocked && !dependencies.Exists(x => x.unlocked == false))
        {
            achievementRef.GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite;
            SaveAchievement(true);

            if (Child != null)
            {
                AchievementManager.Instance.EarnAchievement(child);
            }

            return true;
        }
        else
        {
            //do nothing
        }
        return false;
    }

    public void SaveAchievement(bool value)
    {
        Unlocked = value;
        int tmpPoints = PlayerPrefs.GetInt("Points");

        PlayerPrefs.SetInt("Points", tmpPoints += Points);
        PlayerPrefs.SetInt(Name, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadAchievemet()
    {
        Unlocked = PlayerPrefs.GetInt(Name) == 1 ? true : false;

        if(Unlocked)
        {
            AchievementManager.Instance.textPoints.text = PlayerPrefs.GetInt("Points").ToString();
            achievementRef.GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite;
        }
    }

    public void DeleteAchievement()
    {
        UnityEngine.MonoBehaviour.Destroy(achievementRef);
    }
}
