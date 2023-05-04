using UnityEngine;


public class MovementRealize : MonoBehaviour
{

    public static bool dashIsReady = false;
    // Movement variables
    private const float heroMoveSpeed = 6.3f;
    [SerializeField]
    private float dashRange = 6f;
    public static float dashCD = 3f;

    // Animation Script
    private CharacterAnimation anim;

    private Vector3 destinationPosition;
    private float destinationDistance = 0.0f;
    private readonly float minMove = 0.5f;
    private readonly float maxMove = 500.0f;
    private GameObject lastTriggerGo = null;
    public static float speedMultiplier = 1.2f;
    public static bool wasDamaged = false;
    public static float dashCDTimer;

    public GameObject blinkSmoke;

    public static bool canMove = true;

    public Vector3 Current_position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
    void Start()
    {
        anim = GetComponent<CharacterAnimation>(); // Get the animation script
        dashCDTimer = Time.time;
    }

    void OnEnable()
    {
        destinationPosition = transform.position; // Set the destinationPosition to your current location so you don't canMove on enable
    }

    // FixedUpdate is used for physics based movement
    void FixedUpdate()
    {

        if (canMove)
        {
            CharacterAnimation.dead = false;
            MovementControl(); // Player movement function
        }

        OutOfBoundsStopper();

        if (!canMove) CharacterAnimation.dead = true;
    }

    private void MovementControl()
    {
        MovePlayer();

        // Player Move function
        if (Input.GetButton("Jump")) // If left mouse button is clicked or held down
        {
            RotatePlayer(); // Player Rotate function
            Invoke(nameof(BlinkPlayer), 0.02f);
        } else if (Input.GetMouseButton(0))
        {
            RotatePlayer();
        }
        //destinationPosition.y = transform.position.y; // Set the destination Y position to your local Y position (allows you to canMove up ramps)


        destinationDistance = Vector3.Distance(destinationPosition, transform.position); // Distance between the player and where clicked
    }

    private void MovePlayer()
    {
        if (destinationDistance >= minMove && destinationDistance <= maxMove)// If the distance between the player and clicked is greater than the minimum range and less than the maximum range
        {
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + heroMoveSpeed * speedMultiplier * Time.deltaTime * transform.forward); // Move forward based on players Vector3
            anim._animRun = true; // Enable the run animation
            Debug.DrawLine(destinationPosition, transform.position, Color.cyan); // This draws a line in Scene View so you can see where you've clicked
        }
        else // If the distance between the player and clicked is less than the min range and less than the max range then continue
        {
            anim._animRun = false; // Disable the run animation
        }
    }

    public void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Set ray to the position of your mouse
        Plane playerPlane = new(Vector3.up, transform.position); // Create a plane for the raycast
                                                                 // set a float for the position of your click
        if (playerPlane.Raycast(ray, out float hitdist)) // If the Raycast has hit something (in this case, the mouse position) then continue
        {
            Vector3 targetPoint = ray.GetPoint(hitdist); // Set a Vector3 for position clicked
            destinationPosition = targetPoint; // Set destination position to position clicked
            GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(targetPoint - transform.position)); // Rotate player towards position clicked
        }
    }

    private void BlinkPlayer()
    {  

        //if Space toogle and dash ready
        if (destinationDistance >= minMove && destinationDistance <= maxMove && dashIsReady)
        {
            BlinkVFX();
            //blink forward
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + dashRange * transform.forward);
            dashCDTimer = Time.time;
            dashIsReady = false;

            Invoke(nameof(BlinkVFX),0.05f);
        }
        else
        {
            print("problems or cd: " + (Time.fixedTime - dashCDTimer) % 0.1f);
        }
        Debug.DrawLine(destinationPosition, transform.position, Color.cyan); // This draws a line in Scene View so you can see where you've clicked
       
    }


    //проверка на столкновение с игроком
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;

        lastTriggerGo = go;

        if (go.CompareTag("Ball"))
        {
            GameObject explode = Instantiate(BallDestroyer.vfxExplPrefab);
            explode.transform.position = transform.position;
            Destroy(explode, 2);

            wasDamaged = true; //истина этой переменной позволяет выполниться условию в UIMain отвечающей за получение урона
            Destroy(go);

        }
    }

    //при пересечении границы арены перемещает обратно к границе арены
    void OutOfBoundsStopper()
    {
        Vector3 tempPos = Current_position;
        if (tempPos.x > 39) tempPos.x = 39;
        if (tempPos.z > 39) tempPos.z = 39;
        if (tempPos.x < -1) tempPos.x = -1;
        if (tempPos.z < -1) tempPos.z = -1;
        Current_position = tempPos;
    }

    void BlinkVFX()
    {
        GameObject go = Instantiate(blinkSmoke);
        go.transform.position = transform.position;
        Destroy(go, 2);
    }

 

}