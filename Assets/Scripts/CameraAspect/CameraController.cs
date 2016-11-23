using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public Vector3 target;
    public Vector3 offset;
    Vector3 targetPos;
    bool inMotion = false;

    int screenHeight;
    int screenWidth;

    Transform playerPos;
    // Use this for initialization
    void Start()
    {
        target = transform.position;
        targetPos = target;
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }


    void Update() {

        targetPos = target;

        if (!inMotion) {
            float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
            float vertExtent = Camera.main.orthographicSize;
            if (playerPos.position.y >= transform.position.y +vertExtent ) {
                target.y = transform.position.y + vertExtent * 2;
                target.x = transform.position.x;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != Vector3.zero)
        {
            inMotion = true;

            Vector3 posNoZ = transform.position;
            posNoZ.z = target.z;

            Vector3 targetDirection = (target - posNoZ);

            interpVelocity = targetDirection.magnitude * 15f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
            
            if (transform.position == targetPos ) {
                inMotion = false;
                target = Vector3.zero;
            }

        }
    }
}

// Original post with image here  >  http://unity3diy.blogspot.com/2015/02/unity-2d-camera-follow-script.html