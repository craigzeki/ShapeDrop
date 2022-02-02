using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//required enums
using ShapeDrop.Enums;

public class GameSystemController : MonoBehaviour
{
    public enum GameStates
    {
        Menu = 0,
        GamePlay,
        GamePause, //for future development
        GameAd,
        GameOver,
        NumOfGameStates
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas adCanvas;
    [SerializeField] private Canvas timeUpCanvas;
    [SerializeField] private TextMeshProUGUI adTimerText;

    private GameStates newGameState = GameStates.Menu;
    private GameStates currentGameState = GameStates.Menu;
    private static GameSystemController instance;

    [SerializeField] private float adDuration = 5.0f;
    private float adTimer;

    public static GameSystemController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<GameSystemController>();
            }
            return instance;
        }
    }

    public GameStates NewGameState
    {
        get
        {
            return newGameState;
        }

        set
        {
            newGameState = value;
            HandleGameStateTransitions();
        }
    }

    public GameStates CurrentGameState { get => currentGameState; }

    private void Awake()
    {
        gameOverCanvas.enabled = false;
        adCanvas.enabled = false;
        timeUpCanvas.enabled = false;
        menuCanvas.enabled = true;

        //instantite player
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<PlayerMovement>().MyShape = ShapeIDs.Square; //Always use square for menu mode
        CameraDirector.Instance.SetNewPlayer(player);
        //set camera to menumode
        CameraDirector.Instance.SetCamera(CameraDirector.CameraList.MenuCam);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            case GameStates.Menu:
                
                break;
            case GameStates.GamePlay:
                break;
            case GameStates.GamePause:
                break;
            case GameStates.GameAd:
                adTimer -= Time.deltaTime;
                adTimerText.text = "Time: " + Mathf.Clamp(Mathf.Round(adTimer), 0, adDuration) + "s";
                if(adTimer + 0.3f <= 0) //ad on small buffer to show the time = 0 in the text
                {
                    NewGameState = GameStates.GamePlay;
                }
                break;
            case GameStates.GameOver:
                break;
            case GameStates.NumOfGameStates:
                break;
            default:
                break;
        }
    }

    private void HandleGameStateTransitions()
    {
        //Check for 1-off transition actions which should be performed immediatley
        //Update functionw will handle the cyclic state behaviors

        switch (currentGameState)
        {
            case GameStates.Menu:
                //Check transitions and execute transition actions
                if (NewGameState == GameStates.GamePlay)
                {
                    //hide the menu
                    menuCanvas.enabled = false;
                    //initialise new game
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    player.GetComponent<PlayerMovement>().MyShape = ShapeIDs.Square; //replace this with a shape selected from the menu
                    GameLoop.Instance.Init();
                    TimeManager.Instance.Init();
                    ScoringSystem.Instance.resetScores();
                    //complete the transition
                    currentGameState = NewGameState;
                }
                else
                {
                    // no valid transitions
                    NewGameState = currentGameState;
                }

                break;
            case GameStates.GamePlay:

                if (NewGameState == GameStates.GameOver)
                {
                    GameLoop.Instance.DestroyAllSurfaces();             
                    //re-instantite player
                    GameObject player = Instantiate(playerPrefab);
                    player.GetComponent<PlayerMovement>().MyShape = ShapeIDs.Square; //Always use square for menu mode
                    CameraDirector.Instance.SetNewPlayer(player);
                    //set camera to menumode
                    CameraDirector.Instance.SetCamera(CameraDirector.CameraList.MenuCam);
                    //show proper gameOver canvas
                    if(TimeManager.Instance.RemainingTime > 0)
                    {
                        gameOverCanvas.enabled = true;
                    }
                    else
                    {
                        timeUpCanvas.enabled = true;
                    }
                    

                    currentGameState = NewGameState;
                }
                else
                {
                    NewGameState = currentGameState;
                }
                break;
            case GameStates.GamePause:
                break;
            case GameStates.GameAd:

                if (NewGameState == GameStates.GamePlay)
                {
                    //hide the ad canvase
                    adCanvas.enabled = false;
                    //init new game
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    player.GetComponent<PlayerMovement>().MyShape = ShapeIDs.Square; //replace this with a shape selected from the menu
                    GameLoop.Instance.Resume(); //do not reset the surface speed
                    //TimeManager.Instance.Resume(); //do not reset the time - this increases difficulty a little
                    //ScoringSystem.Instance.resetScores(); //do not reset the scores
                    currentGameState = NewGameState;
                }
                else
                {
                    NewGameState = currentGameState;
                }
                break;
            case GameStates.GameOver:

                if (NewGameState == GameStates.GamePlay)
                {
                    //hide the game over canvases
                    gameOverCanvas.enabled = false;
                    timeUpCanvas.enabled = false;
                    //init new game
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    player.GetComponent<PlayerMovement>().MyShape = ShapeIDs.Square; //replace this with a shape selected from the menu
                    GameLoop.Instance.Init();
                    TimeManager.Instance.Init();
                    ScoringSystem.Instance.resetScores();
                    currentGameState = NewGameState;
                }
                else if(NewGameState == GameStates.GameAd)
                {
                    //reset the ad timer
                    adTimer = adDuration;

                    //hide the retry canvas
                    gameOverCanvas.enabled = false;
                    timeUpCanvas.enabled = false;
                    // show the add canvas
                    adCanvas.enabled = true;
                    currentGameState = NewGameState;
                }
                else if(NewGameState == GameStates.Menu)
                {
                    //hide the retry canvas
                    gameOverCanvas.enabled = false;
                    timeUpCanvas.enabled = false;
                    //show the menu canvas
                    menuCanvas.enabled = true;
                    currentGameState = NewGameState;
                }
                else
                {
                    // no valid transitions
                    NewGameState = currentGameState;
                }
                break;
            case GameStates.NumOfGameStates:
                break;
            default:
                break;
        }
    }

    public void retryButtonPress()
    {
        if(currentGameState == GameStates.GameOver)
        {
            NewGameState = GameStates.GamePlay;
        }
        
    }

    public void resumeButtonPress()
    {
        //Goto ad
        if(currentGameState == GameStates.GameOver)
        {
            NewGameState = GameStates.GameAd;
        }
    }

    public void gotoMenuButtonPress()
    {
        //goto main menu
        if(currentGameState == GameStates.GameOver)
        {
            NewGameState = GameStates.Menu;
        }
    }

    public void startButtonPress()
    {
        if(currentGameState == GameStates.Menu)
        {
            NewGameState = GameStates.GamePlay;
        }
    }
}
