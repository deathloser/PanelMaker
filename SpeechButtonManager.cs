using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Threading;
using System;
using System.IO;


public class SpeechButtonManager : MonoBehaviour
{
    public int maxButtonCountMainPanel;


    public class panelButtonPlacement {
        public int y_position {get; set;}
        public int x_position {get; set;}
        public string panel_name {get; set;}
        public string panel_type {get; set;}
    }

    public class elder_panel {
        public string name {get; set;}
        public int horizontalPos {get; set;}
        public int verticalPos {get; set;}
        public int maxButtonLimit {get;set;}
    }

    public class elder_button {
        public int id {get; set;}
        public string script {get; set;}
        public string label_text {get; set;}
        public string panel {get; set;}
        public string audiofile {get; set;}
        public string animation {get; set;}
        public string voice {get; set;}

    }

    // settings for text to speech
    // private const string SubscriptionKey = "81816fccffb44ee1bbd0bace06044270";
    // private const string Region = "westus";

    public GameObject canvas;
    public GameObject mainPanel;
    public GameObject sidePanel;
    public GameObject speechButtonPrefab;
    public TextAsset buttonCsv;
    public AudioSource canvasAudioSource;

    public GameObject characterWithAnimator;
    
    
    public bool panelIsOpen = false;
    public GameObject[] sidePanels;

    public GameObject sidePanelPrefab;

    // elderly script csv file
    public TextAsset elderScript;

    Dictionary<string, int> field_data = new Dictionary<string, int>();
    
    public void speakAndAnimateOnClick(params string[] paramsArray) {
        // Debug.Log(audiofile);
        string[] splitAudioFile = paramsArray[0].Split('.');
        string audioName = splitAudioFile[0];
        //animation
        Debug.Log(paramsArray[1]);
        characterWithAnimator.GetComponent<Animator>().CrossFade(paramsArray[1], 0.04f);
        
        
        // AudioSource track1 = character.GetComponent<AudioSource>();
        canvasAudioSource.clip = Resources.Load<AudioClip>(audioName);
        canvasAudioSource.Play();
    }


