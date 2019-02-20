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
    public float maxSpeed = 3f; // maximum speed for horizontal and vertical movement
    public float verticalForce = 100f; // force for returning to vertical position
    public float verticalLeniency = 0.1f; // distance before it readjusts vertical position
    public float fireballTime = 3f; // minimum time between fireballs
    public float detectionDistance = 10f; // radius within which the enemy can detect you
    public float trackForce = 50f; // force for movement when tracking player
    public Fireball fireball; // drag the Fireball prefab into this box
    public Transform posPlayer; // drag the scene's player instance's sprite into this box

    private Rigidbody2D rb2d;
    private Transform transform;
    private float posY = 0f;
    private bool collision = false; // to prevent bug where OnCollisionEnter triggers twice per collision
    private float fireballTimer = 0f; // counter for time between fireballs
    private int directionX = 1; // horizontal movement direction

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

        Vector2 toPlayer = new Vector2(posPlayer.position.x - transform.position.x, posPlayer.position.y - transform.position.y); // first get vector from enemy to player

        if (PlayerNearby(toPlayer)) // enter appropriate movement mode based on whether it spots the player
        {
            MovementTracking(rb2d, toPlayer);
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

    public bool PlayerNearby(Vector2 toPlayer) // check if player is within range and not behind wall
    {
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, toPlayer, detectionDistance, LayerMask.GetMask("Player")); // raycast to check if player is within range of enemy
        if (hitPlayer.collider != null)
        {
            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, toPlayer, detectionDistance, LayerMask.GetMask("Terrain")); // raycast to check if wall is between player and enemy
            if (hitWall.collider == null)
            {
                return true; // only returns true if both checks pass, i.e. player within range and not behind wall
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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

    public void MovementTracking(Rigidbody2D rb2d, Vector2 toPlayer) // movement pattern when tracking player
    {
        rb2d.AddForce(toPlayer * trackForce);

        rb2d.velocity = new Vector2(ApplySpeedLimit(rb2d.velocity.x, maxSpeed), ApplySpeedLimit(rb2d.velocity.y, maxSpeed));

        directionX = (int)Mathf.Sign(rb2d.velocity.x);

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionX * Vector2.right, Mathf.Infinity, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float ApplySpeedLimit(float speed, float limit) // reduces input speed to speed limit if input speed is greater than speed limit
    {
        if ((speed < 0) && (speed < -limit))
        {
            return -limit;
        }
        else if ((speed > 0) && (speed > limit))
        {
            return limit;
        }
        else
        {
            return speed;
        }
    }
}