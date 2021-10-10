﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private CapsuleCollider2D coll;
    [Header("Movement Variables")]
    [SerializeField] private int lives = 3;
    [SerializeField] private Text livesText ;
    
    [SerializeField] private float _movementAcceleration = 70f;
    [SerializeField] private float _maxMoveSpeed = 12f;
    [SerializeField] private float _groundLinearDrag = 50f;
    [SerializeField] public LayerMask ground;
    private float _horizontalDirection;
    private float _verticalDirection;
    private bool _changingDirection => (rb.velocity.x > 0f && _horizontalDirection < 0f) || (rb.velocity.x < 0f && _horizontalDirection > 0f);
    private bool _facingRight = true;

    [Header("Jump Variables")]
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _airLinearDrag = 2.5f;
    [SerializeField] private float _fallMultiplier = 8f;
    [SerializeField] private float _lowJumpFallMultiplier = 5f;
    [SerializeField] private float _downMultiplier = 12f;
    [SerializeField] private int _extraJumps = 1;
    [SerializeField] private float _hangTime = .1f;
    [SerializeField] private float _jumpBufferLength = .1f;
    private int _extraJumpsValue;
    private float _hangTimeCounter;
    private float _jumpBufferCounter;
    private bool _canJump => _jumpBufferCounter > 0f && (_hangTimeCounter > 0f || _extraJumpsValue > 0 );
    private bool _isJumping = false;

    //sebezhetetlenség
    private bool invincible=false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown("space")) _jumpBufferCounter = _jumpBufferLength;
        else _jumpBufferCounter -= Time.deltaTime;
        if (_horizontalDirection < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (_horizontalDirection > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if((rb.velocity.x > 0.05f || rb.velocity.x < -0.05f || _changingDirection)&& coll.IsTouchingLayers(ground)){
                anim.SetBool("running", true);
            anim.SetBool("idle", false);
            anim.SetBool("jumping",false);
            anim.SetBool("falling",false);
        }
        else if (rb.velocity.y < -0.1f&& coll.IsTouchingLayers(ground)==false ){
            anim.SetBool("running", false);
            anim.SetBool("idle", false);
            anim.SetBool("jumping",false);
                anim.SetBool("falling",true);
        }
        else if (rb.velocity.y* rb.velocity.y > 0.1f && coll.IsTouchingLayers(ground)==false){
            anim.SetBool("running", false);
            anim.SetBool("idle", false);
                anim.SetBool("jumping",true);
            anim.SetBool("falling",false);
        }
        else{
            if(coll.IsTouchingLayers(ground)){
                anim.SetBool("running", false);
                    anim.SetBool("idle", true);
                anim.SetBool("jumping",false);
                anim.SetBool("falling",false);
            }
            
           
        }
        
    }
    private void FixedUpdate()
    {
        MoveCharacter();
        if (rb.velocity.y==0)
        {
            ApplyGroundLinearDrag();
            _extraJumpsValue = _extraJumps;
            _hangTimeCounter = _hangTime;
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
            _hangTimeCounter -= Time.fixedDeltaTime;
        }
        if (_canJump)
        {
            Jump(Vector2.up);
            
        }
      
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag =="Slime"){
            decreaseLife();
        }
    }

    private void MoveCharacter(){
        rb.AddForce(new Vector2(_horizontalDirection, 0f) * _movementAcceleration);

        if (Mathf.Abs(rb.velocity.x) > _maxMoveSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * _maxMoveSpeed, rb.velocity.y);
    }

    public void decreaseLife(){
        if (!invincible)
        {
                lives--;
                if(lives==0){
                    //respawnolom ebben a poziban ez csak hard coded most
                    transform.position = new Vector2(-34.86f,2);
                    lives=3;
                }
                livesText.text=lives.ToString();
                //átlátszó lesz
                GetComponent<SpriteRenderer> ().color= new Color (1,1,1,0.6f);
                invincible = true;
                Invoke("resetInvulnerability", 2);
            
        }
        
    }

    private void resetInvulnerability()
        {
            GetComponent<SpriteRenderer> ().color= new Color (1,1,1,1);
            invincible = false;
        }


    private void ApplyGroundLinearDrag(){
        if (Mathf.Abs(_horizontalDirection) < 0.4f || _changingDirection)
        {
            rb.drag = _groundLinearDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = _airLinearDrag;
    }
    private void FallMultiplier()
    {
        if (_verticalDirection < 0f)
        {
            rb.gravityScale = _downMultiplier;
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = _fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = _lowJumpFallMultiplier;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
    }
    private void Jump(Vector2 direction)
    {
      
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(direction * _jumpForce, ForceMode2D.Impulse);
        _hangTimeCounter = 0f;
        _jumpBufferCounter = 0f;
        _isJumping = true;
    }


}