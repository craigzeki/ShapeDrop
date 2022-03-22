using UnityEngine;

[System.Serializable]
public class GameModifiers
{
    //serialized so that they are still configurable in the inspector for the various powerups
    [SerializeField] private float gameSpeed;
    [SerializeField] private float playerScaleOffset;
    [SerializeField] private int extraLives;
    [SerializeField] private int gameTime;
    [SerializeField] private bool aIStarted;

    //Properties defined so that other classes can access
    public float GameSpeed { get => gameSpeed; set => gameSpeed = value; }
    public float PlayerScaleOffset { get => playerScaleOffset; set => playerScaleOffset = value; }
    public int ExtraLives { get => extraLives; set => extraLives = value; }
    public int GameTime { get => gameTime; set => gameTime = value; }
    public bool AIStarted { get => aIStarted; set => aIStarted = value; }

    //Constructor which automatically sets the defaults
    public GameModifiers()
    {
        //intentionally left blank as we do not want to overide the inspector values
        //instead a seperate function, ResetToDefaults is provided to override and set defaults
    }

    //constructor to allow overriding of the inspector values and reset to defaults
    public GameModifiers(bool forceDefaults)
    {
        if(forceDefaults)
        {
            ResetToDefaults();
        }    
    }

    //Constructor to allow setting of individual Properties
    public GameModifiers(float gameSpeed, float playerScaleOffset, int extraLives, int gameTime, bool aIStarted)
    {
        //use of 'this' is needed as the local function parameter names are the same as the class parameter names
        this.GameSpeed = gameSpeed;
        this.PlayerScaleOffset = playerScaleOffset;
        this.ExtraLives = extraLives;
        this.GameTime = gameTime;
        this.AIStarted = aIStarted;
    }

    public void ResetToDefaults()
    {
        gameSpeed = 0.0f;
        playerScaleOffset = 0.0f;
        extraLives = 0;
        gameTime = 0;
        aIStarted = false;

    }

    //overide the .ToString() function to provide a string containing all the values of the parameters
    public override string ToString()
    {
        string temp = "";

        temp = "gameSpeed = " + gameSpeed.ToString() + "\n";
        temp += "playerScaleOffset = " + playerScaleOffset.ToString() + "\n";
        temp += "extraLives = " + extraLives.ToString() + "\n";
        temp += "gameTime = " + gameTime.ToString() + "\n";
        temp += "aIStarted = " + aIStarted.ToString() + "\n";

        return temp;
    }
}
