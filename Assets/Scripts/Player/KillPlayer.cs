using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

    LevelManager levMan;

	// Use this for initialization
	void Start () {
        levMan = FindObjectOfType<LevelManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") { levMan.RespawnPlayer(); }
    }
}
