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
    public float movingDrag;
    public float stoppingDrag;


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

        movingDrag = 0.1f;
        stoppingDrag = 2000f;

    }
}

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
        // reads inputs every cycle
        hori =  GetHorizontalInput();
        verti = GetVerticalInput();

        if (hori < 0)
        {
            this.GetComponent<Transform>().localScale = new Vector3(-1, 1, 1);
        } else if (hori > 0)
        {
            this.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        }

        grounded = GetGroundedState();
        AnimatorStateInfo currentstate = anime.GetCurrentAnimatorStateInfo(0);

        // checks last time character was on ground
        if (grounded){
            lastGroundTime = Time.time;
        }

        // Animator params
        anime.SetFloat("hori", Math.Abs(hori));
        anime.SetBool("jump", verti);
        float vertvel = rg2d.velocity.y;
        anime.SetFloat("vertvel", rg2d.velocity.y);
        anime.SetBool("grounded", grounded);
        anime.SetBool("nearground", IsNearGround());
        anime.SetFloat("deltafromground", Time.time - lastGroundTime);
        if (vertvel < -1.0 && !currentstate.IsName("Jump.jumpdownstall")
           && !currentstate.IsName("Jump.jumpland") && !currentstate.IsName("Jump.fallonly"))
        {
            anime.Play("Jump.fallonly");
        }

        // checks for upwards input and whether the character was recently grounded
        // before jumping
        if (verti && (Time.time <  (movement.jumpLeniency + lastGroundTime))){
            anime.Play("Jump.jumpstart");
            rg2d.AddForce(Vector2.up * movement.jumpForce);
        }
        // horizontal movement, doesnt require checks (for now)
        rg2d.AddForce(Vector2.right * hori * movement.moveForce);

        // increases drag if grounded and stopping
        if (StoppingCheck(prevInput) && grounded){
            rg2d.drag = movement.stoppingDrag; // Very high drag
        }
        else{
            rg2d.drag = movement.movingDrag; // much lower drag (~0.1) for free movement
        }

        // ensure movement is within speed limits and adjust
        rg2d.velocity = ApplySpeedLimits(rg2d);

        // check user input for next cycle
        prevInput = Input.GetAxis("Horizontal");

    }

    // Start is called after Awake, before first frame
    void Start()
    {
        rg2d.mass = movement.mass;
    }


    // Awake is called before Start update
    void Awake()
    {
        // Getting components
        anime = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        rg2d = GetComponent<Rigidbody2D>();
    }

    public float GetHorizontalInput()
    {
        // if a direction is inputted -> return 1 in that direction
        // if not, return 0
        float dir = Mathf.Abs(Input.GetAxis("Horizontal"));
        if(dir != 0){
            return Mathf.Sign(Input.GetAxis("Horizontal")) * 1f;
        }
        return 0;
    }

    public bool StoppingCheck(float prevInput)
    {
        // grounded and not inputting a direction -> should not move
        bool landed = grounded && Input.GetAxis("Horizontal") == 0;

        // current input is less than previous input -> stopping
        // might be redundant
        bool decelCheck = Mathf.Abs(Input.GetAxis("Horizontal")) < Mathf.Abs(prevInput);
        return (decelCheck || landed);
    }

    public bool GetVerticalInput()
    {
        // uses keys for jumping instead of axes - to prevent residual input
        return (Input.GetKey("w")|| Input.GetKey("up"));
    }

    public bool GetGroundedState()
    {
        // uses a circle positioned at body feet to detect contact with ground
        return Physics2D.OverlapCircle(footPos.position, 0.01f, LayerMask.GetMask("Background"));
    }

    public bool IsNearGround()
    {
        // uses a circle positioned at body feet to detect contact with ground
        return Physics2D.OverlapCircle(footPos.position, 0.3f, LayerMask.GetMask("Background"));
    }

    public Vector2 ApplySpeedLimits(Rigidbody2D body)
    {
        // if a velocity in a direction exceeds the maximum velocity, sets
        // the velocity in that direction to the maximum velocity
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
