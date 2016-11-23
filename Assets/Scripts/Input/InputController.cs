using UnityEngine;
using System.Collections;
using Rewired;



public class InputController : MonoBehaviour {

    public int playerId = 0;
    private Player player;



    public float VAxisMove;    public float HAxisMove;
    public float VAxisAim;    public float HAxisAim;
    public float TriggL;    public float TriggR;
    public bool TriggRDown; bool i = false;
    public Vector2 MoveVector; public Vector2 AimVector;
    public bool GravButtonPress; public bool GravButton;
    public bool GrabButtonPress; public bool GrabButtonRelease;
    public bool ShootButtonPress;
    public bool AcceptButtonPress;

    


    void Awake() {
        player = ReInput.players.GetPlayer(playerId);
    }
 

    
	// Update is called once per frame
	void Update () {
        //BUTTONS//
        ///////////
        GravButtonPress = player.GetButtonDown("Gravity Change");
        GravButton = player.GetButton("Gravity Change");

        GrabButtonPress = player.GetButtonDown("Pickup");
        GrabButtonRelease = player.GetButtonUp("Pickup");

        ShootButtonPress = player.GetButtonDown("Shoot");

        AcceptButtonPress = player.GetButtonDown("Accept/Advance");
        


        //AXIS//
        ////////
        VAxisMove = player.GetAxis("Move Vertical");
        HAxisMove = player.GetAxis("Move Horizontal");
        MoveVector = new Vector2(HAxisMove, VAxisMove);
        

       // VAxisAim = Input.GetAxis("Aim Vertical");
       // HAxisAim = Input.GetAxis("Aim Horizontal");
       // AimVector = new Vector2(HAxisAim, VAxisAim);
        
        
    }
}
