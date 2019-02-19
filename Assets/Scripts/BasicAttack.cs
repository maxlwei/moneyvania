using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    // follows the player at a certain range
    public GameObject player;
    private Rigidbody2D playerVel;
    private Rigidbody2D attackBody;

    // spacing of attack
    private Vector3 offset;
    private Vector3 range;

    // duration of attack
    public float duration = 1;
    private float timeoutDestructor;

    void Update()
    {
        // update direction of attack if player is moving
        if(Mathf.Abs(playerVel.velocity.x) > 0){
            range = new Vector3(Mathf.Sign(playerVel.velocity.x) * 1f, 0, 0);
        }

        // follow player
        transform.position = followPlayer(range);
    }

    void FixedUpdate()
    {
        // after a time (duration) has expired, destroy attack
        timeoutDestructor -= 1;

        if(timeoutDestructor < 1){
            Destroy(gameObject);
        }
    }


    void Awake()
    {
        player = GameObject.Find("default_sprite");

        // get the rigidbody from the component
        playerVel = player.GetComponent<Rigidbody2D>();

        attackBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        offset = transform.position - player.transform.position;
        attackBody.isKinematic = true;
        timeoutDestructor = duration / Time.fixedDeltaTime;
    }

    public Vector3 followPlayer(Vector3 range)
    {
        return (player.transform.position + offset + range);
    }

}