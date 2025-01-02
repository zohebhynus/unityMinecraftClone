using UnityEngine;

public class Player : MonoBehaviour
{
    public float WalkSpeed;
    public float SprintSpeed;
    public float JumpValue;
    public float gravity;

    public bool isGrounded;
    public bool isRunning;

    public float playerWidth;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool startJump;

    private Transform playerCam;
    private World world;

    private void Start()
    {
        playerCam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        transform.Rotate(Vector3.up * mouseHorizontal);
        playerCam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
    }

    private void HandleMovement()
    {
        // Add gravity to vertical momentum
        if(verticalMomentum > gravity) 
        {
            verticalMomentum += gravity * Time.fixedDeltaTime;
        }

        // sprint or walk
        float speed = WalkSpeed;
        if(isRunning)
        {
            speed = SprintSpeed;
        }
        velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * speed;

        // add vertical momentum
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        // collision blocks
        if((velocity.z > 0 && checkFront()) || (velocity.z < 0 && checkBack()))
        {
            velocity.z = 0;
        }
        if ((velocity.x > 0 && checkRight()) || (velocity.x < 0 && checkLeft()))
        {
            velocity.x = 0;
        }

        if(velocity.y < 0)
        {
            velocity.y = checkDownSpeed(velocity.y);
        }
        else if(velocity.z > 0) 
        {
            velocity.y = checkUpSpeed(velocity.y);
        }
    }

    private void GetPlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if(Input.GetButtonDown("Sprint"))
        {
            isRunning = true;
        }
        if(Input.GetButtonUp("Sprint"))
        {
            isRunning = false;
        }

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            startJump = true;
        }
    }

    private void HandleJump()
    {
        if (startJump)
        {
            verticalMomentum = JumpValue;
            isGrounded = false;
            startJump = false;
        }
    }

    private float checkDownSpeed(float speed)
    {
        if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + speed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + speed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + speed, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + speed, transform.position.z + playerWidth))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return speed;
        }
    }

    private float checkUpSpeed(float speed)
    {
        if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2.0f + speed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2.0f + speed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2.0f + speed, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2.0f + speed, transform.position.z + playerWidth))
        {
            return 0;
        }
        else
        {
            return speed;
        }
    }

    private bool checkFront()
    {
        if(world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x, transform.position.y + 1.0f, transform.position.z + playerWidth))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool checkBack()
    {
        if (world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x, transform.position.y + 1.0f, transform.position.z - playerWidth))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool checkLeft()
    {
        if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1.0f, transform.position.z))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool checkRight()
    {
        if (world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 1.0f, transform.position.z))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
