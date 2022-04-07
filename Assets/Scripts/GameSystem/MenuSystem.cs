//Author: Craig Zeki
//Student ID: zek21003166

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    private Vector2 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameSystemController.Instance.CurrentGameState == GameSystemController.GameStates.Menu)
        {
            Touch myTouch;
            // check if there are any touches to the screen
            if (Input.touchCount > 0)
            {
                myTouch = Input.GetTouch(0); //Get the first touch to the screen - we don't care about multi touch

                switch (myTouch.phase) //switch based on phase
                {
                    case TouchPhase.Began:
                        startPosition = myTouch.position;
                        // user tapped screen
                        break;

                    case TouchPhase.Moved: //if moved or ended phase - the player has moved their finger on the screen

                        break;
                    case TouchPhase.Ended:
                        Vector2 touchDirection = myTouch.position - startPosition;
                        if (touchDirection == Vector2.zero)
                        {
                            //tapped
                            //GameSystemController.Instance.NewGameState = GameSystemController.GameStates.GamePlay;
                        }
                        break;

                    default:

                        break;
                }
            }
        }
        
    }
}
