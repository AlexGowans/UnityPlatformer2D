using UnityEngine;
using System.Collections;

public class GravFlip : MonoBehaviour {

    PlayerController playerCon;

    int directionEntered = 0;
    int directionExited = 0;

    // Use this for initialization
    void Start()
    {
        playerCon = FindObjectOfType<PlayerController>();
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            //From Right
            if (other.transform.position.x > transform.position.x) { directionEntered = 1; Debug.Log("Entered Right"); return; }
            //From Left
            if (other.transform.position.x < transform.position.x) { directionEntered = 0; Debug.Log("Entered Left"); }
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            //From Right
            if (other.transform.position.x > transform.position.x) { directionExited = 1; }
            //From Left
            if (other.transform.position.x < transform.position.x) { directionExited = 0; }

            if (directionEntered != directionExited) {
                playerCon.GravChange(true); playerCon.velocity.y = 0;
            }
        }
    }


}
