using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    public float attackCD = 1f;
    private float attackState;
    private float lastAttack;

    private Rigidbody2D rb2d;

    private CharacterControl movement;

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
            BasicAttack battack = Instantiate(attack1, transform.position, transform.rotation);
            battack.duration = 1;
            battack.battackDirection = movement.facingDirection;
            
            
            attackState = 0;
        }

        if(attackState < (attackCD + 0.1)){
            attackState += Time.deltaTime;
        }
    }
}
