using UnityEngine;
using System.Collections;

public class GravFlip : RaycastController {

    
    public bool beamRight;
    public bool beamLeft;
    public bool beamUp;
    public bool beamDown;

    public int rayLengthForSizeUp = 10000;

    int activeBeams = 0;
    int firstBeamEntered = 0;
    bool firstBeam;
    bool secondBeam;

    LayerMask wallMask;
    LayerMask playerMask;
    

    PlayerController playerCon;

    int directionEntered = 0;
    int directionExited = 0;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        playerCon = FindObjectOfType<PlayerController>();
    }

    void Update() {
        BeamRaycast(); 
    }

    void BeamRaycast() {
        Bounds bounds = collider.bounds;
        activeBeams = 0;
        float rayLength;

        if (beamUp) {
            //Get size of beams (check for a wall)
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin.x += bounds.size.x / 2;
            RaycastHit2D sizeHit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLengthForSizeUp, wallMask);
            if (sizeHit) { rayLength = sizeHit.distance; }
            else {
                Debug.Log("Laser not finding wall");
                rayLength = 0;
            }         

            for (int i = 0; i < 2; i++) {
                rayOrigin = (i == 0) ? raycastOrigins.topLeft : raycastOrigins.topRight;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);
                if (hit) { BeamDetected(i); }
                else { BeamNotDetected(i); }
            }
        }
    }
    void BeamDetected(int i) {
        if (i == 0 && !firstBeam) {
            firstBeam = true;
            if (activeBeams == 0) { firstBeamEntered = 1; }
            activeBeams++;

        }
        if (i == 1 && !secondBeam) {
            secondBeam = true;
            if (activeBeams == 0) { firstBeamEntered = 2; }
            activeBeams++;
        }
    }
    void BeamNotDetected(int i) {
        if (i == 0) {
            firstBeam = false;
            activeBeams--;
            if (activeBeams == 0 && firstBeamEntered == 2) { playerCon.GravChange(true); }
        }
        if (i == 1) {
            secondBeam = false;
            activeBeams--;
            if (activeBeams == 0 && firstBeamEntered == 1) { playerCon.GravChange(true); }
        }
    }


    


}
