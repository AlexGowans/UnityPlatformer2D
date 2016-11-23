using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public GameObject currentCheckpoint;

    float spawnPosX = 0;
    float spawnPosY = 0;
    float spawnPosZ = 0;
    [HideInInspector]
    public int currentCheckpointGravity;

    Transform playerPos;
    private PlayerController playerCon;

    [HideInInspector]
    public Vector2 roomNumber;
    [HideInInspector]
    public Vector2 roomNumberCheckpointLast;

    CameraStaticController staticCam;        

	// Use this for initialization
	void Start () {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        playerCon = GameObject.Find("Player").GetComponent<PlayerController>();
        staticCam = FindObjectOfType<CameraStaticController>();      

        if (Game.control.loadingGame) {
            spawnPosX = Game.control.savedPosX;
            spawnPosY = Game.control.savedPosY;
            spawnPosZ = Game.control.savedPosZ;
            currentCheckpointGravity = Game.control.savedGravDir;

            Debug.Log("Load Detected");
        }
        firstSpawnPlayer();       

        //Where is the player?
        roomNumber = new Vector2(0,0); // New game start location
        roomNumberCheckpointLast = roomNumber;



	}
	
	// Update is called once per frame
	void Update () {

        //Check player room position
        //x
        if (playerPos.position.x > (roomNumber.x * staticCam.roomSize.x) + staticCam.roomSize.x) { roomNumber.x += 1; }
        else if (playerPos.position.x < (roomNumber.x * staticCam.roomSize.x) ) { roomNumber.x -= 1; }
        //y
        if (playerPos.position.y < (roomNumber.y * -staticCam.roomSize.y) - staticCam.roomSize.y) { roomNumber.y += 1; }
        else if (playerPos.position.y > (roomNumber.y * -staticCam.roomSize.y) ) { roomNumber.y -= 1; }
    }

    public void RespawnPlayer() {
        Debug.Log("Player Respawn");
        playerCon.controller.gravityDir = currentCheckpointGravity;
        playerCon.velocity = Vector3.zero;
        playerCon.transform.position = currentCheckpoint.transform.position;

    }

    public void firstSpawnPlayer()
    {
        Debug.Log("Player Spawn");
        playerCon.controller.gravityDir = currentCheckpointGravity;
        playerCon.velocity = Vector3.zero;

        if (Game.control.loadingGame)
        {
            playerCon.transform.position = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        }
        else {
            playerCon.transform.position = currentCheckpoint.transform.position;
        }
    }

    public void Savepoint() {
        Game.control.savedCheckpoint = currentCheckpoint;
        Game.control.savedGravDir = currentCheckpointGravity;
        Game.control.Save();
    }   




}
