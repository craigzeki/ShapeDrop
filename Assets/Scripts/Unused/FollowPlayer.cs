using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float screenEdgePad;


    private Vector3 offset;
    

    private void Awake()
    {
    
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = player.transform.position - transform.position;
    }

    private void Update()
    {
        
    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        
        

        transform.position = player.transform.position - offset;
       
    }
}
