﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //Variables
    public float horizontalInput;
    public float verticalInput;
    public bool mouseButtonDown;
    
    // Update is called once per frame
    void Update() {
        if (!mouseButtonDown && Time.timeScale != 0) {
            mouseButtonDown = Input.GetMouseButtonDown(0);
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    
    // This function is called when the behaviour becomes disabled () or inactive.
    protected void OnDisable() {
        mouseButtonDown = false;
        horizontalInput = 0;
        verticalInput = 0;
    }
}
