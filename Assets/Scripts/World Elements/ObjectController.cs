using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

    [Range(0,5)]
    public float gravity = 12;

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public bool grabbed = false;

    Controller2D controller;


    // Use this for initialization
    void Start () {
        controller = GetComponent<Controller2D>();
    }
	
	// Update is called once per frame
	void Update () {

        if (!grabbed) { 
            float gv = gravity * 1000;
            if (controller.collisions.above || controller.collisions.below) { velocity.y = 0; }

            if (controller.gravityDir == 0)
            { //down            
                velocity.x = 0; //Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
                velocity.y -= gv * Time.deltaTime;
            }

            if (controller.gravityDir == 1)
            { //up
                velocity.x = 0; //Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.above ? accelerationTimeGrounded : accelerationTimeAirborne));
                velocity.y += gv * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
