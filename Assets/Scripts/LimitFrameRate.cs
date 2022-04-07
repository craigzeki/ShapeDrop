//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LimitFrameRate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FPSText;
    [SerializeField] bool fixFPS = false;
    [SerializeField] int targetFPS = 60;

    private void Awake()
    {
        if(fixFPS == true)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFPS;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FPSText.SetText(("FPS: " + Mathf.Round((1.0f / Time.deltaTime))).ToString());
        if((Application.targetFrameRate != targetFPS) && (fixFPS == true))
        {
            Application.targetFrameRate = targetFPS;
        }
    }
}
