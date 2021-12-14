using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
 
    private GameObject player;
    private GameObject surface;
    private Renderer myRenderer;
    private Collider myCollider;
    private ParticleSystem myParticleSystem;
    private Light myLight;

#if UNITY_ANDROID
    [SerializeField] private float sensitivity = 0.1f;
    private Touch myTouch;
    private Vector2 touchOffset;
#endif

    private float myHalfSurfaceXBounds;
    private float myHalfSurfaceYBounds;

    private bool killPlayerInvoked = false;

#if UNITY_STANDALONE_WIN
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Vector3 mouseStartPos = Vector3.zero;
    [SerializeField] private Vector3 mouseDeltaPos;
    [SerializeField] private Vector3 screenPoint = Vector3.zero;
    [SerializeField] private Vector2 screenDimensions;
#endif
    private GameLoop.ShapeIDs myShape;

    public float Speed { get => speed; set => speed = value; }
    public GameLoop.ShapeIDs MyShape { get => myShape; set => myShape = value; }

    // Start is called before the first frame update
    void Start()
    {
        surface = GameLoop.Instance.GetCurrentSurface();
    }


    public void Init(GameLoop.ShapeIDs shape)
    {
        MyShape = shape;
        //TODO: script to change player mesh and colliders
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Surface":
                //surfaceTrigger = true;
                
                StartCoroutine(KillPlayer());
                break;

            case "Gap":
                //do nothing - covered in trigger exit - don't want to clear the surface while we are still in the hole in case the player collides with the side of the hole and dies
                break;

            case "TimeBonus":
                TimeManager.Instance.RemainingTime += 10; //10 second increase TODO: replace with a callback to the gameloop where it can be decided how much time to add based on difficulty level. This is not the right place to do this.
                AchievementManager.Instance.EarnAchievement("My 1st Timer");
                Destroy(other.gameObject.gameObject);
                break;

            default:

                break;
        }
    }

    private IEnumerator KillPlayer()
    {
        //sets a latch so that this cannot be called multiple times
        killPlayerInvoked = true;
        //disable collider
        myCollider.enabled = false;
        //disable renderer (hide the main player mesh)
        myRenderer.enabled = false;
        //disable the yellow light
        myLight.enabled = false;
        //play the explosion particle system
        myParticleSystem.Play();
        //inform the game loop that the player is died
        GameLoop.Instance.PlayerDied = true;
        //wait for the particle system to finish displaying the explosion
        yield return new WaitForSeconds(myParticleSystem.main.startLifetime.constant);
        //inform the game controller to change to the game over state
        GameSystemController.Instance.NewGameState = GameSystemController.GameStates.GameOver;
        //destroy the player game object
        Destroy(this.gameObject);

    }    

    private void OnTriggerExit(Collider other)
    {
        //check what we collided with
        switch (other.tag)
        {
            case "Surface":
                //don't need to do anything - handled this on trigger enter
                break;

            case "Gap":
                //gapTrigger = false;
                if(other.GetComponentInParent<SurfaceData>().Shape == myShape)
                {
                    ScoringSystem.Instance.SurfaceCleared();
                }
                break;

            default:
                //do nothing
                break;
        }
    }

    private void Awake()
    {
        MyShape = GameLoop.ShapeIDs.Square; //default to square - so that menu system shows properly

        //determin the bounds of the shape for use later when limiting player position
        myHalfSurfaceXBounds = GetComponent<Renderer>().bounds.size.x / 2;
        myHalfSurfaceYBounds = GetComponent<Renderer>().bounds.size.y / 2;

        //get references to the key componenets for use later
        myRenderer = GetComponent<Renderer>();
        myCollider = GetComponent<Collider>();
        myParticleSystem = GetComponent<ParticleSystem>();
        myLight = GetComponentInChildren<Light>();

#if UNITY_STANDALONE_WIN
        //get the screen dimensions for mouse control
        screenDimensions.x = Screen.width / 2;
        screenDimensions.y = Screen.height / 2;
#endif
    }


            


   

    // Update is called once per frame
    void Update()
    {
        //check we are in the game play state - otherwise do not run
        if (GameSystemController.Instance.CurrentGameState == GameSystemController.GameStates.GamePlay)
        {
            //get a reference to the current oncoming surface
            surface = GameLoop.Instance.GetCurrentSurface();

            if (surface != null)
            {
                //calculate the screen boundry clamp positions
                float halfSurfaceXBounds = surface.GetComponent<SurfaceData>().MyBounds.size.x / 2;
                float halfSurfaceYBounds = surface.GetComponent<SurfaceData>().MyBounds.size.y / 2;

#if (UNITY_ANDROID)
                // check if there are any touches to the screen - if not check the mouse - but don't allow both
                if (Input.touchCount > 0)
                {
                    myTouch = Input.GetTouch(0); //Get the first touch to the screen - we don't care about multi touch

                    switch (myTouch.phase) //switch based on phase
                    {
                        case TouchPhase.Began:
                            // do nothing - user has only just tapped on screen
                            break;
                        case TouchPhase.Moved: //if moved or ended phase - the player has moved their finger on the screen
                        case TouchPhase.Ended:
                            //calculate new position based on the delta position of the touch
                            transform.position += new Vector3(myTouch.deltaPosition.x * sensitivity * Time.deltaTime, myTouch.deltaPosition.y * sensitivity * Time.deltaTime, 0);
                            break;
                        default:
                            break;
                    }
                }
#endif
#if UNITY_STANDALONE_WIN
                if (Input.GetMouseButtonDown(0)) // mouse first down
                {
                    screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                    mouseStartPos = new Vector3(Input.mousePosition.x - screenDimensions.x, Input.mousePosition.y - screenDimensions.y, screenPoint.z);
                }
                else if (Input.GetMouseButton(0)) //mouse held
                {
                    //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    //calculate the mouse delta
                    //Vector3 mouseDeltaPos = mouseStartPos - Input.mousePosition;
                    mouseDeltaPos = new Vector3(Input.mousePosition.x - screenDimensions.x, Input.mousePosition.y - screenDimensions.y, screenPoint.z) - mouseStartPos; 
                    //mouseDeltaPos = (new Vector3(mouseDeltaPos.x * sensitivity * Time.deltaTime, mouseDeltaPos.y * sensitivity * Time.deltaTime, screenPoint.z));

                    transform.position += new Vector3(mouseDeltaPos.x * mouseSensitivity * Time.deltaTime, mouseDeltaPos.y * mouseSensitivity * Time.deltaTime, 0);
                }
#endif
                // clamp the x and y to the bounds of the surface (compensating for size of the shape)
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, -halfSurfaceXBounds + myHalfSurfaceXBounds, halfSurfaceXBounds - myHalfSurfaceXBounds), //x
                    Mathf.Clamp(transform.position.y, -halfSurfaceYBounds + myHalfSurfaceYBounds, halfSurfaceYBounds - myHalfSurfaceYBounds), //y
                    //transform.position.z + (speed * Time.deltaTime) //z not needed as surfaces now come to the player
                    0f //don't move on z - move the surfaces instead
                    );

                if ((GameLoop.Instance.PlayerDied == true) && (killPlayerInvoked == false))
                {
                    //if the gameloop is indicating that we have died (timer expired for example) then kill the player.
                    StartCoroutine(KillPlayer());
                }
            }
        }
    }
}
