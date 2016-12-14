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

    LineRenderer lineRenderer;


    // Use this for initialization
     void Start()
    {
        playerCon = FindObjectOfType<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update() {
        BeamRaycast(); 
    }


    void BeamRaycast() {
        //activeBeams = 0;
        float rayLength = 0;

        if (beamUp) {
            CreateBeamUpDown(rayLength, 1);
        }
        if (beamDown) {
            CreateBeamUpDown(rayLength, -1);
        }

       
    }
    void CreateBeamUpDown(float rayLength, int dir) {
        //Get size of beams (check for a wall)
        Vector2 rayOrigin = transform.position;
        rayOrigin.y += 8 * dir;
        Debug.DrawRay(rayOrigin, dir * Vector2.up * rayLengthForSizeUp, Color.green);
        RaycastHit2D sizeHit = Physics2D.Raycast(rayOrigin, Vector2.up*dir, rayLengthForSizeUp, wallMask);
        if (sizeHit) {
            rayLength = Vector2.Distance(sizeHit.point, rayOrigin);
        }
        else {
            Debug.Log("Laser not finding wall");
            rayLength = 0;
        }
        for (int i = 0; i < 2; i++) {
            if (i == 0) { rayOrigin.x -= 5; }
            if (i == 1) { rayOrigin.x += 10; } //+10 as +5 only takes it back to Origin                
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dir, rayLength, playerMask);
            Debug.DrawRay(rayOrigin, dir * Vector2.up * rayLength, Color.red);
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
    //Beam Detection What do
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
        if (i == 0 && firstBeam) { 
            firstBeam = false;
            activeBeams--;
            if (activeBeams == 1 && startingBeamEntered == 1)
            {
                startingBeamEntered = 2;
                playerCon.GravChange(true);
            }                       
        }
        if (i == 1 && secondBeam) {
            secondBeam = false;
            activeBeams--;
            if (activeBeams == 1 && startingBeamEntered == 2)
            {
                startingBeamEntered = 1;
                playerCon.GravChange(true);
            }
        }            
    }

   
    


}