    public void closeCurrentSidePanel() {
        // Debug.Log("close the side panel that is currently open");
        sidePanels = GameObject.FindGameObjectsWithTag("sidePanel");
        foreach (GameObject panel in sidePanels) {
            panel.GetComponent<CanvasGroup>().alpha = 0;
            panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        panelIsOpen = false;
    }

    public void openSidePanel(string panelName) {
        GameObject panelToOpen = GameObject.Find(panelName);
        panelToOpen.GetComponent<CanvasGroup>().alpha = 1;
        panelToOpen.GetComponent<CanvasGroup>().interactable = true;
        panelToOpen.GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public void togglePanelOnClick(string panelName) {

        if (panelIsOpen) {
            panelIsOpen = false;
            closeCurrentSidePanel();
        } else {
            openSidePanel(panelName);
            panelIsOpen = true;
        }
        
    }

    //called by UIManager when the Button Size is adjusted from the setting page.
    public void updatePanelToggleButtons(int settingSize) {
        Vector3 newScaleMain;
        Vector3 newScaleSide;
        Vector3 currentPositionLeftPanel;
        Vector3 currentPositionRightPanel;
        Transform mainPanelTransform;
        Transform sidePanelTransform;

        mainPanelTransform = mainPanel.GetComponent<Transform>();
        sidePanelTransform = sidePanel.GetComponent<Transform>();

        currentPositionLeftPanel = mainPanelTransform.position;
        currentPositionRightPanel = sidePanelTransform.position;

        Vector3 updatedPositionLeftPanel;
        Vector3 updatedPositionRightPanel;

        
        // destroyButtonsOnSettingsChange();
        switch (settingSize) {
            case 1:
                Debug.Log("it is 1");
                mainPanelTransform.localScale = new Vector3((float) 0.7f, (float)0.5f, (float)0.5f);

                sidePanelTransform.localScale = new Vector3((float) 0.8f, (float) 0.8f, (float) 0.8f);

                // adjust the left panel to move over and stay on the screen when the button size is increased
                mainPanelTransform.position = new Vector3(158f, currentPositionLeftPanel.y, currentPositionLeftPanel.z);
                Debug.Log("UPDATE position");

                sidePanelTransform.position = new Vector3(301f, currentPositionRightPanel.y, currentPositionRightPanel.z);
                
                // sidePanelTransform.position = sidePanelTransform.position;
                break;
            case 2:
                Debug.Log("it is 2");
                mainPanelTransform.localScale = new Vector3((float) 0.75, (float) 0.55, (float) 0.55);

                sidePanelTransform.localScale = new Vector3((float) 0.85f, (float) 0.85f, (float) 0.85f);

                mainPanelTransform.position = new Vector3(168f, currentPositionLeftPanel.y, currentPositionLeftPanel.z);

                sidePanelTransform.position = new Vector3(301f, currentPositionRightPanel.y, currentPositionRightPanel.z);
                // sidePanelTransform.position = sidePanelTransform.position;
                break;
            case 3:
                Debug.Log("it is 3");
                mainPanelTransform.localScale = new Vector3((float) 0.8, (float) 0.6, (float) 0.6);

                sidePanelTransform.localScale = new Vector3((float) 0.9f, (float) 0.9f, (float) 0.9f);

                mainPanelTransform.position = new Vector3(178f, currentPositionLeftPanel.y, currentPositionLeftPanel.z);

                sidePanelTransform.position = new Vector3(281f, currentPositionRightPanel.y, currentPositionRightPanel.z);
                // sidePanelTransform.position = sidePanelTransform.position;
                break;
            case 4:
                Debug.Log("it is 4");
                mainPanelTransform.localScale = new Vector3((float) 0.95, (float) 0.95, (float) 0.95);

                sidePanelTransform.localScale = new Vector3((float) 0.85f, (float) 0.85f, (float) 0.85f);

                mainPanelTransform.position = new Vector3(188f, currentPositionLeftPanel.y, currentPositionLeftPanel.z);

                sidePanelTransform.position = new Vector3(281f, currentPositionRightPanel.y, currentPositionRightPanel.z);
                // sidePanelTransform.position = sidePanelTransform.position;
                break;
            case 5:
                Debug.Log("it is 5");
                mainPanelTransform.localScale = new Vector3((float) 0.9, (float) 0.7, (float) 0.7);

                sidePanelTransform.localScale = new Vector3((float) 1f, (float) 0.85f, (float) 0.85f);

                mainPanelTransform.position = new Vector3(198f, currentPositionLeftPanel.y, currentPositionLeftPanel.z);

                sidePanelTransform.position = new Vector3(281f, currentPositionRightPanel.y, currentPositionRightPanel.z);
                // sidePanelTransform.position = sidePanelTransform.position;
                break;
        }
        

    }

    public void destroyButtonsOnSettingsChange() {
        //clear dictionary data
        f_data.Clear();

        foreach (Transform child in mainPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    public void createPanelToggleButtons(Dictionary<GameObject, string> panelObjects, int settingSize) {
        // Debug.Log("add buttons to main panel, that toggle its own panel off and on");

        if (settingSize == null) {
            Debug.Log("it is null");
        } else {
            Debug.Log("it is not null");
        }

        //set it with default
        float verticalPositionMainPanel = 148;
        float horizontalPositionMainPanel = -187;


        foreach(KeyValuePair<GameObject, string> panel in panelObjects) {
            // Debug.Log(panel.Key);
            // Debug.Log(panel.Value);
            GameObject buttonInstance = Instantiate(speechButtonPrefab, mainPanel.transform);
            UnityEngine.UI.Button buttonComponent = buttonInstance.GetComponent<Button>();
            if(horizontalPositionMainPanel < -100) {
                    horizontalPositionMainPanel = horizontalPositionMainPanel + 200;
                } else {
                    //spacers
                    horizontalPositionMainPanel = -187;
                    verticalPositionMainPanel = verticalPositionMainPanel - 80;
                }
        
            buttonComponent.transform.localPosition = new Vector3(horizontalPositionMainPanel, verticalPositionMainPanel, 0);

            //create toggles
            buttonComponent.name = panel.Value +"ToggleButton";
            buttonComponent.GetComponentInChildren<TextMeshProUGUI>().text = panel.Value;

            buttonComponent.onClick.AddListener(delegate{togglePanelOnClick(panel.Value);});
            // buttonComponent.transform.localScale = new Vector3(3, 1, 1);
            }
    }
        
    
    
    public Dictionary<GameObject, string> createElderPanels(List<string> panelnames) {
        List<elder_panel> elderPanels = new List<elder_panel>();
        List<GameObject> panelGameObject = new List<GameObject>();
        Dictionary<GameObject, string> panelDict = new Dictionary<GameObject, string>();
        foreach (string panel in panelnames) {
            // Debug.Log(panel);
            elder_panel elderPanel = new elder_panel();
            elderPanel.name = panel;
            elderPanels.Add(elderPanel);
        }

        foreach (var panelObject in elderPanels) {
            //instantiate panels from panel objects
            GameObject panelContainer = GameObject.Find("PanelContainer");

            //create the panels
            GameObject newPanel = Instantiate(sidePanelPrefab, panelContainer.transform);
            newPanel.transform.parent = panelContainer.transform;
            newPanel.name = panelObject.name;
            newPanel.tag = "sidePanel";
            newPanel.GetComponent<CanvasGroup>().alpha = 0;
            newPanel.GetComponent<CanvasGroup>().interactable = false;
            newPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            panelGameObject.Add(newPanel);
            panelDict.Add(newPanel, newPanel.name);
        }
        
        return panelDict;
    }
    
    public Dictionary<string, int> fieldData = new Dictionary<string, int>();
    public Dictionary<string, int> f_data = new Dictionary<string, int>();

    public List<elder_button> processElderCsv(TextAsset eldercsv) {
        string[] csvData = eldercsv.text.Split('\n');
        List<elder_button> elder_buttons = new List<elder_button>();
        List<string> panelsNames = new List<string>();
        Dictionary<GameObject, string> panelObjects = new Dictionary<GameObject, string>();

        for (var j=1; j <csvData.Length; j++) {
            string[] elementInThisRow = csvData[j].Split(',');
            if (!panelsNames.Contains(elementInThisRow[2])) {
                // if panel is not present, add it to list
                panelsNames.Add(elementInThisRow[2]);
            }
        }

        panelObjects = createElderPanels(panelsNames);
        createPanelToggleButtons(panelObjects, 1);

        f_data.Add("id", 0);
        f_data.Add("order", 1);
        f_data.Add("type", 2);
        f_data.Add("audiofile", 3);
        f_data.Add("script", 4);
        f_data.Add("buttonname", 5);
        f_data.Add("animations", 6);
        f_data.Add("voice", 7);

        foreach (string row in csvData) {
            

            if (fieldData.Count == 0) {
                string[] columnHeader = row.Split(',');
                for (int i = 0; i < columnHeader.Length; i++) {
                    fieldData[columnHeader[i].ToLower()] = i;
                    // Debug.Log(columnHeader[i]);
                }

            } else {
                
                string[] currentFields = row.Split(',');

                if (currentFields[f_data["id"]].Length > 0) {
                    elder_button currentButton = new elder_button();
                    currentButton.id = int.Parse(currentFields[f_data["id"]].Trim());
                    string buttonLabel = currentFields[f_data["buttonname"]].Trim();
                    currentButton.audiofile = currentFields[f_data["audiofile"]].Trim();
                    currentButton.label_text = buttonLabel;
                    currentButton.panel = currentFields[f_data["type"]].Trim();
                    currentButton.voice = currentFields[f_data["voice"]].Trim();
                    currentButton.animation = currentFields[f_data["animations"]].Trim();
                    elder_buttons.Add(currentButton);
                }
            }
        }
        return elder_buttons;
    }


    public void createElderButtons(List<elder_button> elderbuttons, int settingSize) {
        if (settingSize == null) {
            Debug.Log("it is null");
        } else {
            Debug.Log("its not null");
        }
        Dictionary<string, panelButtonPlacement> panelButtonLocation = new Dictionary<string, panelButtonPlacement>();

        foreach(var button in elderbuttons) {
            GameObject panelObj = GameObject.Find(button.panel);

            GameObject buttonInstance = Instantiate(speechButtonPrefab, panelObj.transform);
            UnityEngine.UI.Button buttonComponent = buttonInstance.GetComponent<Button>();

            if(panelButtonLocation.ContainsKey(button.panel)) {
                // Debug.Log(button.panel + "It exists, get location");

                //add buttons to panels
                if(panelButtonLocation[button.panel].x_position < -130) {
                    panelButtonLocation[button.panel].x_position = panelButtonLocation[button.panel].x_position + 500;
                } else {
                    panelButtonLocation[button.panel].x_position = -215;
                    panelButtonLocation[button.panel].y_position = panelButtonLocation[button.panel].y_position - 50;
                }
        
                buttonComponent.transform.localPosition = new Vector3(panelButtonLocation[button.panel].x_position, panelButtonLocation[button.panel].y_position, 0);
                buttonComponent.transform.localScale = new Vector3(3, 1, 1);

            } else {
                // Debug.Log(button.panel + "It does not exist, create location");
                panelButtonPlacement newPlacement = new panelButtonPlacement();
                newPlacement.panel_name = button.panel;
                newPlacement.y_position = 210;
                newPlacement.x_position = -215;
                buttonComponent.transform.localPosition = new Vector3(newPlacement.x_position, newPlacement.y_position, 0);
                buttonComponent.transform.localScale = new Vector3(3, 1, 1);

                panelButtonLocation.Add(button.panel, newPlacement);
            }
            

            buttonComponent.name = button.label_text;
            buttonComponent.GetComponentInChildren<TextMeshProUGUI>().text = button.label_text;
            AudioSource track = buttonInstance.GetComponent<AudioSource>();
            buttonComponent.onClick.AddListener(delegate{speakAndAnimateOnClick(button.audiofile, button.animation);});
            
        }
    }

    void reloadResizedButtonsOnSettingsChange (int sizeSetting) {

        // List<elder_button> elderButtons = processElderCsv(elderScript);
        // createElderButtons(elderButtons);
    }



    void Start()
    {
        mainPanel = GameObject.Find("MainPanel");
        mainPanel.transform.position = new Vector3(156f, 275f, 0f);
        sidePanel = GameObject.Find("PanelContainer");
        sidePanel.transform.position = new Vector3(301f, 225f, 0f);
        // instantiateTextToSpeech();
        canvas = GameObject.Find("Canvas");
        canvasAudioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        //replace with model
        characterWithAnimator = GameObject.Find("Ch09_nonPBR");

        List<elder_button> elderButtons = processElderCsv(elderScript);
        createElderButtons(elderButtons, 1);


        
    }
}


