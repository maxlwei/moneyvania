using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    // health counter - text for now but probably will be image based later
    public Text healthText;
    private float health = 10f;
    private float invincibleTimer = 0f;
    private float invincibleTime = 0.5f; // number of seconds before you can be hit again

    void Start()
    {
        healthText.text = "Health: " + health.ToString();
        invincibleTime *= 1/Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= 1f;
        }
    }

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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy" && invincibleTimer <= 0f)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
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
