using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TextboxManager : MonoBehaviour {

    public GameObject textBox;

    public Text theText;

    public int currentLine;
    public int endAtLine;

    [HideInInspector]
    public PlayerController playerCon;
    [HideInInspector]
    public InputController inputCon;

    public TextAsset textFile;
    public string[] textLines;

    public bool isActive;
    [HideInInspector]
    public bool stopPlayerMovement;


    bool justOpened;
    private bool isTyping = false;
    bool cancelTyping = false;

    public float typeSpeed;

    // Use this for initialization
    void Start() {

        playerCon = FindObjectOfType<PlayerController>();
        inputCon = FindObjectOfType<InputController>();

        if (textFile != null) {
            textLines = (textFile.text.Split('\n'));
        }

        else { Debug.Log("No Text File");  }

        if (endAtLine == 0) { endAtLine = textLines.Length - 1; }

        if (isActive) { EnableTextBox(); }
        else { DisableTexBox(); }

    }


    // Update is called once per frame
    void Update () {

        if (!isActive) { return; }
       
        if (inputCon.AcceptButtonPress ) {
            if (!isTyping) { //skip line, not typing
                currentLine += 1;

                //Check if a command not a line
                string commandCheck = textLines[currentLine];
                if (commandCheck[0] == ('/'))
                {
                    CommandFound(commandCheck);
                    currentLine += 1;
                }
                //End
                if (currentLine > endAtLine)
                {
                    DisableTexBox();
                }
                else { //New line of text
                    StartCoroutine(TextScroll(textLines[currentLine]));
                }
            }
            else if (isTyping && !cancelTyping && !justOpened) { //cancel typing, doesnt skip
                cancelTyping = true;
            }     
        }

        justOpened = false;
    }


    private IEnumerator TextScroll (string lineOfText) {
        int letter = 0;
        theText.text = "";
        isTyping = true;
        cancelTyping = false;
        while (isTyping && !cancelTyping && (letter < lineOfText.Length - 1) ) {
            theText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(typeSpeed); //only use this to wait in a co-routine
        }
        theText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }




    public void EnableTextBox() {
        textBox.SetActive(true);
        if (stopPlayerMovement) { playerCon.canMove = false; }
        isActive = true;
        cancelTyping = false;
        justOpened = true;
        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void DisableTexBox() {
        textBox.SetActive(false);
        playerCon.canMove = true;
        isActive = false;
    }

    public void ReloadText(TextAsset newText) {
        if (newText != null) {  
            textLines = new string[1];
            textLines = (newText.text.Split('\n'));
        }
        else { Debug.Log("No Text File"); }

    }

    void CommandFound(string get) {
        if (get.Substring(0, 11) == "/killPlayer") {
            Debug.Log("/killPlayer");
            return; 
        }
        if (get.Substring(0, 9) == "/position")
        {
            Debug.Log("Textbox position");
            return;
        }
    }

}
