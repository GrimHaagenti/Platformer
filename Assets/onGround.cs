using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onGround : MonoBehaviour
{
    public bool isGrounded;
    public bool justLand;
    public  bool jumped;
    private playerScript player;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        justLand = false;
        jumped = false;
        player = GetComponentInParent<playerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        player.isJumping = false;
        
    }
    public void OnTriggerStay2D(Collider2D collision)
    {

        player.grounded = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        player.grounded = false;


    }
}
