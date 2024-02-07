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
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    // Player Variables
    private float verticalVelocity;
    private Vector3 impactOnCharacter;
    public float moveSpeed = 5f;
    public float gravity;
    public bool isInvincible;
    public float invincibleDuration = 2f;
    // Enemy variables
    public bool isPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform targetPlayer;
    public GameObject itemToDrop;
    // state machine
    public enum CharacterState {
        Normal, 
        Attaking,
        dead,
        beingHit,
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
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
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
                //_moveVelocity = Vector3.zero;
                if( Time.time < attackStartTime + attackSlideDuration ) {
                    float passedTime = Time.time - attackStartTime;
                    float lerpTime = passedTime / attackSlideDuration;
                    _moveVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                }
            }
            break;
        case CharacterState.dead:
            return;
        case CharacterState.beingHit:
            if (impactOnCharacter.magnitude > 0.2f){
                _moveVelocity = impactOnCharacter * Time.deltaTime;
                impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);
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
            _moveVelocity = Vector3.zero;
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
                if (_damageCaster != null){
                    DisableDamageCaster();
                }
                break;
            case CharacterState.dead:
                return;
            case CharacterState.beingHit:
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
            case CharacterState.dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                DropItem();
                break;
            case CharacterState.beingHit:
                _animator.SetTrigger("beingHit");
                if(isPlayer){
                    isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
        }
        currentState = newState;
        Debug.Log("switched to: " + currentState);
    }
    
    public void AttackAnimationEnds() {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void BeingHitAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void ApplyDamage(float damage, Vector3 attackerPos = new Vector3()){
        if (isInvincible) {
            return;
        }
        
        if(_health != null){
            _health.ApplyDamage(damage);
        }
        
        if (!isPlayer) {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }
        
        StartCoroutine(MaterialBlink());
        
        if (isPlayer) {
            SwitchStateTo(CharacterState.beingHit);
            AddImpact(attackerPos, 10f);
        }
    }
    
    public void AddImpact(Vector3 attackerPos, float force) {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }
    
    public void EnableDamageCaster(){
        _damageCaster.EnableDamageCaster();
    }
    
    public void DisableDamageCaster(){
        _damageCaster.DisableDamageCaster();
    }
    
    public void DropItem(){
        if (itemToDrop != null) {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }
    
    IEnumerator DelayCancelInvincible(){
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }
    
    IEnumerator MaterialBlink () {
        _materialPropertyBlock.SetFloat("_blink",0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        yield return new WaitForSeconds(0.2f);
        _materialPropertyBlock.SetFloat("_blink",0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    
    IEnumerator MaterialDissolve() {
        yield return new WaitForSeconds(2f);
        
        // VFX variables
        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHigh_start = 20f;
        float dissolveHigh_target  = -10f;
        float dissolveHigh;
        
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        
        while (currentDissolveTime < dissolveTimeDuration) {
            
            currentDissolveTime += Time.deltaTime;
            dissolveHigh = Mathf.Lerp(dissolveHigh_start, dissolveHigh_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHigh);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
    }
}
