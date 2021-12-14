using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Stat health;
    [SerializeField] private Stat energy;
    [SerializeField] private Stat shield;

    private void Awake()
    {
        health.Initialize();
        energy.Initialize();
        shield.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health.CurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            health.CurrentValue += 10;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            energy.CurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            energy.CurrentValue += 10;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            shield.CurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            shield.CurrentValue += 10;
        }
    }
}
