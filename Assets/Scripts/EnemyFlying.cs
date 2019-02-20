using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// moderately advanced enemy type
// when idle: flies left and right, turns around when it hits any object
// when the player enters a certain distance of it with no walls in the way: enters tracking mode, flies towards player
// also launches fireballs when in tracking mode, but only horizontally

// has five collider boxes - two for top/bottom, two for left/right sides, and one for main body (to prevent glitching through walls)
// separate left/right and top/bottom colliders to determine where collisions occur
// alternative method would be to have different tags for the sides and tops of blocks, and check tags when colliding

public class EnemyFlying : MonoBehaviour
{
    public float maxSpeed = 3f; // maximum movement speed
    public float verticalForce = 100f; // force for returning to vertical position
    public float verticalLeniency = 0.1f; // distance before it readjusts vertical position
    public float fireballTime = 3f; // minimum time between fireballs
    //public float detectionDistance = 10f; // radius within which the enemy can detect you
    public Fireball fireball;

    private Rigidbody2D rb2d;
    private Transform transform;
    private float posY = 0f;
    private bool collision = false; // to prevent bug where OnCollisionEnter triggers twice per collision
    private float fireballTimer = 0f; // counter for time between fireballs
    private int directionX = 1; // horizontal movement direction
    //private int directionY = 1; // vertical movement direction

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        posY = transform.position.y;
        fireballTime *= 1/Time.fixedDeltaTime; // convert time to frame count
    }

    void FixedUpdate()
    {
        collision = true;

        if (fireballTimer > 0) // fireball timer ticks down regardless of movement mode
        {
            fireballTimer --;
        }

        if (PlayerNearby())
        {
            MovementTracking(rb2d);
        }
        else
        {
            MovementIdle(rb2d);
        }
    }

    void OnCollisionEnter2D(Collision2D col) // turns around when it hits an object
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if(collision && contact.otherCollider.name == "Sides")
            {
                directionX *= -1;
                collision = false;
            }
        }
    }

    public bool PlayerNearby() // check if player is within range and not behind wall
    {
        //2d raycast from enemy in player direction, with distance limit
        //check if raycast hits a wall
        return false;
    }

    public void MovementIdle(Rigidbody2D rb2d) // movement pattern when idle
    {
        rb2d.velocity = new Vector2(maxSpeed * directionX, 0); // move sideways with constant speed

        if (transform.position.y < (posY - verticalLeniency)) // adjust vertical position if it has changed
        {
            rb2d.AddForce(transform.up * verticalForce);
        }
        else if (transform.position.y > (posY + verticalLeniency))
        {
            rb2d.AddForce(transform.up * -1f * verticalForce);
        }
    }

    public void MovementTracking(Rigidbody2D rb2d) // movement pattern when tracking player
    {
        //move towards player (how)

        if (PlayerInSight() && fireballTimer <= 0)
        {
            AttackFireball();
        }
    }

    public void AttackFireball() // launch a fireball
    {
        Fireball clone = (Fireball)Instantiate(fireball, transform.position, transform.rotation);
        clone.direction = directionX;
        fireballTimer = fireballTime;
    }

    public bool PlayerInSight() // check if player is near same horizontal level as self
    {
        //horizontal 2d raycast
        //check if raycast hits player
        return true;
    }
}