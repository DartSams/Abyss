using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    //updateAnim anim;
    public Animator anim;
    public GameObject player;
    public healthBar healthBar; //takes in the healthbar script
    SpriteRenderer sprite; //gets the the sprite renderer component
    private Rigidbody2D rb;
    public LayerMask playerLayer; //variable to select a layer
    public GameObject popupDamageObj; //prefab game object to show how much damage is recieved
    public TMP_Text popupDamageText;


    public float speed = 90f;
    private float distance;
    public float maxHealth = 100f;
    public float currHealth;
    private bool alive;
    private float detectionRange = 5f;
    private float attackPower = 5;


    private enum movementState { idle, walking, attack, death};
    movementState animState;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currHealth = maxHealth; //sets the current health to the max health
        //healthBar.updateHealthbar(maxHealth, currHealth);
        //healthBar.setPosition(transform.position); //sets the healthbar above the current object
        alive = checkHealth(); //checks if health is above 0 and sets the bool variable to true/false
    }

    // Update is called once per frame
    void Update()
    {
        
        if (alive)
        {
            distance = Vector2.Distance(transform.position, player.transform.position); //finds distance between 2 objects

            if (distance < detectionRange)
            {
                Vector2 direction = player.transform.forward - transform.position;

                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
                animState = movementState.walking;
            } //if the current object distance is less than (so in range) of the detectionRange then move towards another object
        } 
        
        //anim.UpdateAnimationState();
        //Debug.Log(transform.position.x + "," + player.transform.position.x);
        
        if (player.transform.position.x > transform.position.x)
        {
            sprite.flipX = true; // face left

        }
        else
        {
            sprite.flipX = false; //face right

        }
        //healthBar.setPosition(transform.position);
        if (!alive)
        {
            anim.SetTrigger("death");
            Destroy(gameObject, 2); //destroys the enemy after 2 seconds 

        } //if health is 0 or lower then triggers death animation and destroys object after 2 seconds
        //UpdateAnimationState();
    }


    public void takeDamage(float damageAmount)
    {
        alive = checkHealth();
        if (alive)
        {
            currHealth -= damageAmount;
            //Debug.Log(transform.position);
            Instantiate(popupDamageObj, transform.position, Quaternion.identity);
            popupDamageText.text = damageAmount.ToString();
            healthBar.updateHealthbar(maxHealth, currHealth);
        } 

    } //function to decrement health can be utilized in this script or another

    public void UpdateAnimationState()
    {
        if (alive && distance > detectionRange)
        {
            animState = movementState.idle;

        }

       
        if (!alive)
        {
            anim.SetTrigger("death");
            Destroy(gameObject,2); //destroys the enemy after 2 seconds 

        }
        anim.SetInteger("state", (int)animState);
    }

    private bool checkHealth()
    {
        return true ? currHealth > 0 : false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("attack");
            player player = collision.gameObject.GetComponent<player>();
            player.takeDamage(attackPower);
            player.anim.SetTrigger("recieveHit");
        } //checks if the current object collides with another with a tag of Player then triggers the current object attack animation and calls the function for the player to take damage and triggers the recieve hit animaion
    }

    public void knockback(Transform hitPosition, float knockbackPower)
    {
        Vector2 direction = (transform.position - hitPosition.position).normalized;
        Vector2 knockback = direction * (knockbackPower / 4);
        rb.AddForce(knockback, ForceMode2D.Impulse);
    } //adds a knockbakc force when hitting
}
