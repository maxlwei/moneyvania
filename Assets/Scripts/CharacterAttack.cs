using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    // to control attack cooldowns
    public float attackCD = 1f;
    private float attackState;

    // for reading velocity/direction
    private Rigidbody2D rb2d;
    private CharacterControl movement;

    // attack 1 - melee attack
    public BasicAttack attack1;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {    
        attackState = attackCD;
        movement = GetComponent<CharacterControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("e") && (attackState > (attackCD - 0.01f))){
            MeleeAttack();
        }

        if(attackState < (attackCD + 0.1)){
            attackState += Time.deltaTime;
        }
    }

    void MeleeAttack()
    {
        BasicAttack battack = Instantiate(attack1, transform.position + Vector3.right * movement.facingDirection, transform.rotation);
        battack.duration = 1;
        battack.battackDirection = movement.facingDirection;
        
        attackState = 0;
    }
}
