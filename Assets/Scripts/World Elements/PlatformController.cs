using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0,2)]
    public float easeAmmount; //Modified to go 0-2 rather than 1-3

    public bool switchAttatched = false; // 0 = no switch
    public bool switchAlternateMode = false;
    bool altDir = false; //travelto
    bool stopped = false;
    bool switchOn = false;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

	public override void Start () {
        base.Start();

        

        if (switchAlternateMode) { stopped = true; }

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int  i=0; i< localWaypoints.Length; i++) {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
	}
	
	void Update () {

        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
	}

    public void SwitchReceiver(bool sBool) {
        if (sBool) { switchOn = true; }
        else { switchOn = false; }
        
    }


    float Ease(float x) {
        float a = easeAmmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement() {

        if (switchAttatched && !switchOn && !switchAlternateMode) { return Vector3.zero; }
        if (stopped && switchAttatched && switchAlternateMode) {

            if (!altDir && switchOn) { stopped = false; }
            if (altDir && !switchOn) { stopped = false; }
            else { return Vector3.zero; }
        }


        if (Time.time < nextMoveTime) { return Vector3.zero; }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWayPointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWayPointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWayPointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1) {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            
            if (!cyclic) {
                
                if ( fromWaypointIndex >= globalWaypoints.Length - 1) {

                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                    

                    if (switchAttatched && switchAlternateMode) {

                        if (switchOn && !altDir) { stopped = true; }
                        if (!switchOn && altDir) { stopped = true; }
                        altDir = !altDir;
                    }                                       
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform) {
        foreach (PassengerMovement passenger in passengerMovement) {
            if (!passengerDictionary.ContainsKey(passenger.transform)) {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform) {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatfrom);
            }
        }

    }


    void CalculatePassengerMovement(Vector3 velocity) {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        if (velocity.y == 0) { directionY = 0; }

        // Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        //hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, true));
                    }
                }
            }
        }

        // Horizontally moving platform (Horizontal collisions) (NOT FOR IF ON TOP)
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;
                        

                        // hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if ((directionY == -1 || directionY == 1))
        {
            float rayLength = skinWidth * 2;
            
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * -directionY, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        // hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
        //HORIZONTAL ONLY WHILST ON TOP
        else if (velocity.y == 0 && velocity.x != 0) {
            float rayLength = skinWidth * 2;

            
            for (int i = 0; i < verticalRayCount; i++) {
                //Check Up
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                if (hit) {
                    
                    if (!movedPassengers.Contains(hit.transform)) {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;


                        //hit.transform.Translate(new Vector3(pushX, 0));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, 0), true, false));
                    }
                }
                //Check down NOT WORKING FOR SOME REASON, USE TINY Y VALUE
                Vector2 rayOrigin2 = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, Vector2.down, rayLength, passengerMask);
                if (hit2)
                {
                    print("FUCK");
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;



                        //hit2.transform.Translate(new Vector3(pushX, 0));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, 0), true, true));
                    }
                }
            }
        }
    }



    struct PassengerMovement {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatfrom;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatfrom = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos ( ) {
        if (localWaypoints != null) {
            
            Gizmos.color = Color.green;
            float size = 30f;

            for (int i = 0; i < localWaypoints.Length; i++) {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
