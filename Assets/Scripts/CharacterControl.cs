using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterMovement
{
    // acceleration and deceleration
    public float horizontalAccel;
    public float verticalAccel;
    
    // maximum speeds in x and y direction
    public float maxHoriSpeed;
    public float maxUpSpeed;
    public float maxDownSpeed;


    // number of seconds after falling until you can no longer jump
    public float jumpLeniency;
    // number of seconds after landing until you can jump
    public float jumpDelay;

    // gravity for our girl
    public float gravity;

    [NonSerialized]
    public CollisionFlags collisionFlags;
    
    [NonSerialized]
    public Vector2 velocity;


    public CharacterMovement()
    {
        horizontalAccel = 4f;
        verticalAccel = 10f;

        maxHoriSpeed = 8f;
        maxUpSpeed = 12f;
        maxDownSpeed = -12f;
        
        jumpLeniency = 0.2f;
        jumpDelay = 0.1f;

        gravity = 8f;
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

    // input variables
    private float hori;
    private float verti;

    // bool for jumping input
    private bool jumpInput;
    // bool to check if jump is released, used for gravity application
    public bool isJumping = false;
    // variable to store time at which jump was first pressed
    private float jumpPressTime;

    // variables for determining direction and downjumping status
    public float facingDirection = 1f;
    public bool downJumping = false;


    #region Components to be accessed
    //keep this public, for some reason
    public CharacterMovement movement;

    private BoxCollider2D groundCollider;

    private Rigidbody2D rg2d;
    
    private Animator anime;

    public Transform footPos;

    private CharacterController controller;
    #endregion

    // Update is called once per frame
    void Update()
    {
        // reads inputs every frame
        hori =  GetHorizontalInput();
        verti = GetVerticalInput();
        jumpInput = GetJumpInput();
    }

    void FixedUpdate()
    {
        
        // change sprite based on direction of last movement
        if (hori < 0){
            this.GetComponent<Transform>().localScale = new Vector3(-1, 1, 1);

            facingDirection = -1f;
        } 
        else if (hori > 0){
            this.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);

            facingDirection = 1f;
        }

        // check for grounded state
        grounded = GetGroundedState();

        AnimatorStateInfo currentstate = anime.GetCurrentAnimatorStateInfo(0);

        // checks last time character was on ground
        if (grounded){
            lastGroundTime = Time.time;
            groundedTimer += 1 * Time.fixedDeltaTime;
        }
        // reset timer if off the ground for long enough
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
        if (JumpCheck(lastGroundTime, groundedTimer) && jumpInput){
            if((verti < 0) && (groundCollider.IsTouchingLayers(LayerMask.GetMask("OnewayPlatform")))){
                downJumping = true;

                // apply gravity for downjumping, checks to avoid overlap with generic gravity
                if(Time.time < lastGroundTime + movement.jumpLeniency){
                    rg2d.velocity = Gravity(rg2d);
                }
            }
            else{
                if(!downJumping){
                    if (!currentstate.IsName("Jump.jumpstart") && !currentstate.IsName("Jump.jumpupstall")){
                        anime.Play("Jump.jumpstart");
                    }

                    // Jumping movement
                    rg2d.velocity = new Vector2(rg2d.velocity.x, rg2d.velocity.y + movement.verticalAccel);
                }
            }
        }

        // Gravity
        if((!isJumping || (Time.time > lastGroundTime + 2 * movement.jumpLeniency)) && !grounded){
            //  gravity active when not jumping
            rg2d.velocity = Gravity(rg2d);
        }

        // resets downjumping state 
        if((verti > -0.1 || !jumpInput) && grounded){
            downJumping = false;
        }

        // horizontal movement, now with checks
        if(verti >= -0.1 || !grounded) {
            rg2d.velocity = new Vector2(rg2d.velocity.x + movement.horizontalAccel * hori, rg2d.velocity.y);
        }
        else{
            // stop on crouch
            rg2d.velocity = new Vector2(0, rg2d.velocity.y); 
        }
        
        if(StoppingCheck(hori)){
            // set velocity to 0 if no horizontal input is read
            rg2d.velocity = new Vector2(0, rg2d.velocity.y);
        }

        // ensure movement is within speed limits and adjust
        rg2d.velocity = ApplySpeedLimits(rg2d);
    }

    // Start is called after Awake, before first frame
    void Start()
    {
        // turn off gravity, we will apply our own
        // we are not making it kinematic to preserve collider behaviors
        rg2d.gravityScale = 0f;
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
        // archaic, should be updated by setting an axis

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

    public float GetVerticalInput()
    {
        // uses instant axes for vertical input instead of default axes - to prevent residual input
        // separate from jumping to allow for downjumping
        return (Input.GetAxis("Vertical"));
    }

    public bool GetJumpInput()
    {
        // gets the initial jump press if grounded
        if(Input.GetButtonDown("Jump") && grounded){
            jumpPressTime = Time.time;
            isJumping = true;
        }

        // checks if jump input is sustained
        if(Input.GetButtonUp("Jump")){
            isJumping = false;
        }

        // returns jump inputs if not grounded and within the jumping window
        if(Time.time < (jumpPressTime + movement.jumpLeniency)){
            return Input.GetButton("Jump");
        }
        
        return false;
    }

    public bool StoppingCheck(float Input)
    {
        // current input is zero -> stopping
        // leave as a function to allow for expansion of stopping
        bool decelCheck = Input == 0;
        return decelCheck;
    }

    public bool JumpCheck(float groundTime, float timer)
    {
        // time from last landing is past jump delay
        bool canJump = timer > movement.jumpDelay;

        // time from last jumping is within jump leniency
        bool walkoff = Time.time <  (movement.jumpLeniency + groundTime);

        return walkoff && canJump;
    }

    public Vector2 Gravity(Rigidbody2D body)
    {
        // apply downwards velocity
        return new Vector2(body.velocity.x, body.velocity.y - movement.gravity);
    }

    public bool GetGroundedState()
    {
        // uses a collider on footpos
        bool onNormalGround = groundCollider.IsTouchingLayers(LayerMask.GetMask("Terrain"));
        bool onOWPlatform = groundCollider.IsTouchingLayers(LayerMask.GetMask("OnewayPlatform"));
        // checks for bothh regular terrain and OWPs, there is probably a better way to do this
        return (onNormalGround || onOWPlatform);
    }

    public bool IsNearGround()
    {
        // uses a circle positioned at body feet to detect contact with ground
        return Physics2D.OverlapCircle(footPos.position, 0.3f, LayerMask.GetMask("Terrain"));
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
