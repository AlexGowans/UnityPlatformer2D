using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider2D))]
public class NewItemGiver : MonoBehaviour {

    //Main Upgrades
    public bool giveGravityPack = false;
    public bool giveBlaster = false;
    public bool giveGravityGun = false;
    public bool giveIceBoots = false;
    public bool giveSmashBoots = false;
    public bool giveChargeShot = false;

    //Optional Upgrades
    public bool giveBlasterShotsUp = false; //more shots at once?
    public bool giveBlasterDamagePlus = false;

    //Saving Stuff
    public int myId; //used for saving which item booths have been used.
    public bool itemGiverUsed = false;

    void Update() {
        itemGiverUsed = Game.control.savedItemGet[myId];
    }


    void OnTriggerEnter2D(Collider2D col) {
        if ( col.CompareTag("Player") && !itemGiverUsed ) {

            itemGiverUsed = true;
            Debug.Log("GOT ITEM");
            Game.control.GotItem(this.myId);
            
        }
    }
}
