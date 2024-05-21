using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Enemy speed 
    public float speed = 5f;

    // Player game object
    public GameObject player;

    // Public chase distance variable, aka the distance between the player and enemy where enemy stops chasing
    public float chaseDistance = 10f;

    // Player is chasing boolean, initially false 
    // then set to true 5 seconds after beginning sequence (StartGame() in PlayerScript)
    private bool isChasing = false;

    //Animation controller
    private Animator mAnimator;

    // Start is called before the first frame update
    void Start()
    {
        //get animator
        mAnimator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Enemy chases the player when the distance between itself and the player is greater than the chase distance
        if (isChasing && Vector3.Distance(transform.position, player.transform.position) > chaseDistance && player.GetComponent<PlayerScript>().enemyIsHunting == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            Debug.Log("Enemy chasing...");
            mAnimator.SetTrigger("TrWalk");

        }
        // After the timer goes down, enemy goes into hunting state and constantly chases the player
        else if(player.GetComponent<PlayerScript>().enemyIsHunting == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            Debug.Log("Enemy hunting...");
            mAnimator.SetTrigger("TrHunt");

        }
        // Idle state, enemy is within the chase distance and isn't in hunting state so it stands still
        else if(isChasing && Vector3.Distance(transform.position, player.transform.position) <= chaseDistance && player.GetComponent<PlayerScript>().enemyIsHunting == false)
        {
            // Idle animation
            Debug.Log("Enemy idle...");
            mAnimator.SetTrigger("TrIdle");
        }

        //to face player
    }

    // Set enemy chasing boolean to input (shouldChase)
    public void SetIsChasing(bool shouldChase)
    {
        isChasing = shouldChase;
    }
}
