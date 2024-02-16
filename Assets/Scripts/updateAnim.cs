using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateAnim : MonoBehaviour
{

    Animator anim; //gets the animation component
    SpriteRenderer sprite; //gets the the sprite renderer compone

    private enum movementState { idle, running, falling, jumping, attack1, attack2, attack3 };
    private enum attackState { attack1, attack2, attack3 };
    private static System.Random random = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
    }

    public void UpdateAnimationState()
    {
        movementState animState;

        animState = movementState.idle;
        anim.SetInteger("state", (int)animState);
    }
}
