using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onGround : MonoBehaviour
{
    public bool isGrounded;
    public bool justLand;
    public  bool jumped;
    private playerScript asas;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = false;
        justLand = false;
        jumped = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        justLand = true;
        Debug.Log("WWWWWWWWWWWWWWWWW");
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
        jumped = false;

    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        jumped = true;
        isGrounded = false;

    }
}
