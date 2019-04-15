using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    // health counter - text for now but probably will be image based later
    public Text healthText;
    private float health;
    private float defaulthealth = 2f;
    private float invincibleTimer = 0f;
    private float invincibleTime = 2f; // number of seconds before you can be hit again

    public bool isHit = false;

    public bool isDead = false;
    private bool retry;
    private float hitStun = 0.5f;

    public GameObject gameOverScreen;

    void Start()
    {
        health = defaulthealth;

        healthText.text = "Health: " + health.ToString();

        invincibleTime *= 1/Time.fixedDeltaTime;
        hitStun *= 1/Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        if (invincibleTimer > 0f){
            invincibleTimer -= 1f;
        }
        if (invincibleTimer < (invincibleTime - hitStun)){
            isHit = false;
        }
    }

    void Update()
    {
        retry = gameOverScreen.GetComponent<DeathScreen>().tryAgain; 
        if(health == 0){
            isDead = true;
            // DeathScreen death = Instantiate(deathscreen);
            gameOverScreen.SetActive(true);
        }
        if(retry){
            health = defaulthealth;
            SceneManager.LoadScene("Enemy");
        }
        

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.collider.tag == "Enemy" && invincibleTimer <= 0f){
                TakeDamage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy" && invincibleTimer <= 0f){
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health -= 1f;
        healthText.text = "Health: " + health.ToString();
        invincibleTimer = invincibleTime;
        if (health <= 0){
            healthText.text = "You died";
        }
        isHit = true;
    }
}
