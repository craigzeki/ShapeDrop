//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//required enums
using ShapeDrop.Enums;

[RequireComponent(typeof(Collider))]

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUps powerUpType;
    [SerializeField] private GameModifiers[] powerUpSettings = new GameModifiers[((int)Difficulty.NumOfDifficulties)]; //use default constructor to preserve inspector values
    [SerializeField] private AudioClip powerUpSound;

    //Removed the setter for PowerUpSound as we only want external classes to have readonly access
    public AudioClip PowerUpSound { get => powerUpSound; }
    public PowerUps PowerUpType { get => powerUpType; }

    public GameModifiers Collect(Difficulty gameDifficulty)
    {

        return powerUpSettings[((int)gameDifficulty)];
    }

}
