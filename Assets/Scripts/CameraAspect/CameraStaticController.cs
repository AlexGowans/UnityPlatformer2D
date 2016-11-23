using UnityEngine;
using System.Collections;

public class CameraStaticController : MonoBehaviour {

    LevelManager levMan;
    Vector2 roomNumber;
    Vector2 cameraOffset;
    public Vector2 roomSize;
    Vector2 targetPos;


    public float shakeTimer;
    public float shakeAmount;

	// Use this for initialization
	void Start () {
        cameraOffset.x = roomSize.x / 2;
        cameraOffset.y = -(roomSize.y / 2);
        levMan = FindObjectOfType<LevelManager>();
        CheckRoom();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (levMan.roomNumber != roomNumber) { CheckRoom(); }
        transform.position = targetPos;

        if (shakeTimer >= 0) {
            Vector2 shakePos = Random.insideUnitCircle * shakeAmount;
            transform.position = new Vector3(transform.position.x + shakePos.x, transform.position.y + shakePos.y, transform.position.z);
            shakeTimer -= Time.deltaTime;
        }
	}


    void CheckRoom() {

        roomNumber = levMan.roomNumber;

        Debug.Log("ROOM: " + roomNumber);

        targetPos.x = roomNumber.x * roomSize.x;
        targetPos.y = roomNumber.y * -roomSize.y;
        targetPos += cameraOffset;
    }


    public void ShakeCamera (float shakePwr, float shakeDur) { }

    
}
