using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very basic enemy type
// moves left and right, turns around when it hits any object

// has two collider boxes, one for feet and one for body (to ensure feet collisions don't cause it to turn around)
// alternative method would be to have different tags for the sides and tops of blocks, and check tags when colliding

public class EnemyWalker : MonoBehaviour
{
    public float maxSpeed = 3f; // max movement speed

    private Rigidbody2D rb2d;
    private int direction = 1; // movement direction
    private bool collision = false; // all the stuff with this variable is to prevent a bug where OnCollisonEnter2D is called twice per collision

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(maxSpeed * direction, rb2d.velocity.y); // enemy moves with fixed speed rather than with force
        collision = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.otherCollider.name == "Body" && collision)
            {
                direction *= -1;
                collision = false;
            }
        }
    }
}