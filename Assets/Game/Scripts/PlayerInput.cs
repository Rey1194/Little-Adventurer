using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //Variables
    public float horizontalInput;
    public float verticalInput;
    
    // Update is called once per frame
    void Update() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    
    // This function is called when the behaviour becomes disabled () or inactive.
    protected void OnDisable() {
        horizontalInput = 0;
        verticalInput = 0;
    }
}
