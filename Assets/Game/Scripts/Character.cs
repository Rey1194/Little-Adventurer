using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Variables
    private CharacterController _cc;
    private PlayerInput _playerInput;
    private Animator _animator;
    private Vector3 _moveVelocity;
    public float moveSpeed = 5f;
    private float verticalVelocity;
    public float gravity;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _cc = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
    }

    private void CalculatePlayerMovement() {
        _moveVelocity.Set(_playerInput.horizontalInput, 0, _playerInput.verticalInput);
        _moveVelocity.Normalize();
        _moveVelocity = Quaternion.Euler(0, -45f, 0) * _moveVelocity;
        // Run animation
        _animator.SetFloat("Speed",_moveVelocity.magnitude);
        
        _moveVelocity *= moveSpeed * Time.deltaTime;
        // rotación del jugador
        if (_moveVelocity != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(_moveVelocity);
        }
        // Fall animation
        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }
    
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    protected void FixedUpdate() {
        
        CalculatePlayerMovement();
        
        if (_cc.isGrounded == false){
            verticalVelocity = gravity;
        }
        else {
            verticalVelocity = gravity *0.3f;
        }
        _moveVelocity += verticalVelocity * Vector3.up * Time.deltaTime;
        
        _cc.Move(_moveVelocity);
    }
}
