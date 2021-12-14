using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceMovement : MonoBehaviour
{
    private float speed = 1.0f;

    public float Speed { get => speed; set => speed = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -Speed * Time.deltaTime);
    }
}
