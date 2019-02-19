using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float health = 3f;
    private bool invincible = false; // once again, to prevent glitch where OnCollisionEnter can trigger twice per collision

    void Start()
    {
    }

    void FixedUpdate()
    {
        invincible = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.collider.tag == "Attack" && invincible == false)
            {
                TakeDamage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Attack" && invincible == false)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health -= 1f;
        invincible = true;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
