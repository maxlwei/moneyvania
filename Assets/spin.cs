using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    private Transform transform;
    public float speed = -3f; //degrees per update

    // Start is called before the first frame update
    void Start()
    {
        transform = this.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * speed);
    }
}
