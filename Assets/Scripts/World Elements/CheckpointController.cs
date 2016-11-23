using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {


    [Range(0,1)]
    public int checkpointGravity;
    public Vector2 roomNumber = Vector2.zero;
    LevelManager levMan;

    Animator animator;
    AudioSource sfx;

    bool isOn = false;

    // Use this for initialization
    void Start () {
        levMan = FindObjectOfType<LevelManager>();
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();

        if  (checkpointGravity == 1) {
            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;
        }


    }

    void Update() {
        if (levMan.currentCheckpoint == this.gameObject && !isOn ) {
            animator.SetBool("OnOff", true);
            sfx.Play();
            isOn = true;
        }
        else if (levMan.currentCheckpoint != this.gameObject && isOn) {
            animator.SetBool("OnOff", false);
            isOn = false;
        }
    }

   

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            levMan.currentCheckpoint = this.gameObject;
            levMan.roomNumberCheckpointLast = roomNumber;
            levMan.currentCheckpointGravity = checkpointGravity;
         
            levMan.Savepoint();
        }
    }
}
