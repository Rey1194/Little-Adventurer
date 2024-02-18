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
    private Vector3 impactOnCharacter;
    private float verticalVelocity;
    private float attackAnimationDuration;
    public float moveSpeed = 5f;
    public float slideSpeed = 9f;
    public float gravity;
    public float invincibleDuration = 2f;
    public bool isInvincible;
    public int totalCoins;
    // Enemy variables
    public bool isPlayer = true;
    public GameObject itemToDrop;
    private float spawnDuration = 2f;
    private float currentSpawntime;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform targetPlayer;
    // state machine
    public enum CharacterState {
        Normal, 
        Attaking,
        dead,
        beingHit,
        slide,
        spawn
    }
    public CharacterState currentState;
    // Player slide
    private float attackStartTime;
    public float attackSlideDuration = 0.4f;
    public float attackSlideSpeed = 0.6f;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        
        if (!isPlayer){
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
            SwitchStateTo(CharacterState.spawn);
        }
        else{
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    private void CalculatePlayerMovement() {
        if(_playerInput.mouseButtonDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Attaking);
            return; // evitar que se ejecute las siguientes líneas de código
        }
        else if (_playerInput.spaceKeyDown && _cc.isGrounded) {
            SwitchStateTo(CharacterState.slide);
            return;
        }
        // movement
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
    
    private void CalculateEnemyMovement() {
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
            // attack animation
            if (isPlayer) {
                
                if (_playerInput.mouseButtonDown && _cc.isGrounded) {
                    string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    // variable con el nombre de la tercera animación del ataque
                    string attackAnimation03 = "LittleAdventurerAndie_ATTACK_03";
                    if (currentClipName != attackAnimation03 && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f) {
                        _playerInput.mouseButtonDown = false;
                        SwitchStateTo(CharacterState.Attaking);
                        //CalculatePlayerMovement(); se elimina para evitar conflicto con el LookRotation
                    }
                }
                // slide when is attacking
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
            break;
        case CharacterState.slide:
            // Slide velocity
            _moveVelocity = transform.forward * slideSpeed * Time.deltaTime;
            break;
        case CharacterState.spawn:
            currentSpawntime -= Time.deltaTime;
            if (currentSpawntime <= 0) {
                SwitchStateTo(CharacterState.Normal);
            }
            break;
        }
        
        if (impactOnCharacter.magnitude > 0.2f){
            _moveVelocity = impactOnCharacter * Time.deltaTime;
            impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);
        }
        
        // gravity
        if (isPlayer) {
            if (_cc.isGrounded == false){
                verticalVelocity = gravity;
            }
            else {
                verticalVelocity = gravity * 0.3f;
            }
            _moveVelocity += verticalVelocity * Vector3.up * Time.deltaTime;
            _cc.Move(_moveVelocity);
            _moveVelocity = Vector3.zero;
        }
        else {
            if (currentState != CharacterState.Normal) {
                _cc.Move(_moveVelocity);
                _moveVelocity = Vector3.zero;
            }
        }
    }
    
    public void SwitchStateTo( CharacterState newState ) {
        // clear input cache
        if (isPlayer) {
            _playerInput.ClearCache();
        }
        // exiting State
        switch(currentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attaking:
                if (_damageCaster != null){
                    DisableDamageCaster();
                }
                if (isPlayer) {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.dead:
                return;
            case CharacterState.beingHit:
                break;
            case CharacterState.slide:
                break;
            case CharacterState.spawn:
                isInvincible = false;
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
                    // play SFX
                    SFXManager.instance.PlayAudio(0);
                    attackStartTime =Time.time;
                    RotateToCursor(); // rotar hacia donde está el cursor al momento de atacar
                }
                break;
            case CharacterState.dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                DropItem();
                if (!isPlayer) {
                    // play dead sfx
                    SFXManager.instance.PlayAudio(1);
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer = 0;  // así se soluciona el error de que se mostraba la sombra del mesh del enemigo
                }
                break;
            case CharacterState.beingHit:
                _animator.SetTrigger("beingHit");
                SFXManager.instance.PlayAudio(4);
                if(isPlayer){
                    isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.spawn:
                isInvincible = true;
                currentSpawntime = spawnDuration;
                SFXManager.instance.PlayAudio(6);
                StartCoroutine(MaterialAppear());
                break;
        }
        currentState = newState;
        Debug.Log("switched to: " + currentState);
    }
    // función llamada en la animación attack
    public void AttackAnimationEnds() {
        SwitchStateTo(CharacterState.Normal);
    }
    // función llamada en la animación roll
    public void SlideAnimationEnds() {
        SwitchStateTo(CharacterState.Normal);
    }
    // función llamada en la animación hit
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
        else {
            AddImpact(attackerPos, 2.5f);
        }
    }
    
    // desliza al jugador hacia atrás cuando es golpeado
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
    
    public void AddHealth(int health) {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().HealVFX();
    }
    
    public void AddCoin(int coin) {
        totalCoins += coin;
    }
    
    public void PickUpItem (PickUpType item){
        switch (item.type){
        case PickUpType.PickUp.heal:
            AddHealth(item.value);
            break;
        case PickUpType.PickUp.coin:
            AddCoin(item.value);
            break;
        }
    }
    
    public void RotateToTarget() {
        if (currentState != CharacterState.dead) {
            transform.LookAt(targetPlayer, Vector3.up);
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
    
    IEnumerator MaterialAppear() {
        // VFX variables
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHigh_start = -10f;
        float dissolveHigh_target  = 20f;
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
        
        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    
    // Gizmo de donde se muestra el cursor
    protected void OnDrawGizmos() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        
        if (Physics.Raycast(ray, out hitResult, 1000, 1<< LayerMask.NameToLayer("CursorTest"))) {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }
    
    public void RotateToCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        
        if (Physics.Raycast(ray, out hitResult, 1000, 1<< LayerMask.NameToLayer("CursorTest"))) {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }
}
