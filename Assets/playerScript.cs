using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { NONE, UP, DOWN, LEFT, RIGHT };

public enum States {NONE, MOVE, JUMP, FALL, ATK, DASH}

public class playerScript : MonoBehaviour
{
    private BoxCollider2D box2D;
    private Rigidbody2D rigidbody;
    private Animator anim;
    private SpriteRenderer spr;
    private InputManager inputMan;
    

    private Vector2 spdVector;
    private Vector2 prevSpd;

    private Direction moveDir;

    private int RunningID;
    private int FallID;
    private int JumpingID;


    public bool isRunning;
    public bool isJumping;
    public bool wasJumping;

    public bool falling;

    public bool jumpPerformed;
    public bool canWallJump;
    public Direction jumpDir;

    public float moveSpeed = 10;
    public float jumpSpeed = 300;

    // Start is called before the first frame update
    void Start()
    {
        box2D = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        RunningID = Animator.StringToHash("is_running");
        JumpingID = Animator.StringToHash("jump");
        FallID = Animator.StringToHash("falling");

        isRunning = false;
        isJumping = false;
        wasJumping = false;
        jumpPerformed = false;
        canWallJump = false;
        falling = false;

        moveDir = Direction.NONE;
        inputMan = InputManager.Instance;
        
            
       }

    // Update is called once per frame
    void Update()
    {
        moveDir = Direction.NONE;
        isRunning = false;


        
        if (inputMan.ButtonDown[(int)GameInputs.RIGHT])
        {
            isRunning = true;
            moveDir = Direction.RIGHT;
            spr.flipX = false;
        }
        if (inputMan.ButtonDown[(int)GameInputs.LEFT])
        {
            isRunning = true;
            moveDir = Direction.LEFT;
            spr.flipX = true;
        }

   
        if (!isJumping )
        {
            if (inputMan.ButtonDown[(int)GameInputs.JUMP])
            {
                jumpPerformed = true;
                isJumping = true;
            }
        }
        
        //anim.SetBool(RunningID, isRunning);

        if (wasJumping != isJumping)
        {
            if (isJumping == jumpPerformed) { anim.SetBool(JumpingID, isJumping); }
            else { anim.SetBool(FallID, falling); }
        }
        
        wasJumping = isJumping;
    }
    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime * 1000;
        spdVector.y = rigidbody.velocity.y;
        switch (moveDir)
        {
            default:
                spdVector.x = 0;
                break;
            case Direction.RIGHT:
                spdVector.x = moveSpeed * delta;
                break;
            case Direction.LEFT:
                spdVector.x = -moveSpeed * delta;
                break;
        }
        rigidbody.velocity = spdVector;
        if (rigidbody.velocity.y < 0)
        {
            falling = true;
        }
        if (isJumping && !jumpPerformed)
        {
            jumpPerformed = true;
            float jumpSpdX = 0;
            if (jumpDir == Direction.LEFT)
            {
                jumpSpdX = -jumpSpeed * delta;
                spr.flipX = true;
            }
            else if (jumpDir == Direction.RIGHT)
            {
                jumpSpdX = jumpSpeed * delta;
                spr.flipX = false;
            }
            rigidbody.AddForce(new Vector2(jumpSpdX, (jumpSpeed*100) * delta));
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Scenario")
        {
            isJumping = true;
            canWallJump = false;
            jumpDir = Direction.NONE;
        }
    }


    private bool checkRaycastWithScenario(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Scenario")
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
        if (collision.gameObject.tag == "Scenario")
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

               

                RaycastHit2D[] hits = Physics2D.RaycastAll(bottomCenterPos, -Vector2.up, 2);
                col1 = checkRaycastWithScenario(hits);

                if (!col1)
                {
                    hits = Physics2D.RaycastAll(bottomLeftPos, -Vector2.up, 2);
                    col2 = checkRaycastWithScenario(hits);
                }
                if (!col2)
                {
                    hits = Physics2D.RaycastAll(bottomRightPos, -Vector2.up, 2);
                    col3 = checkRaycastWithScenario(hits);
                }
                if (col1 || col2 || col3) { 
                    isJumping = false;
                    falling = false;
                }

               
                
            }
        }
    }

}
