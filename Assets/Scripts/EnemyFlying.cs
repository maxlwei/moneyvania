using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very basic enemy type
// moves left and right, turns around when it hits any object
// jumps when it hits the player

// has two collider boxes, one for feet and one for body (to ensure feet collisions don't cause it to turn around)
// alternative method would be to have different tags for the sides and tops of blocks, and check tags when colliding

public class EnemyFlying : MonoBehaviour
{
    public float maxSpeed = 3f; // max movement speed
    public float directionTime = 2f; // number of seconds until vertical direction changes
    public float fireballTime = 3f; // number of seconds between each fireball
    public Fireball fireball;

    private Rigidbody2D rb2d;
    private int directionX = 1; // horizontal direction
    private int directionY = 1; // vertical direction
    private float directionTimer = 0f; // counter for changing vertical direction
    private bool collision = false; // all the stuff with this variable is to prevent a bug where OnCollisonEnter2D is called twice per collision
    private float fireballTimer = 0f;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        fireballTime *= 1/Time.fixedDeltaTime; // convert times to frame counts and set timers
        fireballTimer = fireballTime;

        directionTime *= 1/Time.fixedDeltaTime;
        directionTimer = directionTime;
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(maxSpeed * directionX, maxSpeed * directionY); // enemy moves with fixed speed rather than with force
        collision = true;

        directionTimer -= 1;
        Debug.Log(directionTimer.ToString());
        if (directionTimer <= 0)
        {
            directionTimer = directionTime;
            directionY *= -1;
        }

        fireballTimer -= 1;
        if (fireballTimer <= 0)
        {
            Fireball clone = (Fireball)Instantiate(fireball, transform.position, transform.rotation);
            clone.direction = directionX;
            fireballTimer = fireballTime;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (collision)
            {
                if (contact.otherCollider.name == "Sides")
                {
                    directionX *= -1;
                    collision = false;
                }

                else if (contact.otherCollider.name == "Top/bottom")
                {
                    directionY *= -1;
                    collision = false;
                }
            }
        }
    }
}