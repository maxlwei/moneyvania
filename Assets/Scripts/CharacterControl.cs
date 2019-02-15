using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterMovement
{
    // mass - mess with acceleration and deceleration
    public float mass;
    
    // maximum speeds in x and y direction
    public float maxHoriSpeed;
    public float maxVertiSpeed;

    // forces used to move rigid bodies
    public float moveForce;
    public float jumpForce;

    // number of seconds after falling until you can no longer jump
    public float jumpLeniency;

    // friction - resistance to motion
    public float airDrag;
    public float groundDrag;

    [NonSerialized]
    public CollisionFlags collisionFlags;
    
    [NonSerialized]
    public Vector2 velocity;


    public CharacterMovement()
    {
        mass = 1f;

        maxHoriSpeed = 5f;
        maxVertiSpeed = 8f;
        moveForce = 200f * mass;
        jumpForce = 200f * mass;

        jumpLeniency = 0.2f;

        airDrag = 0.1f;
        groundDrag = 2000f;

    }
}

// public class InputDir
// {
//     public float hori;
//     public float verti;

//     public InputDir()
//     {
//         hori = Input.GetAxis("Horizontal");
//         verti = Input.GetAxis("Vertical");
//     }
// }

[AddComponentMenu ("Character controller")]
[RequireComponent (typeof (Rigidbody2D))]
public class CharacterControl : MonoBehaviour
{
    [NonSerialized]

    // bool for being on ground or not
    private bool grounded;

    private float lastGroundTime;

    // input vars
    private float hori;
    private bool verti;

    private float prevInput;

    // speed + motion characteristics
    public CharacterMovement movement;

    private BoxCollider2D collider;

    private Rigidbody2D rg2d;
    
    private Animator anime;

   public Transform footPos;
    private CharacterController controller;
    
    void FixedUpdate()
    {
        // constants
        hori =  GetHorizontalInput();
        verti = GetVerticalInput();

        grounded = GetGroundedState(); 

        if(grounded){
            lastGroundTime = Time.time;
        }

        if(verti && (Time.time <  (movement.jumpLeniency + lastGroundTime))){
            rg2d.AddForce(Vector2.up * movement.jumpForce);
        }
        

        // horizontal movement controls 
        rg2d.AddForce(Vector2.right * hori * movement.moveForce);
        if(hori != 0)
        {
            this.GetComponent<Transform>().localScale = new Vector3(hori, 1, 1);
        }
        anime.SetFloat("hori", Math.Abs(hori));

        Debug.Log(GetHorizontalInput());

        if (StoppingCheck(prevInput) && grounded){
            rg2d.drag = movement.groundDrag;
            if(Mathf.Abs(rg2d.velocity.x) < 4){
                rg2d.velocity = Vector2.zero;
            }
        }
        else{
            rg2d.drag = movement.airDrag;
        }

        // ensure movement is within speed limits
        rg2d.velocity = ApplySpeedLimits(rg2d);

        // check if user is decelerating
        prevInput = Input.GetAxis("Horizontal");

    }

    // Start is called after Awake, before first frame
    void Start()
    {
        rg2d.mass = movement.mass;
        anime = this.GetComponent<Animator>();
    }


    // Awake is called before Start update
    void Awake()
    {
        //  anime = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        rg2d = GetComponent<Rigidbody2D>();
    }

    public float GetHorizontalInput()
    {
        float dir = 0;
        if(Mathf.Abs(Input.GetAxis("Horizontal")) != 0){
            dir = Mathf.Sign(Input.GetAxis("Horizontal")) * 1f;
        }
        return dir;
    }
    public bool StoppingCheck(float prevInput)
    {
        bool decelCheck = Mathf.Abs(Input.GetAxis("Horizontal")) < Mathf.Abs(prevInput);
        return decelCheck;
    }

    public bool GetVerticalInput()
    {
        return (Input.GetKey("w")|| Input.GetKey("up"));
    }

    public bool GetGroundedState()
    {
        return Physics2D.OverlapCircle(footPos.position, 0.2f, LayerMask.GetMask("Ground"));
    }

    public Vector2 ApplySpeedLimits(Rigidbody2D body)
    {
        Vector2 velocity = new Vector2(body.velocity.x, body.velocity.y);
        if (Mathf.Abs(body.velocity.x) > movement.maxHoriSpeed){
            velocity = new Vector2(Mathf.Sign(body.velocity.x) * movement.maxHoriSpeed, velocity.y);
        }
        if (Mathf.Abs(body.velocity.y) > movement.maxVertiSpeed){
            velocity = new Vector2(velocity.x, Mathf.Sign(body.velocity.y) * movement.maxVertiSpeed);
        }
        return velocity;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
