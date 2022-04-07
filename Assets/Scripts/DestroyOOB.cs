//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOOB : MonoBehaviour
{
    [SerializeField] private float zBound = -50;

    public float ZBound { get => zBound; set => zBound = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z <= ZBound)
        {
            GameLoop.Instance.DestroySurface(this.gameObject);
        }
    }
}
