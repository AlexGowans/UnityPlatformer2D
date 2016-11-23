using UnityEngine;
using System.Collections;

public class GravFlip : MonoBehaviour {

    PlayerController playerCon;

    // Use this for initialization
    void Start()
    {
        playerCon = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") { playerCon.GravChange(true); playerCon.velocity.y = 0; }
    }


}
