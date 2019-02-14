using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float jumpSpeed = 10f; //jump speed
	public float jumpLeniency = 1f; //number of seconds after falling until you can no longer jump
	public int jumpMax = 1; //maximum number of jumps
    public float moveForce = 100f; //horizontal movement acceleration in movementType 1 or 2
    public float maxSpeed = 10f; //maximum horizontal movement speed
	public float slowFactorAir = 1f; //air momentum factor for moveType 1 (bigger means more rapid deceleration)
	public float slowFactorGround = 10f; //momentum factor for moveType 1 (bigger means more rapid deceleration)
	public Transform feetPos; //variable to store feet position for grounded check (set manually in Inspector)

	private bool jump = false;
	private int jumpsMade = 0;
	private bool grounded = false;
	private bool groundedRecent = false;
	private float groundedCounter = 0f;
	private Rigidbody2D rb2d;

    void Awake () 
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

	void Update()
	{
		if (Input.GetButtonDown("Jump") && (groundedRecent || jumpsMade < jumpMax))
        {
            jump = true;
			groundedRecent = false;
			jumpsMade ++;
        }
	}

    void FixedUpdate()
    {
		//horizontal movement, depends on movementType
        float h = Input.GetAxis("Horizontal");

		if (h == 0)
		{
			if (grounded == true)
			{
				rb2d.AddForce(new Vector2(-rb2d.velocity.x * slowFactorGround, 0));
			}
			else
			{
				rb2d.AddForce(new Vector2(-rb2d.velocity.x * slowFactorAir, 0));
			}
		}

		if (h * rb2d.velocity.x < maxSpeed)
			rb2d.AddForce(Vector2.right * h * moveForce);

		if (Mathf.Abs (rb2d.velocity.x) > maxSpeed)
			rb2d.velocity = new Vector2(Mathf.Sign (rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

		//jump function, including calculations for grounded/grounded recently
		grounded = Physics2D.OverlapCircle(feetPos.position, 0.01f, 1 << LayerMask.NameToLayer("Ground"));

		if (jump)
        {
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            jump = false;
			grounded = false;
			groundedRecent = false;
        }

		if (grounded)
		{
			groundedRecent = true;
			groundedCounter = (1/Time.fixedDeltaTime) * jumpLeniency;
			jumpsMade = 0;
		}

		else
		{
			groundedCounter --;
			if (groundedCounter <= 0)
			{
				groundedRecent = false;
				groundedCounter = 0;
			}
		}	
    }
}