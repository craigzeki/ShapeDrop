using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceData : MonoBehaviour
{

    private Bounds myBounds;
    private float speed = 1.0f;
    private GameLoop.ShapeIDs shape;

    public Bounds MyBounds { get => myBounds; }
    public float Speed { get => speed; set => speed = value; }
    public GameLoop.ShapeIDs Shape { get => shape; set => shape = value; }

    private void Awake()
    {
        RefreshBounds();
    }

    public void RefreshBounds()
    {
        myBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            myBounds.Encapsulate(r.bounds);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
