using UnityEngine;
using UnityEngine.UI;

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

    public Transform highlightBlock;
    public Transform placeHighlightBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8.0f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1;

    

    private void Start()
    {
        playerCam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Locked;
        selectedBlockText.text = world.blockTypes[selectedBlockIndex].Name;
    }

    private void Update()
    {
        GetPlayerInput();
        CheckHighlightBlocks();
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

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0) 
        {
            // scroll 1 to blocktypes list length
            if (scroll > 0) 
            {
                selectedBlockIndex++;
            }
            else
            {
                selectedBlockIndex--;
            }
            if (selectedBlockIndex > (byte)(world.blockTypes.Length - 1))
            {
                selectedBlockIndex = 1;
            }
            if (selectedBlockIndex < 1)
            {
                selectedBlockIndex = (byte)(world.blockTypes.Length - 1);
            }

            selectedBlockText.text = world.blockTypes[selectedBlockIndex].Name;
        }

        if (highlightBlock.gameObject.activeSelf) 
        {
            // destroying block
            if(Input.GetMouseButtonDown(0)) 
            {
                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 3);
            }

            // placing block
            if (Input.GetMouseButtonDown(1))
            {
                world.GetChunkFromVector3(placeHighlightBlock.position).EditVoxel(placeHighlightBlock.position, selectedBlockIndex);
            }
        }
    }

    private void CheckHighlightBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPosition = new Vector3();

        while(step < reach)
        {
            Vector3 lookPos = playerCam.position + (playerCam.forward * step);
            if(world.CheckForVoxel(lookPos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(lookPos.x), Mathf.FloorToInt(lookPos.y), Mathf.FloorToInt(lookPos.z));
                placeHighlightBlock.position = lastPosition;

                highlightBlock.gameObject.SetActive(true);
                placeHighlightBlock.gameObject.SetActive(true);

                return;
            }
            lastPosition = new Vector3(Mathf.FloorToInt(lookPos.x), Mathf.FloorToInt(lookPos.y), Mathf.FloorToInt(lookPos.z));
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeHighlightBlock.gameObject.SetActive(false);
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
