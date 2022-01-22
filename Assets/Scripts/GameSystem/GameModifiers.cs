using UnityEngine;

[System.Serializable]
public class GameModifiers
{
    public float gameSpeed;
    public float playerScaleOffset;
    public int extraLives;
    public int gameTime;
    public bool aIStarted;

    public GameModifiers()
    {
        Debug.Log("GameModifiers.Constructor called");
    }

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
