using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // References
    private CharacterController _cc;
    private PlayerInput _playerInput;
    private Animator _animator;
    private Vector3 _moveVelocity;
    private Health _health;
    private DamageCaster _damageCaster;
    // Player Variables
    public float moveSpeed = 5f;
    private float verticalVelocity;
    public float gravity;
    // Enemy variables
    public bool isPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform targetPlayer;
    // state machine
    public enum CharacterState {
        Normal, 
        Attaking,
    }
    public CharacterState currentState;
    // Player slide
    private float attackStartTime;
    public float attackSlideDuration = 0.4f;
    public float attackSlideSpeed = 0.6f;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        if (!isPlayer){
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
        }
        else{
            _playerInput = GetComponent<PlayerInput>();
        }
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
    }

    private void CalculatePlayerMovement() {
        if(_playerInput.mouseButtonDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Attaking);
            return; // evitar que se ejecute las siguientes líneas de código
        }
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
    
    private void CalculateEnemyMovement(){
        if ( Vector3.Distance(targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance ) {
            _navMeshAgent.SetDestination(targetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            SwitchStateTo(CharacterState.Attaking);
        }
    }
    
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    protected void FixedUpdate() {
        switch(currentState) {
        case CharacterState.Normal:
            // Verificar si es player of enemy
            if(isPlayer){
                CalculatePlayerMovement();
            }
            else{
                CalculateEnemyMovement();
            }
            break;
        case CharacterState.Attaking:
            Debug.Log("is Attacking");
            // slide when attacking
            if (isPlayer){
                _moveVelocity = Vector3.zero;
                if( Time.time < attackStartTime + attackSlideDuration ) {
                    float passedTime = Time.time - attackStartTime;
                    float lerpTime = passedTime / attackSlideDuration;
                    _moveVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                }
            }
            break;
        }
        // gravity
        if (isPlayer) {
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
    
    public void SwitchStateTo( CharacterState newState ) {
        // clear input cache
        if (isPlayer) {
            _playerInput.mouseButtonDown = false;
        }
        // exiting State
        switch(currentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attaking:
                break;
        }
        // entering state
        switch(newState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attaking:
                if (!isPlayer){
                    Quaternion newRotation = Quaternion.LookRotation(targetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                _animator.SetTrigger("Attack");
                if (isPlayer){
                    attackStartTime =Time.time;
                }
                break;
        }
        currentState = newState;
        Debug.Log("switched to: " + currentState);
    }
    
    public void AttackAnimationEnds() {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void ApplyDamage(float damage, Vector3 attackerPos = new Vector3()){
        if(_health != null){
            _health.ApplyDamage(damage);
        }
    }
    
    public void EnableDamageCaster(){
        _damageCaster.EnableDamageCaster();
    }
    
    public void DisableDamageCaster(){
        _damageCaster.DisableDamageCaster();
    }
}
