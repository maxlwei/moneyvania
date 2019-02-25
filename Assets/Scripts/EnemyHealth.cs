using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public TextMesh healthText;
    public float health = 3f; // maximum hitpoints
    private bool invincible = false; // to prevent glitch where OnCollisionEnter can trigger twice per collision

    void Start()
    {
        healthText.text = "Health: " + health.ToString();
    }

    void FixedUpdate()
    {
        invincible = false;
    }

    // all 4 functions below are for taking damage from different sources - hitting colliders, staying in colliders, hitting triggers, and staying in triggers
    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (CheckTags(contact.collider.tag) && invincible == false)
            {
                TakeDamage();
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (CheckTags(contact.collider.tag) && invincible == false)
            {
                TakeDamage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (CheckTags(col.tag) && invincible == false)
        {
            TakeDamage();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (CheckTags(col.tag) && invincible == false)
        {
            TakeDamage();
        }
    }

    public void TakeDamage() // function that subtracts 1 from hitpoints, updates health text, and deletes enemy at 0 health
    {
        health -= 1f;
        healthText.text = "Health: " + health.ToString();
        invincible = true;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool CheckTags(string tag) // checks tags to see if enemy can be damaged by entity
    {
        if ((tag == "Attack") || (tag == "AllDamage"))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
