using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    // health counter - text for now but probably will be image based later
    public Text healthText;
    public float health = 10f; // total number of hitpoints (each hit always subtracts 1 hitpoint right now)
    public float invincibleTime = 0.5f; // number of seconds before you can be hit again
    private float invincibleTimer = 0f;

    void Start()
    {
        healthText.text = "Health: " + health.ToString();
        invincibleTime *= 1/Time.fixedDeltaTime; // set invincible timer
    }

    void FixedUpdate()
    {
        if (invincibleTimer > 0f) // tick down invincible timer if it's not 0
        {
            invincibleTimer -= 1f;
        }
    }

    // all 4 functions below are for taking damage from different sources - hitting colliders, staying in colliders, hitting triggers, and staying in triggers
    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.collider.tag == "Enemy" && invincibleTimer <= 0f)
            {
                TakeDamage();
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.collider.tag == "Enemy" && invincibleTimer <= 0f)
            {
                TakeDamage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy" && invincibleTimer <= 0f)
        {
            TakeDamage();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Enemy" && invincibleTimer <= 0f)
        {
            TakeDamage();
        }
    }

    public void TakeDamage() // function that subtracts 1 from hitpoints, updates health text, and starts invincible timer
    {
        health -= 1f;
        healthText.text = "Health: " + health.ToString();
        invincibleTimer = invincibleTime;
        if (health <= 0)
        {
            healthText.text = "You died";
        }
    }
}
