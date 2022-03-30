using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraDirector : MonoBehaviour
{
    public enum CameraList
    {
        MenuCam = 0,
        FollowCam,
        GameOverZoomOutCam,
        NumOfCams
    }

    public enum CameraPriority
    {
        Low = 0,
        High = 1,
        NumOfPriorities
    }

    [SerializeField] private CinemachineVirtualCamera[] cameraList = new CinemachineVirtualCamera[(int)CameraList.NumOfCams];

    private static CameraDirector instance;
    

    public static CameraDirector Instance
    {
        get
        {
            //set the instance variable if not yet set
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CameraDirector>();
            }
            return instance;
        }
    }

    void Awake()
    {
        SetCamera(CameraList.MenuCam);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCamera(CameraList newCam)
    {
        for (int i = (int)CameraList.MenuCam; i < (int)CameraList.NumOfCams; i++)
        {
            cameraList[i].Priority = (i == (int)newCam) ? (int)CameraPriority.High : (int)CameraPriority.Low;
        }
    }

    public void SetNewPlayer(GameObject player)
    {
        for (int i = (int)CameraList.MenuCam; i < (int)CameraList.NumOfCams; i++)
        {
            cameraList[i].Follow = player.transform;
            cameraList[i].LookAt = player.transform;
        }
    }
}
