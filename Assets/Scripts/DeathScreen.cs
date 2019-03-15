using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public bool tryAgain = false;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            tryAgain = true;
        }
    }

}
