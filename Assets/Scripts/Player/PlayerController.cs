using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(InputController))]
public class PlayerController : MonoBehaviour{

    [Range(0,1)]
    public float accelerationTimeAirborne = 0.0f;
    [Range(0,1)]
    public float accelerationTimeGrounded = 0.0f;

    [HideInInspector]
    public bool canMove; //For the message box or cutscenes to grab control

    [Range(0,10)]
    public float moveSpeed = 4;
    float directionFacing;


    public float pickupRange = 30;
    public LayerMask pickupMask;
    public LayerMask obstacleMask;
    GameObject grabbedObject;


    LevelManager levMan;
    AudioSource sfx;
    
    
    int gravCharge = 5;
    bool gravCanChange = false;


    [Range(0,5)]
    public float gravity = 3;
    [Range(300,1000)]
    public float maxFallSpeed = 300;

    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    [HideInInspector]
    public Controller2D controller;
    InputController input;


    //Blaster Stuff
    [Range(0,0.2f)]
    public float fireRate = 0.1f;
    float fireRateHolder = 0;
    [Range(1,12)]
    public float bulletLimit = 3;
    private float bulletCount = 0;

    public float damage = 10;
    public float BulletSpeed = 6; 
       
    public float bulletLimitTimer = 3.0f;
    private float bulletLimitTimerHolder = 0;
    Transform FirePoint;



    void Start () {
        
        controller = GetComponent<Controller2D>();
        input = GetComponent<InputController>();
        levMan = FindObjectOfType<LevelManager>();
        sfx = GetComponent<AudioSource>();

        //Blaster Stuff
        FirePoint = transform.FindChild("FirePoint");
        if (FirePoint == null) { Debug.LogError("No FirePoint?"); }
    }
	
	void Update () {

        if (velocity.x != 0) { directionFacing = Mathf.Sign(velocity.x); }


        float gv = gravity * 500;
        float sp = moveSpeed * 50;
        if (controller.collisions.above || controller.collisions.below) { velocity.y = 0; }       
        float targetVelocityX = input.HAxisMove * sp;
        if (!canMove) { targetVelocityX = 0; }

        if (controller.gravityDir == 0) { //down            
            velocity.x = targetVelocityX;
            //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
            velocity.y = -maxFallSpeed;
            //velocity.y -= gv * Time.deltaTime;
            //if (velocity.y < -maxFallSpeed) { Debug.Log("MAX FALL SPEED"); velocity.y = -maxFallSpeed; }

            if (controller.collisions.below) { gravCanChange = true; }
            else { gravCanChange = false; }
        }
       
        if (controller.gravityDir == 1) { //up
            velocity.x = targetVelocityX;
            //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.above ? accelerationTimeGrounded : accelerationTimeAirborne));
            velocity.y = maxFallSpeed;
            //velocity.y += gv * Time.deltaTime;
            //if (velocity.y > maxFallSpeed) { Debug.Log("MAX FALL SPEED"); velocity.y = maxFallSpeed; }

            if (controller.collisions.above) { gravCanChange = true; }
            else { gravCanChange = false; }

        }

        //Moving
        controller.Move(velocity * Time.deltaTime);


        if (!canMove) { return; }


        //Grabbing
        if (input.GrabButtonPress && grabbedObject == null) {
            Debug.Log(GetObjectInRange());
            TryGrabObject(GetObjectInRange());
        }
        else if (input.GrabButtonPress && grabbedObject != null) { DropObject(); }
        if (grabbedObject != null) {
            Vector3 newPos = gameObject.transform.position;
            newPos.x += 15 * directionFacing;
            
            grabbedObject.transform.position = newPos;
        }   
        
        //Shooting
        if (directionFacing == 1) { FirePoint.position = new Vector3(transform.position.x + 24,transform.position.y,0); }
        else { FirePoint.position = new Vector3(transform.position.x - 24, transform.position.y, 0); }

            //counts time up
        if (bulletLimitTimerHolder <= bulletLimitTimer) { bulletLimitTimerHolder += Time.deltaTime; }
            //if timer over bullet count goes down and can shoot again
        if (bulletLimitTimerHolder > bulletLimitTimer) { bulletCount = 0; }
            //if the timer is over when we shoot THEN we will reset timer
            //repeat for between shots
        if (fireRateHolder > 0) { fireRateHolder -= Time.deltaTime; }

        if (input.ShootButtonPress) { ShootBlaster(); }            

        //Gravving
        GravChange();
	}



    void TryGrabObject(GameObject grabObject) {
        if (grabObject == null) { return; }
        grabbedObject = grabObject;
        grabbedObject.GetComponent<ObjectController>().grabbed = true;
    }
    void DropObject() {
        if (grabbedObject == null) { return; }
        grabbedObject.GetComponent<ObjectController>().grabbed = false;
        grabbedObject = null;
    }
    GameObject GetObjectInRange() {

        Controller2D c = controller;
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = pickupRange ;

        for (int i = 0; i < c.horizontalRayCount; i++)
        {
            //Ray Origin
            Vector2 rayOrigin = new Vector2(0, 0);
            if (c.gravityDir == 0)
            {
                rayOrigin = (directionX == -1) ? c.raycastOrigins.bottomRight : c.raycastOrigins.bottomLeft;
                rayOrigin += Vector2.up * (c.horizontalRaySpacing * i);
            }
            if (c.gravityDir == 1)
            {
                rayOrigin = (directionX == -1) ? c.raycastOrigins.topRight : c.raycastOrigins.topLeft;
                rayOrigin += Vector2.down * (c.horizontalRaySpacing * i);
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, pickupMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);

            if (hit) {  return hit.collider.gameObject; }           
        }
        return null;
    }


    public void GravChange(bool ovrd = false) {
        if ( (input.GravButtonPress == true && gravCanChange == true) || (ovrd == true) ) {
            sfx.Play();
            if (controller.gravityDir == 0) { controller.gravityDir = 1; }
            else { controller.gravityDir = 0; }
            gravCanChange = false;
        }       
    }


    void ShootBlaster() {

        if ( (bulletLimit > bulletCount) && (fireRateHolder <= 0) ) { 

            //Create Bullet
            GameObject TempBullet;
            Vector2 FirePointPosition = new Vector2(FirePoint.position.x + (velocity.x / 16), FirePoint.position.y);

            TempBullet = Instantiate(Resources.Load("Bullet"), new Vector3(FirePointPosition.x, FirePointPosition.y, 0), transform.rotation) as GameObject;
            TempBullet.GetComponent<Bullet>().damage = damage;

            bulletCount += 1;
            if (bulletLimitTimerHolder > bulletLimitTimer) { bulletLimitTimerHolder = 0; }
            fireRateHolder = fireRate;

            //Get RigidBody2d
            Rigidbody2D TempRigid;
            TempRigid = TempBullet.GetComponent<Rigidbody2D>();

            TempRigid.AddForce(new Vector2(directionFacing,0) * BulletSpeed * 5000);

        }
    }
    
    
        
}
