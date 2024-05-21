using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

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

    // Game started boolean, false initially, true when player clicks play game button
    private bool gameStarted = false;

    // Flashlight gameobject
    private Light flashlight;

    // Flashight on/off boolean to activate/deactivate flashlight, initially off
    private bool isFlashlightOn = false;

    // Boolean (initially false) to show if player started the mission to find the cabin 
    // aka, they have left the original cabin and started their search for the cabin
    private bool missionStarted = false;

    //Jumpscare
    //Time to stop determines when video will end
    public GameObject jumpscarePlayer;
    public int timeToStop;

    //Text fields
    //Intro Text
    public TextMeshProUGUI introtext;
    //Win Text
    public TextMeshProUGUI wintext;
    // Boolean for if the 3 minute timer runs out and the enemy is now hunting (chasing to kill)
    public bool enemyIsHunting = false;

    // Public variable for the timer for starting the hunt (in seconds)
    public float timer = 180f;

    // Public button variable for restarting the game
    public Button restartButton;

    // Boolean for locking the player's movement and rotation
    private bool playerIsLocked = true;

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

        // Get flashlight game object
        flashlight = GameObject.Find("Flashlight").GetComponent<Light>();

        Debug.Log("Flashlight: " + flashlight);

        // Keep video off until collision
        jumpscarePlayer.SetActive(false);

        // Keep win text off until win
        wintext.gameObject.SetActive(false);

        // Keep restart game button off until game is lost/won
        restartButton = GameObject.FindGameObjectWithTag("RestartGameButton").GetComponent<Button>();
        restartButton.gameObject.SetActive(false);

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

        // Handle game logic in here that only occurs after the player presses play game button
        if(gameStarted)
        {
            // Turn off the flashlight when player presses F
            if(Input.GetKeyDown(KeyCode.F))
            {
                flashlight.enabled = !isFlashlightOn;
                isFlashlightOn = !isFlashlightOn;
            }
        }
        if(playerIsLocked)
        {
            transform.position = new Vector3(-22.9500008f,34.8069992f,-88.2099991f);
            GameObject.Find("Camera Head").transform.rotation = Quaternion.Euler(0,0,0);
            GameObject.Find("Player Camera").transform.rotation = Quaternion.Euler(0,0,0);
            transform.rotation = Quaternion.Euler(0,351.703644f,0);
        }
    
    }

    // Detect collisions with the player
    private void OnCollisionEnter(Collision collision)
    {
        // Player collides with enemy
        if (collision.gameObject == GameObject.FindGameObjectWithTag("Enemy"))
        {
            Debug.Log("Hit by enemy: " + collision.gameObject);

            //jumpscare video to play
            jumpscarePlayer.SetActive(true);
          
          //destroys video player after timeToStop seconds
          //  Destroy(jumpscarePlayer, timeToStop);
            GameLost();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // If the player walks through the beginning trigger (GameStartTrigger) set that trigger inactive
        // Here the game starts officially and the cabin will teleport
        if(collision.gameObject == GameObject.FindGameObjectWithTag("Start") && !missionStarted)
        {
            // Set trigger as inactive, to do: add coroutine to set it active after a few seconds
            collision.gameObject.SetActive(false);
            StartGame(collision.gameObject);
        }

        // If the player walks into the cabin after finding it again, player wins the game
        if(collision.gameObject == GameObject.FindGameObjectWithTag("Start") && missionStarted)
        {
            GameWon();
        }

    }

    // Function for winning the game, aka the player finds the cabin and walks in
    void GameWon()
    {
        Debug.Log("Congratulations, you win!!!");
        
        wintext.gameObject.SetActive(true);

        // Set the chasing and hunting value to false to stop the enemy
        enemyIsHunting = false;
        enemyScript.SetIsChasing(false);
        enemy.SetActive(false);
        playerIsLocked = true;

        // Display restart button
        StartCoroutine(DisplayRestartButton());
    }

    // Function for losing the game, the player gets killed by the enemy
    void GameLost()
    {
        Debug.Log("You lost!!!");
        // Set the chasing and hunting value to false to stop the enemy
        enemyIsHunting = false;
        enemyScript.SetIsChasing(false);
        enemy.SetActive(false);
        playerIsLocked = true;

        // Display restart button
        StartCoroutine(DisplayRestartButton());
    }

    // Function for starting the game once player leaves the cabin for the first time
    // Beginning sequence teleport cabin randomly, starter cutscene, possible text objective etc.
    void StartGame(GameObject trigger)
    {
        // Teleport cabin to random cabin spawn point and start mission to find the cabin
        Debug.Log("Player has left the cabin for the first time");
        int rand = Random.Range(0, 3);
        cabin.transform.position = cabinSpawnPoints[rand].transform.position;
        missionStarted = true;

        // Re-activate the cabin trigger for game winning logic
        StartCoroutine(ReActivateTrigger(trigger));

        // Set the enemy to chase the player
        StartCoroutine(StartChase());

        // Set the timer for the hunt to start where enemy chases the player constantly
        StartCoroutine(StartTheHunt(timer));
    } 

    // On play game button clicked, start game for player, disable button and enable movement
    public void PlayGame()
    {
        introtext.gameObject.SetActive(false);

        button.gameObject.SetActive(false);
        isLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameStarted = true;
        playerIsLocked = false;
    }

    // Coroutine function to make the enemy chase the player 5 seconds after leaving the cabin
    private IEnumerator StartChase()
    {
        yield return new WaitForSeconds(20);
        enemyScript.SetIsChasing(true);
        Debug.Log("Enemy is now chasing the player");
    }

    // Coroutine for re-activating the cabin trigger after player leaves the cabin
    private IEnumerator ReActivateTrigger(GameObject trigger)
    {
        yield return new WaitForSeconds(5);
        trigger.gameObject.SetActive(true);
    }

    // Coroutine for setting the timer, after the seconds inputted, start the enemy's hunt for the player
    private IEnumerator StartTheHunt(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemyIsHunting = true;
    }

    // Coroutine for displaying the restart button after dying and getting jumpscared
    private IEnumerator DisplayRestartButton()
    {
        yield return new WaitForSeconds(10);
        restartButton.gameObject.SetActive(true);
        isLocked = false;
    }

    // Function for restarting the game on restart button click
    public void RestartGame()
    {
        // Restart the game, respawn player, cabin and enemy in the beginning
        Debug.Log("Restarting game...");

        // Disable restart button and win text
        restartButton.gameObject.SetActive(false);
        wintext.gameObject.SetActive(false);

        // Disable jumpscare 
        jumpscarePlayer.GetComponent<VideoPlayer>().Stop();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isLocked = true;

        // Unlock player
        playerIsLocked = false;

        enemy.SetActive(true);
        enemyScript.SetIsChasing(false);

        //Respawn cabin, player and enemy to original spawn points
        cabin.transform.position = GameObject.Find("CabinOriginalSpawnPoint").transform.position;
        gameObject.transform.position = GameObject.Find("PlayerOriginalSpawnPoint").transform.position;
        enemy.transform.position = GameObject.Find("EnemyOriginalSpawnPoint").transform.position;

        // Set mission started boolean false to stop the previous mission to find the forest
        missionStarted = false;
    }
}
