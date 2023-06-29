using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsPage;
    CanvasGroup mainMenuCG;
    CanvasGroup settingsPageCG;
    public GameObject playButton;
    public TextMeshProUGUI buttonText;

    public GameObject leftButtonPanel;
    public GameObject rightButtonPanel;

    public int toggleButtonSize = 1;
    public int toggleButtonSpacing = 1;

    public int buttonSize = 1;
    int buttonSizeMax = 5;
    int buttonSizeMin = 1;
    public TextMeshProUGUI buttonSizeDisplay;


    public int speechButtonSize = 1;
    public int speechButtonSpacing = 1;
    

    public bool playButtonIsReady = false;

    public GameObject speechButtonManager;

    void Start() {
        mainMenuCG = mainMenu.GetComponent<CanvasGroup>();
        settingsPageCG = settingsPage.GetComponent<CanvasGroup>();

    }

    //main menu and settings button on click functions
    public void mainWindowSettingsButtonOnClick() {
        // close main window
        Debug.Log("close main window and open settings");
        mainMenuCG.alpha = 0;
        mainMenuCG.interactable = false;
        mainMenuCG.blocksRaycasts = false;

        //open settings window

        settingsPageCG.alpha = 1;
        settingsPageCG.interactable = true;
        settingsPageCG.blocksRaycasts = true;
    }

    public void settingsIconButtonOnClick() {
        settingsPageCG.alpha = 1;
        settingsPageCG.interactable = true;
        settingsPageCG.blocksRaycasts = true;
    }

    public void settingsWindowsQuitToMainMenu() {
        Debug.Log("close settings page and return to main menu");

        settingsPageCG.alpha = 0;
        settingsPageCG.interactable = false;
        settingsPageCG.blocksRaycasts = false;

        mainMenuCG.alpha = 1;
        mainMenuCG.interactable = true;
        mainMenuCG.blocksRaycasts = true;

    }

    public void settingsWindowQuitToGame() {
        Debug.Log("close settings page and return to main menu");

        settingsPageCG.alpha = 0;
        settingsPageCG.interactable = false;
        settingsPageCG.blocksRaycasts = false;

        mainMenuCG.alpha = 0;
        mainMenuCG.interactable = false;
        mainMenuCG.blocksRaycasts = false;
    }

    //when connecting to the server, the button is enabled
    public void connectedToServerEnablePlayButton() {
        Debug.Log("Play button is now ready");
        buttonText.text = "Play";
        playButtonIsReady = true;
    }

    public void playButtonOnClick() {
        if (playButtonIsReady) {
            Debug.Log("close main window and open game");
            mainMenuCG.alpha = 0;
            mainMenuCG.interactable = false;
            mainMenuCG.blocksRaycasts = false;
        } else {
            Debug.Log("Can't start the game if not connected to server.");
        }

    }

    public void quitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }

    //the buttons for the settings page, to increase and decrease the size of toggle and panel buttons

    public void resizeButtons () {

    }

    public void leftPanelDecreaseButtonSize() {
        Debug.Log("Decrease left side panel buttons");
        Debug.Log(buttonSizeDisplay.text);
        string buttonSizeText = buttonSizeDisplay.text;
        int parsedButtonSize = int.Parse(buttonSizeText);
    
        if (parsedButtonSize - 1 > 0) {
            Debug.Log("allowed");
            int newButtonSize = parsedButtonSize - 1;
            string newSize = newButtonSize.ToString();
            buttonSizeDisplay.text = newSize;
            speechButtonManager = GameObject.Find("ButtonManager(Clone)");
            speechButtonManager.GetComponent<SpeechButtonManager>().updatePanelToggleButtons(newButtonSize);
        } else {
            Debug.Log("not allowed");
        }

    }

    public void leftPanelIncreaseButtonSize() {
        Debug.Log("Increase left side panel buttons");
        // Debug.Log(buttonSizeDisplay.text);
        string buttonSizeText = buttonSizeDisplay.text;
        int parsedButtonSize = int.Parse(buttonSizeText);
        

        if (parsedButtonSize + 1 < 6) {
            Debug.Log("allowed");
            int newButtonSize = parsedButtonSize + 1;
            string newSize = newButtonSize.ToString();
            buttonSizeDisplay.text = newSize;
            speechButtonManager = GameObject.Find("ButtonManager(Clone)");
            speechButtonManager.GetComponent<SpeechButtonManager>().updatePanelToggleButtons(newButtonSize);

        } else {
            Debug.Log("not allowed");
        }
    }

    public void rightPanelDecreaseButtonSize() {
        Debug.Log("Decrease right side panel buttons");
    }
    public void rightPanelIncreaseButtonSize() {
        Debug.Log("Increase right side panel buttons");
    }


}
