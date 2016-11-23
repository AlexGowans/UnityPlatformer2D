using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController {

    

    float maxClimbAngle = 80;
    float maxDecendAngle = 80;


    public int gravityDir = 0;
    
    public CollisionInfo collisions;
    PlayerController playerCon;

    // Use this for initialization
    public override void Start() {
        base.Start();
        if (gameObject.GetComponent<PlayerController>()) { playerCon = gameObject.GetComponent<PlayerController>(); }
        
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity; 

        if ((velocity.y < 0) && gravityDir == 0) {DecendSlope(ref velocity);}
        if ((velocity.y > 0) && gravityDir == 1) { DecendSlope(ref velocity); }

        if (velocity.x != 0) { HorizontalCollisions(ref velocity); }
        if (velocity.y != 0) { VerticalCollisions(ref velocity); }

        transform.Translate(velocity);        


        if (standingOnPlatform && gravityDir == 0) { collisions.below = true; }
        if (standingOnPlatform && gravityDir == 1) { collisions.above = true; }
    }




    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //Ray Origin
            Vector2 rayOrigin = new Vector2(0,0);
            if (gravityDir == 0) {                
                    rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);                
            }
            if (gravityDir == 1) {
                rayOrigin = (directionX == -1) ? raycastOrigins.topLeft : raycastOrigins.topRight;
                rayOrigin += Vector2.down * (horizontalRaySpacing * i);
            }


            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) {
                //Slopes
                float slopeAngle = 0;
                if (gravityDir == 0) { slopeAngle = Vector2.Angle(hit.normal, Vector2.up); }
                if (gravityDir == 1) { slopeAngle = Vector2.Angle(hit.normal, Vector2.down); }

                if (i == 0 && slopeAngle <= maxClimbAngle) { 
                    if (collisions.decendingSlope) { //If decending previously
                        collisions.decendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }
                
                

                if ( (!collisions.climbingSlope) || (slopeAngle > maxClimbAngle) ) {  
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope) {
                        if (gravityDir == 0) { velocity.y = (Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) ); }
                        if (gravityDir == 1) { velocity.y = -(Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)); }
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

    }


    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {           
            
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
              
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin,Vector2.up * directionY * rayLength, Color.red);

            if (hit) {


                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;              
            }
        }

        if (collisions.climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);

            Vector2 dir = Vector2.up;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            if (gravityDir == 1) {
                dir = Vector2.down;
                rayOrigin = ((directionX == -1) ? raycastOrigins.topLeft : raycastOrigins.topRight) + Vector2.up * velocity.y;
            }
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, dir);
                if (slopeAngle != collisions.slopeAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }

        }
    }



    void ClimbSlope(ref Vector3 velocity, float slopeAngle) {

            float moveDistance = Mathf.Abs(velocity.x);
            float climbVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            if ((velocity.y <= climbVelocity && gravityDir == 0) || (velocity.y >= -climbVelocity && gravityDir == 1)) {
                if (gravityDir == 0) { velocity.y = climbVelocity; collisions.below = true; }
                if (gravityDir == 1) { velocity.y = -climbVelocity; collisions.above = true; }
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
            }
    }

    void DecendSlope(ref Vector3 velocity) {

        Vector2 dir = Vector2.up;
        if (gravityDir == 1) { dir = Vector2.down;  }
        float directionX = Mathf.Sign(velocity.x);

        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        if (gravityDir == 1) { rayOrigin = (directionX == -1) ? raycastOrigins.topRight : raycastOrigins.topLeft; }
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -dir, Mathf.Infinity, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, dir);
                if (slopeAngle != 0 && slopeAngle <= maxDecendAngle) {
                    if (Mathf.Sign(hit.normal.x) == directionX) {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                            float moveDistance = Mathf.Abs(velocity.x);
                            float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                            if (gravityDir == 0) { velocity.y -= decendVelocityY; }
                            if (gravityDir == 1) { velocity.y += decendVelocityY; }

                            collisions.slopeAngle = slopeAngle;
                            collisions.decendingSlope = true;
                            if (gravityDir == 0) { collisions.below = true; }
                            if (gravityDir == 1) { collisions.above = true; }
                        }
                    }
                }
            }
        
    }


    
    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool decendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
