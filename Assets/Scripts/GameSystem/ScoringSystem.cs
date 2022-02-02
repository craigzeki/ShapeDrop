using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI clearedText;
    [SerializeField] TMPro.TextMeshProUGUI highText;
    [SerializeField] TMPro.TextMeshProUGUI timeElapsedText;
    [SerializeField] TMPro.TextMeshProUGUI highTimeElapsedText;

    private string clearedStr = "Cleared: ";
    private string elapsedStr = "Elapsed: ";
    private string highStr = "High: "; // replace with player initials when players are available in menu system

    private int clearedSurfaces = 0;
    private int elapsedTime = 0;
    private int highScore = 0; //stored in PlayerPrefs "HighScore"
    private int highTime = 0;

    private static ScoringSystem instance;

    public static ScoringSystem Instance { 
        get 
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ScoringSystem>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore");
        highTime = PlayerPrefs.GetInt("HighTime");
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameSystemController.Instance.CurrentGameState == GameSystemController.GameStates.GamePlay)
        {
            elapsedTime = Mathf.RoundToInt(TimeManager.Instance.TimeElapsed);
            if (elapsedTime > highTime)
            {
                //player has achieved high score (time)
                highTime = elapsedTime;

                //save the high score
                PlayerPrefs.SetInt("HighTime", highTime);
                PlayerPrefs.Save();
            }
        }
        
        UpdateScoreText();
        CheckAchievements();
    }

    private void UpdateScoreText()
    {
        //current score
        clearedText.text = clearedStr + clearedSurfaces;
        timeElapsedText.text = elapsedStr + elapsedTime + "s";

        //high score text
        highText.text = highStr + highScore;
        highTimeElapsedText.text = highStr + highTime + "s";
        
    }

    public void SurfaceCleared()
    {
        clearedSurfaces++;
        if (clearedSurfaces > highScore)
        {
            highScore = clearedSurfaces;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        GameLoop.Instance.UpdateDifficulty(clearedSurfaces);
        
    }

    public void resetScores()
    {
        clearedSurfaces = 0;
        elapsedTime = 0;
    }

    public void resetHighScores()
    {
        highScore = 0;
        highTime = 0;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetInt("HighTime", highTime);
        PlayerPrefs.Save();

    }

    private void CheckAchievements()
    {
        switch(clearedSurfaces)
        {
            case 1:
                AchievementManager.Instance.EarnAchievement("My 1st Surface");
                break;

            case 5:
                AchievementManager.Instance.EarnAchievement("Clear 5 Surfaces");
                break;

            case 10:
                AchievementManager.Instance.EarnAchievement("Clear 10 Surfaces");
                break;

            case 15:
                AchievementManager.Instance.EarnAchievement("Clear 15 Surfaces");
                break;

            case 30:
                AchievementManager.Instance.EarnAchievement("Clear 30 Surfaces");
                break;
        }

        //Check the time achievements - note only works as elapsedTime has been rounded to an int and will be a whole number for 1 second
        //This is much nicer than nested if's though!
        switch(elapsedTime)
        {
            case 30:
            
                AchievementManager.Instance.EarnAchievement("Last 30 Seconds");
                break;
            case 60:
                AchievementManager.Instance.EarnAchievement("Last 60 Seconds");
                break;
            case 90:
                AchievementManager.Instance.EarnAchievement("Last 90 Seconds");
                break;

        }
        

    }
}
