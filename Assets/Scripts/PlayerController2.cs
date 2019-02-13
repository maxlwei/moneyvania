using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
	public int movementType = 0;
	//0 for instant stop/start on ground and in air
	//1 for momentum in ground and on air
	//2 for instant stop/start on ground, momentum in air

	public float jumpSpeed = 20f;
    public float moveForce = 100f; //how quickly the character accelerates in movementType 1 or 2
    public float maxSpeed = 10f;
	public float slowFactor = 1f; //how quickly the character slows down in movementType 1 or 2
	public float jumpLeniency = 1f; //number of seconds after falling until you can no longer jump

	private bool jump = false;
	private bool grounded = true;
	private bool groundedRecent = true;
	private float groundedCounter = 0f;
	private Rigidbody2D rb2d;
	public Transform feetPos;

    void Awake () 
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

	void Update () 
    {
        //grounded = Physics2D.Linecast(transform.position, groundCheck.position, LayerMask.NameToLayer("Ground"));
		//isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius, whatIsGround);

        if (Input.GetButtonDown("Jump") && groundedRecent)
        {
            jump = true;
        }
	}

    void FixedUpdate()
    {
		//calculations for grounded/grounded recently, used in jump function
		grounded = Physics2D.OverlapCircle(feetPos.position, 0.5f, 1 << LayerMask.NameToLayer("Ground"));

		if (grounded)
		{
			groundedRecent = true;
			groundedCounter = 50f * jumpLeniency;
		}

		else
		{
			groundedCounter -= 1;
			if (groundedCounter == 0)
			{
				groundedRecent = false;
			}
		}

		//horizontal movement, depends on movementType (could probbaly be simplified)
        float h = Input.GetAxis("Horizontal");

		if (movementType == 0)
		{
			if (h == 0)
				rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

			if (h != 0)
				rb2d.velocity = new Vector2(Mathf.Sign(h)*maxSpeed, rb2d.velocity.y);
		}

        if (movementType == 1)
		{
			if (h == 0)
				rb2d.AddForce(new Vector2(-rb2d.velocity.x * slowFactor, rb2d.velocity.y));
			
			if (h * rb2d.velocity.x < maxSpeed)
            	rb2d.AddForce(Vector2.right * h * moveForce);

       		if (Mathf.Abs (rb2d.velocity.x) > maxSpeed)
        		rb2d.velocity = new Vector2(Mathf.Sign (rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
		}

		if (movementType == 2)
		{
			if (grounded == true)
			{
				if (h == 0)
					rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

				if (h != 0)
					rb2d.velocity = new Vector2(Mathf.Sign(h)*maxSpeed, rb2d.velocity.y);
			}

			if (grounded == false)
			{
				if (h == 0)
					rb2d.AddForce(new Vector2(-rb2d.velocity.x * slowFactor, rb2d.velocity.y));
				
				if (h * rb2d.velocity.x < maxSpeed)
					rb2d.AddForce(Vector2.right * h * moveForce);

				if (Mathf.Abs (rb2d.velocity.x) > maxSpeed)
					rb2d.velocity = new Vector2(Mathf.Sign (rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
			}
		}

		//jump function
        if (jump)
        {
			rb2d.velocity = new Vector2(0f, jumpSpeed);
            jump = false;
			groundedRecent = false;
        }
    }
}