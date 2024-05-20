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

    // Player is chasing boolean, initially false 
    // then set to true 5 seconds after beginning sequence (StartGame() in PlayerScript)
    private bool isChasing = false;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        //to face player
    }

    // Set enemy chasing boolean to input (shouldChase)
    public void SetIsChasing(bool shouldChase)
    {
        isChasing = shouldChase;
    }
}
