using UnityEngine;
using System.Collections;

public class ActivateText : MonoBehaviour {

    public TextAsset theText;

    public int startLine;
    public int endLine;

    public bool stopPlayerMovement;


    [HideInInspector]
    public TextboxManager theTextBox;
    InputController inputCon;

    public bool requireButtonPress;
    private bool waitForPress;

    public bool destroyWhenActivated; //for one time proximity things

	// Use this for initialization
	void Start () {
        theTextBox = FindObjectOfType<TextboxManager>();
        inputCon = FindObjectOfType<InputController>();	
	}
	
	// Update is called once per frame
	void Update () {
	    if (waitForPress && inputCon.AcceptButtonPress && !theTextBox.isActive) { TextCall(); }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == ("Player") ) {
            if (requireButtonPress) {
                waitForPress = true;
                return;
            }
            TextCall();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == ("Player") ) { waitForPress = false; }
    }

    void TextCall() {
        theTextBox.ReloadText(theText);
        theTextBox.currentLine = startLine;
        if (endLine != 0) { theTextBox.endAtLine = endLine; }
        else { endLine = theTextBox.endAtLine; }
        if (stopPlayerMovement) { theTextBox.stopPlayerMovement = true; }
        else { theTextBox.stopPlayerMovement = false; }

        theTextBox.EnableTextBox();
              
        if (destroyWhenActivated) { Destroy(gameObject); }
    }

}
