using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Rewired;

public class GamepadMenuSelector : MonoBehaviour {

    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;

    Player player;

    void Awake() {
        player = ReInput.players.GetPlayer(0);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (player.GetAxis("UIVertical") != 0 && !buttonSelected ) {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
	}

    private void OnDisable() {
        buttonSelected = false;
    }
}
