//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Stat timeBar;

    private float remainingTime;
    private float timeElapsed = 0;

    private static TimeManager instance;

    public float RemainingTime { get => remainingTime; set => remainingTime = Mathf.Clamp(value, remainingTime, timeBar.MaxValue); } //only allow externals to set time upwards
    public static TimeManager Instance
    {
       
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TimeManager>();
            }
            return instance;
        }
        
    }

    public float TimeElapsed { get => Mathf.Round(timeElapsed); set => timeElapsed = value; }

    private void Awake()
    {
        timeBar.Initialize();
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        timeElapsed = 0;
        RemainingTime = timeBar.MaxValue;
        timeBar.CurrentValue = RemainingTime;
    }

    public void Resume()
    {
        RemainingTime = timeBar.MaxValue;
        timeBar.CurrentValue = RemainingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameSystemController.Instance.CurrentGameState == GameSystemController.GameStates.GamePlay)
        {
            if ((RemainingTime >= 0) && (GameLoop.Instance.PlayerDied == false))
            {
                remainingTime -= Time.deltaTime;
                timeBar.CurrentValue = Mathf.Clamp(RemainingTime, 0, timeBar.MaxValue);
                if (RemainingTime <= 0)
                {
                    GameLoop.Instance.PlayerDied = true;
                }
                TimeElapsed = timeElapsed + Time.deltaTime;
            }
        }
        
        
    }
}
