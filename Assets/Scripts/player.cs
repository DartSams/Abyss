using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class player : MonoBehaviour
{
    //PlayerInput playerInput; //reads the player controller
    //InputAction moveAction; //accepts the input actions


    public Vector2 moveInput;
    Rigidbody2D rb; //rigidbody component of current object
    //public Transform cameraTransform; // Reference to the camera's transform
    public healthBar healthBar; //takes in the healthbar script
    RaycastHit hit; //value for raycast that returns what its colliding with
    public Animator anim; //gets the animation component
    SpriteRenderer sprite; //gets the the sprite renderer component
    PolygonCollider2D coll; //gets the box collider component
    public Transform attackPoint;
    public GameObject popupDamageObj;
    public TMP_Text popupDamageText;

    public LayerMask jumpableGround; //variable to select a layer
    public LayerMask enemyLayer;

    public float moveSpeed = 5f; //starting movement speed
    public float maxHealth = 100f; //starting max health
    public float currentHealth; //variable for current health 
    public float attackPower = 10f; //damage multiplier
    private float attackRadius = 0.5f;
    private float jumpPower = 20;//with a gravity scale of 10 in the rigidbody
    private float groundRadius = 1f;
    private int facing; //a variable to determine what direction the player is facing 

    private enum movementState { idle,running,falling,jumping,attack1,attack2,attack3};
    private enum attackState { attack1, attack2, attack3 };
    private static System.Random random = new System.Random();

    

    // Start is called before the first frame update
    void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        //moveAction = playerInput.FindAction("Move");
        rb = GetComponent<Rigidbody2D>(); //gets the rigidbody component of the current object
        //cameraTransform = Camera.main.transform; //gets the current camera
        currentHealth = maxHealth; //starts the scene by setting the current health to the max health
        healthBar.updateHealthbar(maxHealth, currentHealth); //calls the function in the healthbar script to update the healthbar UI Image to be the current health percentage
        anim = GetComponent <Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<PolygonCollider2D>(); 
    }

    // Update is called once per frame
    void Update()
    {

        MovePlayer();
        UpdateAnimationState();
        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {

            rb.velocity = new Vector2(5 * facing, jumpPower);
        }
    }

    private void OnMove(InputValue value)
    {
        //Debug.Log("here");
        moveInput = value.Get<Vector2>();
        //Debug.Log(moveInput.x);

        //changeAnimation(moveInput.x, moveInput.y);
    } //automatically called with unity new input system the name of this function is based on the input mapping name


    private void MovePlayer()
    {
        Vector2 moveDirection = moveInput.normalized;
        Vector2 moveVelocity = moveDirection * moveSpeed;
        
            //rb.velocity = moveVelocity;
        Vector3 newPosition = transform.position + new Vector3(moveInput.x * moveSpeed * Time.deltaTime, 0f, 0f);
        transform.position = newPosition;
        
        //Debug.Log(moveVelocity);
    }

    private void UpdateAnimationState()
    {
        movementState animState;

        if (moveInput.x > 0f)
        {
            animState = movementState.running;
            facing = 1;
            sprite.flipX = false;
            attackPoint.transform.position = new Vector3(transform.position.x + 0.98f,transform.position.y + -1.09f,0f); //sets the attacking point of the player when facing the right
        }
        else if (moveInput.x < 0f)
        {
            animState = movementState.running;
            facing = -1;
            sprite.flipX = true;
            attackPoint.transform.position = new Vector3(transform.position.x + -0.98f, transform.position.y + -1.09f, 0f); //sets the attacking point of the player when facing the left

        }
        else
        {
            animState = movementState.idle;
        }

        if (moveInput.y > 0.1f && IsGrounded())
        {
            animState = movementState.jumping;
        }
        else if (IsGrounded() == false)
        {
            animState = movementState.falling;
        }

        

        if (Input.GetMouseButtonUp(0))
        { 
            movementState randomAttackState = GetRandomAttackState();
            anim.SetTrigger(randomAttackState.ToString());
            hitEnemies();
        }
        anim.SetInteger("state",(int)animState);
    }

    private static movementState GetRandomAttackState()
    {
        Array values = Enum.GetValues(typeof(movementState));
        int startIndex = 4; // Index of the first attack state
        int endIndex = 6;   // Index of the last attack state
        int randomIndex = random.Next(startIndex, endIndex + 1); // +1 to include the upper bound
        return (movementState)values.GetValue(randomIndex);
    } //reurns a random value from a enum list

    private void hitEnemies()
    {
        Collider2D[] Enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        foreach(Collider2D enemy in Enemies)
        {
            enemyAI enemyScript = enemy.GetComponent<enemyAI>();
            enemyScript.takeDamage(attackPower);
            //Instantiate(popupDamageObj, transform.position, Quaternion.identity);
            //popupDamageText.text = attackPower.ToString();
            enemyScript.anim.SetTrigger("recieveHit");

            enemyScript.knockback(attackPoint,attackPower);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

    }
    public void takeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.updateHealthbar(maxHealth, currentHealth);
    } //function to decrement health can be utilized in this script on another

    private bool IsGrounded()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, jumpableGround);
        //return hit.collider != null;

        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, groundRadius, jumpableGround);

        //return Physics2D.OverlapCircle(feet.position, groundRadius, jumpableGround);

    }

    public void knockback(Transform hitPosition, float knockbackPower)
    {
        Vector2 direction = (transform.position - hitPosition.position).normalized;
        Vector2 knockback = direction * (knockbackPower / 4);
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }
}
