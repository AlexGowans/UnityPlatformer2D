using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(BoxCollider2D))]

public class ButtonController : MonoBehaviour {
    
    public GameObject myObject;
    public bool permanantSwitch = false;
    public bool reverseSwitch = false;
    public bool blasterSwitch = false;
    public bool upsideDown = false;
    public bool onLeftWall = false;

    int triggerCount = 0;

    bool pressed = false;

    Animator animator;

    BoxCollider2D myCollider;
    
    
    void Awake() {
        animator = GetComponent<Animator>();
        if (blasterSwitch) {
            animator.SetInteger("BlasterCheck", 2);
        }
        else {
            animator.SetInteger("BlasterCheck", 1);
        }
    }
	
    void Start() {

        myCollider = GetComponent<BoxCollider2D>();
              
        if (blasterSwitch) {
            myCollider.size = new Vector2(4, 12); //change collider
            myCollider.offset = new Vector2(-6, 0);
            permanantSwitch = true; //Makes sure the blaster switch is permanent;
        } 
        Vector3 scale = transform.localScale;
        if  (upsideDown) {         
            scale.y *= -1;
            transform.localScale = scale;
            if (!blasterSwitch) { 
                myCollider.offset = new Vector2(0, 12);
            }                    
        }
        if (onLeftWall) {
            scale.x *= -1;
            transform.localScale = scale;
            if (blasterSwitch) {
                myCollider.offset = new Vector2(6, 0);
            }
        }
    }

	// Update is called once per frame
	void Update () {
        animator = GetComponent<Animator>();
        
        CheckIfPressed();
        

    }

    void SendSignal(bool p) {
        if (myObject.CompareTag("Platform")) {myObject.GetComponent<PlatformController>().SwitchReceiver(p); }
        else { myObject.GetComponent<PlatformController>().SwitchReceiver(p); } //Default
    }

    void CheckIfPressed() {
        
        bool pressedCheck = true;

        if (reverseSwitch == true) { pressedCheck = !pressedCheck; }
        if (triggerCount > 0) { //Check for rigidbody collision
            if (pressed == false) {
                SendSignal(pressedCheck); pressed = true;
                animator.SetBool("OnOff", true);
            }          
        }
        else {
            if (!permanantSwitch) { 
                if (pressed == true) {
                    SendSignal(!pressedCheck); pressed = false;
                    animator.SetBool("OnOff", false);
                } 
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {

        if (!blasterSwitch) { 
            if (col.CompareTag("Player") || col.CompareTag("Objects") ) {
                triggerCount++;               
                return;
            }           
        }

        else { 
            if (col.CompareTag("Bullet")) { triggerCount++; }
            
        }    
    }
    void OnTriggerExit2D(Collider2D col) {

        if (!blasterSwitch) { 
            if (col.CompareTag("Player") || col.CompareTag("Objects") ) {
                triggerCount--;               
                return;
            }           
        }

        else { 
            if (col.CompareTag("Bullet")) { /*do i need to even do a thing here?*/ }
            
        }    
    }









}
