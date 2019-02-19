using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterMovement
{
    // mass - mess with acceleration and deceleration
    public float mass;
    
    // maximum speeds in x and y direction
    public float maxHoriSpeed;
    public float maxUpSpeed;
    public float maxDownSpeed;

    // forces used to move rigid bodies
    public float moveForce;
    public float jumpForce;

    // number of seconds after falling until you can no longer jump
    public float jumpLeniency;

    // number of seconds after landing until you can jump
    public float jumpDelay;

    // increased gravity for our girl
    public float gravity;

    [NonSerialized]
    public CollisionFlags collisionFlags;
    
    [NonSerialized]
    public Vector2 velocity;


    public CharacterMovement()
    {
        mass = 1f;

        maxHoriSpeed = 4f;
        maxUpSpeed = 8f;
        maxDownSpeed = -16f;
        moveForce = 200f;
        jumpForce = 300f;

        jumpLeniency = 0.2f;
        jumpDelay = 0.1f;

        gravity = 9.81f;

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
    private float groundedTimer;

    // input vars
    private float hori;
    private float verti;


    // speed + motion characteristics

    //keep this public, for some reason
    public CharacterMovement movement;

    private BoxCollider2D groundCollider;

    private Rigidbody2D rg2d;
    
    private Animator anime;

    public Transform footPos;

    private CharacterController controller;

    // Update is called once per frame
    void Update()
    {
        // reads inputs every frame
        hori =  GetHorizontalInput();
        verti = GetVerticalInput();

    }
    
    void FixedUpdate()
    {
        
        if (hori < 0){
            this.GetComponent<Transform>().localScale = new Vector3(-1, 1, 1);
        } 
        else if (hori > 0){
            this.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        }

        grounded = GetGroundedState();
        AnimatorStateInfo currentstate = anime.GetCurrentAnimatorStateInfo(0);

        // checks last time character was on ground
        if (grounded){
            lastGroundTime = Time.time;
            groundedTimer += 1 * Time.fixedDeltaTime;
        }
        else if(Time.time >  (movement.jumpLeniency + lastGroundTime)){
            groundedTimer = 0;
        }

        // Animator params
        anime.SetFloat("hori", Math.Abs(hori));
        anime.SetFloat("verti", verti);
        float vertvel = rg2d.velocity.y;
        anime.SetFloat("vertvel", rg2d.velocity.y);
        anime.SetBool("grounded", grounded);
        anime.SetBool("nearground", IsNearGround());
        anime.SetFloat("deltafromground", Time.time - lastGroundTime);

        
        if (vertvel < -1.0 && !currentstate.IsName("Jump.jumpdownstall")
           && !currentstate.IsName("Jump.jumpland") && !currentstate.IsName("Jump.fallonly")) {
            anime.Play("Jump.fallonly");
        }

        // checks for upwards input and whether the character was recently grounded
        // before jumping
        if (JumpCheck(verti > 0, lastGroundTime, groundedTimer)){
            anime.Play("Jump.jumpstart");
            rg2d.AddForce(Vector2.up * movement.jumpForce);
        }

        // horizontal movement, now with checks
        if(verti >= -0.1 || !grounded) {
            rg2d.AddForce(Vector2.right * hori * movement.moveForce);
        }
        else{
            rg2d.velocity = new Vector2(0, rg2d.velocity.y); 
        }
        
        if(StoppingCheck(hori)){
            // set velocity to 0 if no horizontal input is read
            rg2d.velocity = new Vector2(0, rg2d.velocity.y);
        }

        // extra gravity
        rg2d.AddForce(Vector2.down * rg2d.mass * movement.gravity);

        // ensure movement is within speed limits and adjust
        rg2d.velocity = ApplySpeedLimits(rg2d);
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

        // gets component from the child
        foreach(BoxCollider2D collider in GetComponentsInChildren<BoxCollider2D>()){
            groundCollider = collider;
        }
        rg2d = GetComponent<Rigidbody2D>();
    }

    public float GetHorizontalInput()
    {
        // if a direction is inputted -> return 1 in that direction
        // if not, return 0

        if(Input.GetKey("a") || Input.GetKey("left")){
            return -1;
        }
        if(Input.GetKey("d") || Input.GetKey("right")){
            return 1;
        }
        return 0;
    }

    public bool StoppingCheck(float Input)
    {
        // current input is less than previous is zero -> stopping
        bool decelCheck = Input == 0;
        return decelCheck;
    }

    public bool JumpCheck(bool input, float groundTime, float timer)
    {
        bool canJump = timer > movement.jumpDelay;
        bool walkoff = Time.time <  (movement.jumpLeniency + groundTime);
        return input && walkoff && canJump;
    }

    public float GetVerticalInput()
    {
        // uses keys for jumping instead of axes - to prevent residual input
        return (Input.GetAxis("Jump"));
    }

    public bool GetGroundedState()
    {
        // uses a circle positioned at body feet to detect contact with ground
        // return Physics2D.OverlapCircle(footPos.position, 0.01f, LayerMask.GetMask("Background"));

        // uses a collider on footpos
        return groundCollider.IsTouchingLayers(LayerMask.GetMask("Background"));
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
        if (body.velocity.y > movement.maxUpSpeed) {
            velocity = new Vector2(velocity.x, movement.maxUpSpeed);
        }
        else if (body.velocity.y < movement.maxDownSpeed) {
            velocity = new Vector2(velocity.x, movement.maxDownSpeed);
        }
        return velocity;
    }
}
