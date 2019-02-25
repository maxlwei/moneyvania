using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatController : MonoBehaviour
{
    public GameObject player;
    private bool isDownJumping;

    private CompositeCollider2D collider2D;

    void Awake()
    {
        player = GameObject.Find("default_sprite");
        collider2D = GetComponent<CompositeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isDownJumping = player.GetComponent<CharacterControl>().downJumping;
        if(isDownJumping == true){
            collider2D.offset = Vector2.up * 1000;
        }
        else{
            collider2D.offset = Vector2.zero;
        }

        // Debug.Log(isDownJumping);
    }

}
