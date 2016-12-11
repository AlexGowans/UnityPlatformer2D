using UnityEngine;
using System.Collections;

public class GravFlip : MonoBehaviour {

    
    public bool beamRight;
    public bool beamLeft;
    public bool beamUp;
    public bool beamDown;

    public float rayLengthForSizeUp = 100;

    int activeBeams = 0;
    int startingBeamEntered = 0;
    bool firstBeam;
    bool secondBeam;

    public LayerMask wallMask;
    public LayerMask playerMask;
    

    PlayerController playerCon;


    // Use this for initialization
     void Start()
    {
        playerCon = FindObjectOfType<PlayerController>();
    }


    void Update() {
        BeamRaycast(); 
    }


    void BeamRaycast() {     
        //activeBeams = 0;
        float rayLength;

        if (beamUp) {
            //Get size of beams (check for a wall)
            Vector2 rayOrigin = transform.position;                        
            Debug.DrawRay(rayOrigin, Vector2.up * rayLengthForSizeUp, Color.green,5.0f);
            RaycastHit2D sizeHit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLengthForSizeUp, wallMask);            
            if (sizeHit) {
                rayLength = Vector2.Distance(sizeHit.point, rayOrigin);
                //Debug.Log(rayLength);
                //Debug.Log(rayLengthForSizeUp);
                
            }
            else {
                Debug.Log("Laser not finding wall");
                rayLength = 0;
            }         
            for (int i = 0; i < 2; i++) {
                rayOrigin.x = (i == 0) ? (rayOrigin.x - 5) : (rayOrigin.x + 5);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);
                if (hit) {
                    BeamDetected(i);
                    Debug.Log("Active Beams: " + activeBeams);
                }
                else {
                    BeamNotDetected(i);
                    Debug.Log("Cant find player: " + i);
                }
            }
        }
    }
    void BeamDetected(int i) {
        if (i == 0 && !firstBeam) {
            firstBeam = true;
            if (activeBeams == 0) { startingBeamEntered = 1; }
            activeBeams++;
        }
        if (i == 1 && !secondBeam) {
            secondBeam = true;
            if (activeBeams == 0) { startingBeamEntered = 2; }
            activeBeams++;
        }
    }
    void BeamNotDetected(int i) {
        if (i == 0) {
            firstBeam = false;
            activeBeams--;
            if (activeBeams == 0 && startingBeamEntered == 2) { playerCon.GravChange(true); }
        }
        if (i == 1) {
            secondBeam = false;
            activeBeams--;
            if (activeBeams == 0 && startingBeamEntered == 1) { playerCon.GravChange(true); }
        }
    }


    


}
