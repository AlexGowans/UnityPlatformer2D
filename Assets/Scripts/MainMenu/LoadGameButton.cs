using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour {
    Button b;
	// Use this for initialization
	void Start () {
        b = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update() {

        if ( File.Exists(Application.persistentDataPath + "/mySaviour.corey") ) {
            b.interactable = true;
	    }
        else { b.interactable = false; }
    }
}
