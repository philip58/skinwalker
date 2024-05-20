using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    // Cabin prefab
    private GameObject cabin;

    // Cabin spawn points array
    private GameObject[] cabinSpawnPoints;

    // Enemy gameobject
    private GameObject enemy;

    // Enemy Script
    private EnemyScript enemyScript;

    // Button variable
    public Button button;

    // Cursor is locked variable
    private bool isLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the cabin and cabin spawn points for game logic
        cabin = GameObject.FindGameObjectWithTag("Cabin");

        Debug.Log("Cabin : " + cabin);

        cabinSpawnPoints = GameObject.FindGameObjectsWithTag("CabinSpawnPoint");

        for(int i = 0; i < cabinSpawnPoints.Length; i++)
        {
            Debug.Log("Cabin Spawn Point" + cabinSpawnPoints[i]);
        }

        // Get enemy and enemy script for enemy logic
        enemy = GameObject.FindGameObjectWithTag("Enemy");

        enemyScript = enemy.GetComponent<EnemyScript>(); 

        // Get button component
        button = GameObject.Find("PlayGame").GetComponent<Button>();

        Debug.Log("Button" + button);
    }

    // Update is called once per frame
    void Update()
    {
        // Disable cursor lock for player to click button
        if(!isLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    
    }

    // Detect collisions with the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == GameObject.FindGameObjectWithTag("Enemy"))
        {
            Debug.Log("Hit by enemy: " + collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // If the player walks through the beginning trigger (GameStartTrigger) set that trigger inactive
        // Here the game starts officially and the cabin will teleport
        if(collision.gameObject == GameObject.FindGameObjectWithTag("Start"))
        {
            Debug.Log("Player has left the cabin for the first time");
            collision.gameObject.SetActive(false);
            StartGame();
        }
    }

    // Function for starting the game once player leaves the cabin for the first time
    // Beginning sequence teleport cabin randomly, starter cutscene, possible text objective etc.
    void StartGame()
    {
        // Teleport cabin to random cabin spawn point
        int rand = Random.Range(0, 3);
        cabin.transform.position = cabinSpawnPoints[rand].transform.position;

        // Set the enemy to chase the player
        // Should be after 5 seconds, to do: add coroutine for this
        enemyScript.SetIsChasing(true);
    } 

    // On play game button clicked, start game for player, disable button and enable movement
    public void PlayGame()
    {
        button.gameObject.SetActive(false);
        isLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
