using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShapeDrop.Enums; //all required enums for the game


[RequireComponent(typeof(AudioSource))]

public class GameLoop : MonoBehaviour
{
    
    [SerializeField] private GameObject[] surfacePrefabs = new GameObject[(int)ShapeIDs.NumOfIDs];
    [SerializeField] private GameObject[] powerUpPrefabs = new GameObject[(int)PowerUps.NumOfPowerUps];
    [SerializeField] private Color correctHoleColour;
    [SerializeField] private Color incorrectHoleColour;
    [SerializeField] private GameObject surfaceHolder;
    [SerializeField] private int surfaceSize = 5;
    [SerializeField] private Difficulty difficulty = Difficulty.Easy;
    [SerializeField] private Dictionary<int, int> difficultyThresholds = new Dictionary<int, int>() { {(int)Difficulty.Easy, 0 },{ (int)Difficulty.Medium, 5 },{(int)Difficulty.Hard, 10 },{(int)Difficulty.VeryHard, 20},{(int)Difficulty.Ultra, 40 } };
    [SerializeField] private Dictionary<int, float> difficultySpeeds = new Dictionary<int, float>() { {(int)Difficulty.Easy, 30 },{ (int)Difficulty.Medium, 50 },{(int)Difficulty.Hard, 70 },{(int)Difficulty.VeryHard, 90},{(int)Difficulty.Ultra, 100 } };
    [SerializeField] private Dictionary<int, int> difficultyTimerEveryXSurfaces = new Dictionary<int, int>() { {(int)Difficulty.Easy, 4 },{ (int)Difficulty.Medium, 5 },{(int)Difficulty.Hard, 6 },{(int)Difficulty.VeryHard, 7},{(int)Difficulty.Ultra, 8 } };
    
    [SerializeField] private int numSurfacesToBuffer = 3;
    


    private List<GameObject> surfaces = new List<GameObject>();
    private GameObject player;

    //private static GameLoop instance;
    [SerializeField] private float surfaceSpeed = 0;
    private int forceTimerEveryXSurface = 0;
    private int timerDeltaSurfaces = 0;

    private GameModifiers gameModifiers = new GameModifiers(true); //use constructor overload to reset to defaults

    private bool playerDied = false;
    [SerializeField] private float wrongSurfacePenaltyScaleOffset = 0.1f;
    [SerializeField] private AudioSource myAudioSource;
    
    private float surfaceSpeedOffset = 0;
    [SerializeField] private AudioClip wrongHolePenaltySound;
    [SerializeField] private int maxPowerUpsAtOnce = 3;
    private bool[] powerUpRequiresSameShapeHole = new bool[((int)PowerUps.NumOfPowerUps)] {false, false, false, true, true, false, false };

