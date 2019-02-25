using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float duration = 5f; // how long the fireball lasts in seconds
    public float speed = 10f; // maximum speed of the fireball
    public int direction = 1; // direction of the fireball (only left/right for now)

    private float timer = 0f;
    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        duration *= 1/Time.fixedDeltaTime;
        timer = duration;
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(speed * direction, 0f);

        timer -= 1;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Attack")
        {
            direction *= -1;
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
