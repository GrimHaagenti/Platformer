using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum States {IDLE, MOVE, JUMP, FALL, ATK, DASH}

public class playerScript : MonoBehaviour
{
    private BoxCollider2D box2D;
    private Rigidbody2D rigidbody;
    private InputManager inputMan;
    private SpriteRenderer spr;
    [SerializeField] GameObject atk;
    private Animator anim;

    private Vector2 spdVector;
    private Vector2 prevSpd;

    private float x_motion;

    public bool isRunning;
    public bool isJumping;
    public bool wasJumping;
    public bool grounded;

    public bool attacking;

    public bool falling;

    public States state;


    public bool jumpPerformed;
    public bool canWallJump;


    public float moveSpeed = 10;
    public float jumpHeight = 68;
    public float timeToPeak = 0.6f;
    public float timeToPeak2 = 0.4f;
    private float jumpSpeed;
    private float up_grav;
    private float down_grav;

    public bool anim_finish;
    int[] animHash = new int[(int)GameInputs.LAST_ACTION];
    public float fall_grav = 20;

    // Start is called before the first frame update
    void Start()
    {
        box2D = GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        

        isRunning = false;
        isJumping = false;
        wasJumping = false;
        jumpPerformed = false;
        canWallJump = false;
        falling = false;
        grounded = false;
        anim_finish = true;

        animHash[(int)GameInputs.SLASH] = Animator.StringToHash("atk1");


        inputMan = InputManager.Instance;
        up_grav = -2 * jumpHeight / (timeToPeak * timeToPeak);
        down_grav = -2 * jumpHeight / (timeToPeak2 * timeToPeak2);
        jumpSpeed = 2 * jumpHeight / timeToPeak;
        rigidbody.gravityScale = down_grav / Physics2D.gravity.y;
    }

    // Update is called once per frame

   
    void Update()
    {
        if (rigidbody.velocity == Vector2.zero )
        {
            if (state != States.ATK)
            {
                state = States.IDLE;
               
                    anim.Play("idle");

                   
                

            }
        
        }
        
        


        

        x_motion = Input.GetAxis("Horizontal");
       

        
        
       
        
  
        switch(state)
        {
            case States.IDLE:
                move();
                jump();
                atkF();
                break;

            case States.MOVE:

                jump(); 
                move();
                atkF();

                break;


            case States.JUMP:
                move();
                atkF();
                break;

            case States.FALL:
                move();
                break;

            case States.ATK:
            

                atkF();
                break;

            case States.DASH:
                break;



        }
        //anim.SetBool(RunningID, isRunning);




    }

    private void idle()
    {
        if (rigidbody.velocity == Vector2.zero)
        {
                state = States.IDLE;

                anim.Play("idle");

         
        }

    }

    private void move()
    {
        
        if (x_motion != 0)
        {
            state = States.MOVE;


            isRunning = true; 
            if (x_motion < 0)
            {
                spr.flipX = true;
            }
            else { spr.flipX = false; }


            if (grounded ) { anim.Play("run"); }
        }
        else { rigidbody.velocity = new Vector2(0, rigidbody.velocity.y); }
        
    }

    private void jump() {
        if (inputMan.ButtonPressed[(int)GameInputs.JUMP])
        {
            state = States.JUMP;
            anim.Play("jump");
            rigidbody.gravityScale = up_grav / Physics2D.gravity.y;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpSpeed);


        }
    }
    private void atkF() {
        if (inputMan.ButtonPressed[(int)GameInputs.SLASH])
        {
            
            state = States.ATK;
            rigidbody.velocity = Vector2.zero;
            anim.Play("atk1");

        }

    }
        private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime * 1000;
        spdVector.y = rigidbody.velocity.y;


        if (isRunning)
        {
            spdVector.x = x_motion * moveSpeed * delta;
        }
        if (!grounded)
        {
            spdVector.y -= fall_grav;


            if (rigidbody.velocity.y < 0)
            {
                spdVector.y -= fall_grav;

                falling = true;
            }
        }
      
        
     
   
      

        rigidbody.velocity = spdVector;
    }

    //    switch (moveDir)
    //    {
    //        default:
    //            spdVector.x = 0;
    //            break;
    //        case Direction.RIGHT:
    //            spdVector.x = moveSpeed * delta;
    //            break;
    //        case Direction.LEFT:
    //            spdVector.x = -moveSpeed * delta;
    //            break;
    //    }
    //    rigidbody.velocity = spdVector;
    //    if (rigidbody.velocity.y < 0)
    //    {
    //        falling = true;
    //    }
    //    if (isJumping )
    //    {
    //        jumpPerformed = true;
    //        float jumpSpdX = 0;
    //        if (jumpDir == Direction.LEFT)
    //        {
    //            jumpSpdX = -jumpSpeed * delta;
    //            spr.flipX = true;
    //        }
    //        else if (jumpDir == Direction.RIGHT)
    //        {
    //            jumpSpdX = jumpSpeed * delta;
    //            spr.flipX = false;
    //    //    }
    //        rigidbody.AddForce(new Vector2(jumpSpdX, (jumpSpeed*100) * delta));


    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    //if (collision.gameObject.tag == "Scenario")
    //    //{
    //    //    isJumping = true;
    //    //    canWallJump = false;
    //    //    jumpDir = Direction.NONE;
    //    //}
    //}


    private bool checkRaycastWithScenario(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "tiles")
                {
                    Debug.Log("WWWWWWWW");

                    return true;
                }
            }
        }
        return false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tiles")
        {
            if (isJumping)
            {
                bool col1 = false;
                bool col2 = false;
                bool col3 = false;

                float center_x = (box2D.bounds.min.x + box2D.bounds.max.x) / 2;

                Vector2 bottomCenterPos = new Vector2(center_x, box2D.bounds.min.y);
                Vector2 bottomLeftPos = new Vector2(box2D.bounds.min.x, box2D.bounds.min.y);
                Vector2 bottomRightPos = new Vector2(box2D.bounds.max.x, box2D.bounds.min.y);



                RaycastHit2D[] hits = Physics2D.RaycastAll(bottomCenterPos, -Vector2.up, 0.1f);
                col1 = checkRaycastWithScenario(hits);

                if (!col1)
                {
                    hits = Physics2D.RaycastAll(bottomLeftPos, -Vector2.up, 0.1f);
                    col2 = checkRaycastWithScenario(hits);
                }
                if (!col2)
                {
                    hits = Physics2D.RaycastAll(bottomRightPos, -Vector2.up, 0.1f);
                    col3 = checkRaycastWithScenario(hits);
                }
                if (col1 || col2 || col3)
                {
                    grounded = true;
                    falling = false;
                }
                else { grounded = false; 
                }

            }

            
        }
    }

}
