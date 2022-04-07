//Author: Craig Zeki
//Student ID: zek21003166

//Based on Youtube tutorial https://youtube.com/playlist?list=PLX-uZVK_0K_5vK5wDQ2p2gFFg-uIkuJQA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementManager : MonoBehaviour
{

    [SerializeField] GameObject achievementPrefab;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject achievementMenu;
    [SerializeField] int fadeTime = 2;

    public Sprite[] sprites;

    private AchievementButton activeButton;

    public GameObject visualAchievement;

    public Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    public Sprite unlockedSprite;

    public Text textPoints;

    private static AchievementManager instance;

    

    public static AchievementManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<AchievementManager>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        activeButton = GameObject.Find("GeneralBtn").GetComponent<AchievementButton>();

       //Create the achievements
        CreateAchievement("Training", "My 1st Surface", "Guide your shape through the first surface!", 5, 0);
        CreateAchievement("Training", "My 1st Timer", "Collect a timer power up for the 1st time!", 5, 1);
        CreateAchievement("Training", "Training Complete!", "Complete the training achievements", 10, 2, new string[] { "My 1st Surface", "My 1st Timer" });

        CreateAchievement("General", "Clear 5 Surfaces", "Pass through 5 surfaces in 1 run", 5, 3);
        CreateAchievement("General", "Clear 10 Surfaces", "Pass through 10 surfaces in 1 run", 10, 4);
        CreateAchievement("General", "Clear 15 Surfaces", "Pass through 15 surfaces in 1 run", 15, 5);
        CreateAchievement("General", "Clear 30 Surfaces", "Pass through 30 surfaces in 1 run", 30, 6);
        CreateAchievement("General", "Surfaces Surfer!", "Complete all the surface challenges", 50, 7, new string[] { "Clear 5 Surfaces", "Clear 10 Surfaces", "Clear 15 Surfaces", "Clear 30 Surfaces" });

        CreateAchievement("General", "Last 30 Seconds", "Don't hit a surface for at least 30 seconds", 5, 8);
        CreateAchievement("General", "Last 60 Seconds", "Don't hit a surface for at least 60 seconds", 10, 9);
        CreateAchievement("General", "Last 90 Seconds", "Don't hit a surface for at least 90 seconds", 20, 10);
        CreateAchievement("General", "Maximum Stamina!", "Complete all the timer challenges", 50, 11, new string[] { "Last 30 Seconds", "Last 60 Seconds", "Last 90 Seconds" });


        foreach (GameObject achievementList in GameObject.FindGameObjectsWithTag("AchievementList"))
        {
            achievementList.SetActive(false);
        }

        activeButton.Click();

        //achievementMenu.SetActive(false); //We dont need to do this as it is hidden off screen until swiped into view
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EarnAchievement(string title)
    {
        if (achievements[title].EarnAchievement())
        {
            GameObject achievement = (GameObject)Instantiate(visualAchievement);
            SetAchievementInfo(achievement, "EarnCanvas", title);
            textPoints.text = PlayerPrefs.GetInt("Points").ToString();
            StartCoroutine(FadeAchievement(achievement));
        }
    }

    public IEnumerator HideAchievement(GameObject achievement)
    {
        yield return new WaitForSeconds(3);
        Destroy(achievement);
    }

    public void CreateAchievement(string parent, string title, string description, int points, int spriteIndex, string[] dependencies = null)
    {
        //GameObject achievement = (GameObject)Instantiate(achievementPrefab, GameObject.Find(parent).transform);
        GameObject achievement = Instantiate(achievementPrefab);
        Achievement newAchievement = new Achievement(title, description, points, spriteIndex, achievement);
        achievements.Add(title, newAchievement);
        SetAchievementInfo(achievement, parent, title);
        //SetAchievementInfo(achievement, title);

        if (dependencies != null)
        {
            foreach (string achievementTitle in dependencies)
            {
                //E.g.
                //Dependency = Press Space <-- Child = Press W
                //NewAchievement = Press W --> Press Space

                Achievement dependency = achievements[achievementTitle];
                dependency.Child = title;
                newAchievement.AddDependency(dependency);
            }
        }
    }

    //public void SetAchievementInfo(GameObject achievement, string category, string title, string description, int points, int spriteIndex)
    public void SetAchievementInfo(GameObject achievement, string parent, string title)
    {
        
            achievement.transform.SetParent(GameObject.Find(parent).transform);
            achievement.transform.localScale = new Vector3(1, 1, 1);
            achievement.transform.GetChild(0).GetComponent<Text>().text = title;
            achievement.transform.GetChild(1).GetComponent<Text>().text = achievements[title].Description;
            achievement.transform.GetChild(2).GetComponent<Text>().text = achievements[title].Points.ToString();
            achievement.transform.GetChild(3).GetComponent<Image>().sprite = sprites[achievements[title].SpriteIndex];
               
    }

    public void ChangeCategory(GameObject button)
    {
        AchievementButton achievementButton = button.GetComponent<AchievementButton>();

        scrollRect.content = achievementButton.achievementList.GetComponent<RectTransform>();

        achievementButton.Click();
        activeButton.Click();
        activeButton = achievementButton;
    }

    private IEnumerator FadeAchievement(GameObject achievement)
    {
        CanvasGroup canvasGroup = achievement.GetComponent<CanvasGroup>();

        float rate = 1.0f / fadeTime;

        int startAlpha = 0;
        int endAlpha = 1;

        

        for (int i = 0; i < 2; i++)
        {
            float progress = 0.0f;

            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            yield return new WaitForSeconds(2);
            startAlpha = 1;
            endAlpha = 0;
        }

        Destroy(achievement);
    }

    public void ResetAchievements()
    {
        foreach (var item in achievements)
        {
            PlayerPrefs.DeleteKey(item.Key);
            item.Value.DeleteAchievement();
        }
        PlayerPrefs.DeleteKey("Points");

        achievements.Clear();

        //reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