    private static GameLoop instance;
    public static GameLoop Instance {

        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameLoop>();
            }
            return instance;
        }    
    }

    public void Init()
    {
        //clear the game modifiers
        gameModifiers.ResetToDefaults();

        UpdateDifficulty(0); //will set surfaceSpeed = 0 also
        PlayerDied = false;
        CameraDirector.Instance.SetCamera(CameraDirector.CameraList.FollowCam);
        player = GameObject.FindGameObjectWithTag("Player");
        
        surfaceSpeedOffset = 0;
    }

    public void Resume()
    {
        PlayerDied = false;
        CameraDirector.Instance.SetCamera(CameraDirector.CameraList.FollowCam);
        player = GameObject.FindGameObjectWithTag("Player");
        
        surfaceSpeedOffset = 0;
    }

    public bool PlayerDied
    {
        get
        {
            return playerDied;
        }

        set
        {
            playerDied = value;
            if (playerDied) { CameraDirector.Instance.SetCamera(CameraDirector.CameraList.GameOverZoomOutCam); }
        }
    }

    public void DestroyAllSurfaces()
    {
        foreach(GameObject surface in surfaces)
        {
            //TODO - fade surfaces out first
            Destroy(surface);

        }
        surfaces.Clear();
    }

    public float SurfaceSpeed { get => surfaceSpeed; set => surfaceSpeed = value; }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private bool randomBool()
    {
        int value = Random.Range(0, 10);
        return value >= 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameSystemController.Instance.CurrentGameState == GameSystemController.GameStates.GamePlay)
        {
            if (!playerDied)
            {
                if (surfaces.Count < numSurfacesToBuffer)
                {
                    CreateSurface(SurfaceSpeed, randomBool());
                }

            }
            else
            {

                foreach (GameObject surface in surfaces)
                {
                    surface.GetComponent<SurfaceMovement>().Speed = 0;
                }
            }
        }
        
        
    }

    public void UpdateDifficulty(int surfacesCleared)
    {
        int difficultySurfs;
        
        if (surfacesCleared == 0)
        {
            difficulty = Difficulty.Easy;
            difficultySpeeds.TryGetValue((int)difficulty, out surfaceSpeed);
            difficultyTimerEveryXSurfaces.TryGetValue((int)difficulty, out forceTimerEveryXSurface);
        }
        else
        {

            if (difficulty != Difficulty.NumOfDifficulties - 1)
            {
                difficultyThresholds.TryGetValue((int)difficulty + 1, out difficultySurfs);
                if (surfacesCleared >= difficultySurfs)
                {
                    difficulty++;
                    difficultySpeeds.TryGetValue((int)difficulty, out surfaceSpeed);
                    difficultyTimerEveryXSurfaces.TryGetValue((int)difficulty, out forceTimerEveryXSurface);
                }

            }
        }

    }

    private void generateRandomPowerUps(out List<PowerUps> powerUpListMatchingHole, out List<PowerUps> powerUpListAnyHole)
    {
        int numOfPowerUpsToGenerate = Random.Range((int)0, maxPowerUpsAtOnce + 1);
        powerUpListMatchingHole = new List<PowerUps>();
        powerUpListAnyHole = new List<PowerUps>();

        PowerUps[] powerUpSeq = new PowerUps[((int)PowerUps.NumOfPowerUps)];
        float[] randFloats = new float[((int)PowerUps.NumOfPowerUps)];
        bool timerAdded = false;

        for (int i = 0; i < ((int)PowerUps.NumOfPowerUps); i++)
        {
            powerUpSeq[i] = (PowerUps)i;
            randFloats[i] = Random.Range(0.0f, 10000.0f);
        }

        System.Array.Sort(randFloats, powerUpSeq);

        if (numOfPowerUpsToGenerate > ((int)PowerUps.NumOfPowerUps))
        {
            numOfPowerUpsToGenerate = ((int)PowerUps.NumOfPowerUps);
        }

        timerAdded = false;

        for (int i = 0; i < numOfPowerUpsToGenerate; i++)
        {
            if(powerUpSeq[i] == PowerUps.Timer)
            {
                timerAdded = true;
            }
            addPowerUpToList((powerUpSeq[i]), ref powerUpListMatchingHole, ref powerUpListAnyHole);
            //if(powerUpRequiresSameShapeHole[((int)powerUpSeq[i])])
            //{
            //    powerUpListMatchingHole.Add(powerUpSeq[i]);
            //}
            //else
            //{
            //    powerUpListAnyHole.Add(powerUpSeq[i]);
            //}
                
        }

        if((!timerAdded) && (timerDeltaSurfaces >= forceTimerEveryXSurface))
        {
            //need to add a timer to balance the game --> force into matching hole list
            addPowerUpToList(PowerUps.Timer, ref powerUpListMatchingHole, ref powerUpListMatchingHole);
            timerDeltaSurfaces = 0;
        }
        else
        {
            timerDeltaSurfaces++;
        }
        
    }

    private void addPowerUpToList(PowerUps powerUp, ref List<PowerUps> powerUpListMatchingHole, ref List<PowerUps> powerUpListAnyHole)
    {
        if (powerUpRequiresSameShapeHole[((int)powerUp)])
        {
            powerUpListMatchingHole.Add(powerUp);
        }
        else
        {
            powerUpListAnyHole.Add(powerUp);
        }
    }

    public GameObject GetCurrentSurface()
    {
        if (surfaces.Count == 0)
        {
            return null;
        }
        return surfaces[0];
    }

    private void CreateSurface(float speed, bool includeTimeBonus)
    {
        
        int pos = surfaces.Count + 1;
        GameObject newSurface = Instantiate(surfaceHolder, new Vector3(0,0, pos*200), new Quaternion(0,0,0,0));

        //Generate the surface
        List<PowerUps> powerUpsMatchingHole = new List<PowerUps>();
        List<PowerUps> powerUpsAnyHole = new List<PowerUps>();
        generateRandomPowerUps(out powerUpsMatchingHole, out powerUpsAnyHole);

        ShapeIDs[,] surfaceMap = new ShapeIDs[surfaceSize, surfaceSize];
        float arrayOriginOffset = ((float)surfaceSize / 2.0f) - 0.5f; //calculate the offset to the origin of the surfaceHolder in 2D array space
        int tileCount = 0; //used to track the location in the surfaceMap as we itterate through

        System.Array.Clear(surfaceMap, (int)ShapeIDs.Blank, surfaceMap.Length); //set all array positions to Blank (0)

        //generate a psuedo random map. This is the map we will use to determine which shape will go where.
        //first we generate a 1D array to contain a 1-1 map to the surfaceMap. This will simply indicate the order to populate the surfaceMap with.
        //We will start with this array sequenced in order 0 --> 24
        // we will then create a second array of equal size but of floats. This will be populated with random numbers. We will use this array to sort the mapping
        // array into a random order. We can then simply itterate through the sorted map and use this as the index for the surfaceMap. This will ensure all elements are included, without having to
        // call Random too many times.

        int[] surfaceSeq = new int[surfaceSize * surfaceSize];
        float[] randomSeq = new float[surfaceSeq.Length];

        //populate the two sequences
        for (int i = 0; i < surfaceSeq.Length; i++)
        {
            surfaceSeq[i] = i;
            randomSeq[i] = Random.Range(0.0f, 10000.0f);
        }
        
        System.Array.Sort(randomSeq, surfaceSeq);

        //place a random number of matching player shape holes based on difficulty (force to int so that it can be used to reference in the array)
        //The easier the difficulty, the more holes mathcing your shape that are potentially available
        int d = (int)Random.Range((int)1, (int)Difficulty.NumOfDifficulties - (int)difficulty);

        int powerUpIndex = powerUpsMatchingHole.Count - 1; //used to track through the power up lists

        for(tileCount = 0; (tileCount < d) && (tileCount < surfaceMap.Length); tileCount++)
        {
            int x, y;
            calc2DArrayIndex(surfaceSeq[tileCount], surfaceSize, out x, out y);
            surfaceMap[x, y] = player.GetComponent<PlayerMovement>().MyShape;
            GameObject newSurfaceTile = (GameObject)Instantiate(surfacePrefabs[(int)player.GetComponent<PlayerMovement>().MyShape], newSurface.transform);
            newSurfaceTile.transform.localPosition = new Vector3(
                (x - arrayOriginOffset) * newSurfaceTile.GetComponent<SurfaceData>().MyBounds.size.x, //e.g. (0,0) (bottom left) in array space = (0-((5/2)-0.5))*5 = -10
                (y - arrayOriginOffset) * newSurfaceTile.GetComponent<SurfaceData>().MyBounds.size.y,
                0);
            newSurfaceTile.GetComponent<SurfaceData>().Shape = surfaceMap[x, y];
            newSurfaceTile.GetComponentInChildren<Light>().color = correctHoleColour;
            //if ((tileCount == 0) && (includeTimeBonus == true))
            //{
            //    // place a timer bonus in the hole
            //    GameObject timerBonus = (GameObject)Instantiate(powerUpPrefabs[(int)PowerUps.Timer], newSurfaceTile.transform);
            //    //newSurfaceTile.GetComponentInChildren<Light>().enabled = false;
            //}
            ////DEBUG
            //else if(tileCount == 0)
            //{
            //    GameObject aiPowerUp = (GameObject)Instantiate(powerUpPrefabs[(int)PowerUps.AI], newSurfaceTile.transform);
            //    //newSurfaceTile.GetComponentInChildren<Light>().enabled = false;
            //}

            //add any powerups which require matching the shapes hole
            if(powerUpIndex >= 0)
            {
                Instantiate(powerUpPrefabs[(int)powerUpsMatchingHole[powerUpIndex]], newSurfaceTile.transform);
                newSurfaceTile.GetComponent<SurfaceData>().hasPowerUp = true;
                powerUpIndex--;
            }
        }

        //reset the power up index for use in the AnyHole array
        powerUpIndex = powerUpsAnyHole.Count - 1;

        //populate the rest of the map
        for(; tileCount < surfaceMap.Length; tileCount++)
        {
            int x, y;
            calc2DArrayIndex(surfaceSeq[tileCount], surfaceSize, out x, out y);
            surfaceMap[x, y] = (ShapeIDs)Random.Range((int)0, (int)ShapeIDs.NumOfIDs);
            GameObject newSurfaceTile = Instantiate(surfacePrefabs[(int)surfaceMap[x,y]], newSurface.transform);
            newSurfaceTile.transform.localPosition = new Vector3(
                (x - arrayOriginOffset) * newSurfaceTile.GetComponent<SurfaceData>().MyBounds.size.x, //e.g. (0,0) (bottom left) in array space = (0-((5/2)-0.5))*5 = -10
                (y - arrayOriginOffset) * newSurfaceTile.GetComponent<SurfaceData>().MyBounds.size.y,
                0);
            newSurfaceTile.GetComponent<SurfaceData>().Shape = surfaceMap[x, y];
            
            if ((surfaceMap[x,y] != player.GetComponent<PlayerMovement>().MyShape) && (surfaceMap[x,y] != ShapeIDs.Blank))
            {
                newSurfaceTile.GetComponentInChildren<Light>().color = incorrectHoleColour;
                //newSurfaceTile.GetComponentInChildren<Light>().enabled = false;
            }

            if (powerUpIndex >= 0)
            {
                Instantiate(powerUpPrefabs[(int)powerUpsAnyHole[powerUpIndex]], newSurfaceTile.transform);
                newSurfaceTile.GetComponent<SurfaceData>().hasPowerUp = true;
                powerUpIndex--;
            }
        }

        

        newSurface.GetComponent<SurfaceData>().RefreshBounds();
        surfaces.Add(newSurface);

        //surface speed should be stored ready for applying when it is the currentSurface (the one the player is playing against)
        //Until it is the current surface it should match the speed of the current surface to maintain the correct distance
        if(newSurface == GetCurrentSurface())
        {
            //the new surface is the current surface, set its proper speed;
            newSurface.GetComponent<SurfaceMovement>().Speed = speed;
        }
        else
        {
            //the new surface is behind the current surface, match the current surface speed
            newSurface.GetComponent<SurfaceMovement>().Speed = GetCurrentSurface().GetComponent<SurfaceMovement>().Speed;
        }

        //store the desired speed for use later
        newSurface.GetComponent<SurfaceData>().Speed = speed;
        
    }

    private void calc2DArrayIndex(int inputVal, int dimSize, out int x, out int y)
    {
        if(dimSize == 0)
        {
            //failed divide by 0 test
            x = 0;
            y = 0;
        }
        else
        {
            x = inputVal / dimSize;
            y = inputVal % dimSize;
        }
    }

   public void CollectPowerUp(GameObject powerUpObj)
    {
        //Test to make sure powerUpObj is not null
        if (powerUpObj == null) { Debug.Log("GameLoop.CollectPowerUp : powerUpObj = Null");  return; }
        //Test to make sure PowerUp component is present
        if(powerUpObj.GetComponent<PowerUp>() ==  null) { Debug.Log("GameLoop.CollectPowerUp : powerUpObj.PowerUp = Null"); return; }

        //get the correct set of modifiers for this power up dependant upon difficulty
        gameModifiers = powerUpObj.GetComponent<PowerUp>().Collect(difficulty);
        applyGameModifiers();

        Debug.Log("Game Modifiers collected: " + gameModifiers.ToString());
        playSoundEffect(powerUpObj.GetComponent<PowerUp>().PowerUpSound);
        
        Destroy(powerUpObj);

    }

    private void playSoundEffect(AudioClip clip)
    {
        myAudioSource.clip = clip;
        myAudioSource.loop = false;
        myAudioSource.Play();
    }

    private void applyGameModifiers()
    {
        TimeManager.Instance.RemainingTime += gameModifiers.GameTime;
        StartCoroutine(lerpPlayerScale(gameModifiers.PlayerScaleOffset));
        //player.transform.localScale = new Vector3(Mathf.Clamp(player.transform.localScale.x + gameModifiers.playerScaleOffset, minPlayerScale.x, (minPlayerScale.x * maxScaleFactor)),
        //                                            Mathf.Clamp(player.transform.localScale.y + gameModifiers.playerScaleOffset, minPlayerScale.y, (minPlayerScale.y * maxScaleFactor)),
        //                                            player.transform.localScale.z);
        surfaceSpeedOffset += gameModifiers.GameSpeed; //CZ


    }

    public void applyWrongSurfacePenalty()
    {
        playSoundEffect(wrongHolePenaltySound);
        StartCoroutine(lerpPlayerScale(wrongSurfacePenaltyScaleOffset));

    }

    private void adjustPlayerScale(Vector3 currentScale, float scaleOffset)
    {
        Vector3 playerMinScale = player.GetComponent<PlayerMovement>().minScale;
        Vector3 playerMaxScale = player.GetComponent<PlayerMovement>().maxScale;

        player.transform.localScale = new Vector3(Mathf.Clamp(currentScale.x + scaleOffset, playerMinScale.x, playerMaxScale.x),
                                                    Mathf.Clamp(currentScale.y + scaleOffset, playerMinScale.y, playerMaxScale.y),
                                                    Mathf.Clamp(currentScale.z + scaleOffset, playerMinScale.z, playerMaxScale.z));
    }

    IEnumerator lerpPlayerScale(float targetOffset)
    {

        float timeElapsed = 0.0f;
        float duration = wrongHolePenaltySound.length;
        float scaleOffset = 0.0f;
        Vector3 currentScale = player.transform.localScale;

        if(targetOffset != 0.0f)
        {
            while (timeElapsed < duration)
            {
                scaleOffset = Mathf.Lerp(0, targetOffset, timeElapsed / duration);
                adjustPlayerScale(currentScale, scaleOffset);
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            scaleOffset = targetOffset;
            adjustPlayerScale(currentScale, scaleOffset);
        }
        
    }

    public void DestroySurface(GameObject surfaceGameObject)
    {
        if(surfaceGameObject != null)
        {
            surfaces.Remove(surfaceGameObject);
            Destroy(surfaceGameObject);

            UpdateSurfaces(); //ensure next surface has proper speed, etc.
        }
        
    }
    private void UpdateSurfaces()
    {
        foreach (GameObject surface in surfaces)
        {
            //set the surface speed to the one originally desired at surface creation for the currentSurface
            //surface.GetComponent<SurfaceMovement>().Speed = GetCurrentSurface().GetComponent<SurfaceData>().Speed;
            surface.GetComponent<SurfaceMovement>().Speed = SurfaceSpeed + surfaceSpeedOffset;
        }

    }
}
