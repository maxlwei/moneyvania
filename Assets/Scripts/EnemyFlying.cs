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
    public int directionTime = 50; // number of FixedUpdate cycles until vertical direction changes

    private Rigidbody2D rb2d;
    private int directionX = 1; // horizontal direction
    private int directionY = 1; // vertical direction
    private int directionTimer = 0; // counter for changing vertical direction
    private bool collision = false; // all the stuff with this variable is to prevent a bug where OnCollisonEnter2D is called twice per collision

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(maxSpeed * directionX, maxSpeed * directionY); // enemy moves with fixed speed rather than with force
        collision = true;

        directionTimer -= 1;
        if (directionTimer <= 0)
        {
            directionTimer = directionTime;
            directionY *= -1;
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