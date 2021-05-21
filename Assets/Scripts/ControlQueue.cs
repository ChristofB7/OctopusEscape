using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ControlQueue : MonoBehaviour
{
    private Queue<List<UInput>> inputs = new Queue<List<UInput>>();
    private Queue<List<InputLog>> inputLog = new Queue<List<InputLog>>();
    private bool recording;

    [SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	//[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround = default;							// A mask determining what is ground to the character
	private Transform g_GroundCheck = default;							// A position marking where to check if the player is grounded.
    [SerializeField] private float m_movespeed = 3f;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private SpriteRenderer m_Renderer;

    private int jumpCount = 15;

    private GameObject ghost;
    private Rigidbody2D g_Rigidbody2D;
    private Transform g_transform;
    private Grapple1 g_claw;
    private bool g_FacingRight = true;
    private bool g_Grounded;            // Whether or not the player is grounded.
    private Vector3 g_Velocity = Vector3.zero;
    private SpriteRenderer g_Renderer;
    private LineRenderer g_LineRenderer;
    
    private int recording_frame_count = 0;
    private int executing_frame_count = 0;

    private Camera currentCamera;   //used to determine mouse postition
    //private Canvas canvas;


    private Animator octoAnimator;

    private Overlay overlay;


    //input flags
    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private bool click = false;
    private bool unclick = false;

    private bool startInput = false;

    private bool practiceMode = false;

    public UnityEvent OnLandEvent;
    

    [System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

    
    enum InputLog
    {
        Left,
        UnLeft,
        Right,
        UnRight,
        Space,
        UnSpace,
        Click,
        UnClick
    }

    // Start is called before the first frame update

    void Awake()
    {
        currentCamera = Camera.main;
        recording = true;
        startInput = false;
        octoAnimator = GetComponent<Animator>();

        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_Renderer = gameObject.GetComponent<SpriteRenderer>();

        Ghost ghosty = (Ghost)FindObjectOfType(typeof(Ghost));
        ghost = ghosty.gameObject;
        g_Rigidbody2D = ghost.GetComponent<Rigidbody2D>();
        g_transform = ghost.transform;
        g_claw = ghost.GetComponent<Grapple1>();
        g_GroundCheck = g_transform.GetChild(0);
        g_Renderer = ghost.GetComponent<SpriteRenderer>();
        g_LineRenderer = ghost.GetComponent<LineRenderer>();

        overlay = (Overlay)FindObjectOfType(typeof(Overlay));
        //Canvas canvas = FindObjectOfType<Overlay>().gameObject;
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if (recording)
        {
            List<InputLog> frameLog = new List<InputLog>();
            if (Input.GetMouseButtonDown(0))
            {
                startInput = true;
                click = true;
                frameLog.Add(InputLog.Click);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                startInput = true;
                unclick = true;
                frameLog.Add(InputLog.UnClick);
            }

            //input log logic, just check when they press a key so we dont record every frame a key is held

            //TODO: Add flags for each letter as they are held
            if (Input.GetKey("a"))
            {
                startInput = true;
                left = true;
                if (Input.GetKeyDown("a"))
                {
                    frameLog.Add(InputLog.Right);
                }
                else if (Input.GetKeyUp("a"))
                {
                    frameLog.Add(InputLog.UnRight);
                }
            }
            if (Input.GetKey("d"))
            {
                startInput = true;
                right = true;
                if (Input.GetKeyDown("d"))
                {
                    frameLog.Add(InputLog.Left);
                }
                else if (Input.GetKeyUp("d"))
                {
                    frameLog.Add(InputLog.UnLeft);
                }
            }

            if (Input.GetKey("space"))
            {
                startInput = true;
                jump = true;
                if (Input.GetKeyDown("space"))
                {
                    frameLog.Add(InputLog.Space);
                }
                else if (Input.GetKeyUp("space"))
                {
                    frameLog.Add(InputLog.UnSpace);
                }
            }

            if (startInput)
            {
                inputLog.Enqueue(frameLog);
            }

        }

        if (Input.GetKeyDown("r"))
        {
            RestartLevel();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        bool wasGrounded = g_Grounded;
        g_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(g_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                g_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    


        if (m_Rigidbody2D.velocity.magnitude > 0)
        {
            octoAnimator.SetBool("isRunning", true);
        }
        else
        {
            octoAnimator.SetBool("isRunning", false);
        }


        //take user input and move ghost
        if (recording)
        {
            recording_frame_count += 1;
            List<UInput> frame = new List<UInput>();
            Move move = new Move(0, this);
            int g_move = 0;
            if (left)
            {
                move.Left();
                g_move -= 1;
                startInput = true;
                left = false;
                //Debug.Log("move left input");


            }

            if (right)
            {
                move.Right();
                g_move += 1;
                startInput = true;
                right = false;
                //Debug.Log("move right input");

            }

            if (g_move > 0 && !g_FacingRight)
            {
                Flip(true);
            }
            else if (g_move < 0 && g_FacingRight)
            {
                Flip(true);
            }

            if (!g_claw.IsGrapple())
            {
                if (g_move != 0)
                {
                    //movement physics
                    // Move the character by finding the target velocity
                    Vector3 targetVelocity = new Vector2(g_move * m_movespeed, g_Rigidbody2D.velocity.y);
                    // And then smoothing it out and applying it to the character
                    g_Rigidbody2D.velocity = Vector3.SmoothDamp(g_Rigidbody2D.velocity, targetVelocity, ref g_Velocity, m_MovementSmoothing);
                }

                if (startInput)
                {
                    frame.Add(move);
                }
            }

            if (jump)
            {
                
                if (g_Grounded && !g_claw.IsGrapple() && jumpCount <= 0)
                {
                    startInput = true;
                    //Debug.Log("ghost jump");
                    g_Grounded = false;      
                    g_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                    jumpCount = 15;
                    frame.Add(new Jump(this));
                }
                jump = false;
            }

            //Grapple stuff
            if (click)
            {
                startInput = true;
                Debug.Log("Click");
                Vector3 mouse = currentCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePosition = new Vector2(mouse.x, mouse.y);
                g_claw.ShootGrapple(mousePosition);
                frame.Add(new Click(mousePosition, true, this));
                click = false;
            } 
            else if (unclick)
            {
                startInput = true;
                Debug.Log("Unclick");
                g_claw.ReleaseGrapple();
                frame.Add(new Click(new Vector2(0,0), false, this));
                unclick = false;
            }


            if (!practiceMode)
            {
                if (Input.GetKey("return"))
                {

                    Debug.Log("stop recording");
                    recording = false;
                }
                if (startInput)
                {
                    inputs.Enqueue(frame);
                }
            }

        }
        else if (CanExec())
        {
           // Debug.Log("execute");
            Execute();
            executing_frame_count += 1;
        }
        else
        {
            StartCoroutine(PlayerLose());
        }

        jumpCount = Mathf.Max(0, jumpCount - 1);
        //see what happens then die or whatever then start recording again
        
    }

    private IEnumerator PlayerLose()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(recording_frame_count + " : " + executing_frame_count);
        Debug.Log("start recording again");
        RestartLevel();    
    }

    private void RestartLevel()
    {
        recording = true;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }


   /* void FixedUpdate() {
        bool wasGrounded = g_Grounded;
		g_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(g_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				g_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
    }*/

    //

    //will throw exception if no things in queue
    public void Execute() 
    {

        List<UInput> frame = inputs.Dequeue();

        foreach (UInput input in frame)
        {
            input.Execute();
        }
    }

    public bool CanExec()
    {
        int count = inputs.Count;
        return count > 0;
    }

    interface UInput 
    {
        void Execute();
    }

    class Move : UInput
    {

        private ControlQueue parent;
        // -1 if left, 0 if no input, 1 if right
        public int dir;


        public Move(int dir, ControlQueue parent)
        {

            this.dir = dir;
            this.parent = parent;
            
        }

        public void Left()
        {
            dir -= 1;
        }

        public void Right()
        {
            dir += 1;
        }

        public void Execute() 
        {
           // Debug.Log("moving somewhere" + dir);
            //movement physics
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(dir * parent.m_movespeed, parent.m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			parent.m_Rigidbody2D.velocity = Vector3.SmoothDamp(parent.m_Rigidbody2D.velocity, targetVelocity, ref parent.m_Velocity, parent.m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (dir > 0 && !parent.m_FacingRight)
			{
				// ... flip the player.
				parent.Flip(false);
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (dir < 0 && parent.m_FacingRight)
			{
				// ... flip the player.
				parent.Flip(false);
			}
        }
    }

    class Click : UInput
    {

        private ControlQueue parent;
        private Vector2 pos;
        bool click;
    
        public Click(Vector2 pos, bool click, ControlQueue parent)
        {
            this.pos = pos;
            this.parent = parent;
            this.click = click;

        }

        public void Execute() 
        {
            Debug.Log("grapple");
            if (click)
            {
                parent.gameObject.GetComponent<Grapple1>().ShootGrapple(pos);
            }
            else
            {
                parent.gameObject.GetComponent<Grapple1>().ReleaseGrapple();
            }

        }
    }

    class Jump : UInput 
    {
        private ControlQueue parent;

        public Jump(ControlQueue parent)
        {
            this.parent = parent;
        }

        public void Execute()
        {
            //Debug.Log("mf jump");
            
            parent.m_Rigidbody2D.AddForce(new Vector2(0f, parent.m_JumpForce));
        }
    }


    //fix this
    public void Flip(bool ghost)
	{
        
        if (!ghost)
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        else
        {
            // Switch the way the player is labelled as facing.
            g_FacingRight = !g_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = g_transform.localScale;
            theScale.x *= -1;
            g_transform.localScale = theScale;
        }
	}

    public void EnablePracticeMode()
    {
        m_Renderer.enabled = false;
        g_LineRenderer.enabled = true;
        g_Renderer.enabled = true; 
        practiceMode = true;
    }
    public void ExitPracticeMode()
    {
        m_Renderer.enabled = true;
        g_LineRenderer.enabled = false;
        g_Renderer.enabled = false;
        practiceMode = false;
    }

}

