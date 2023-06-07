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
    public class panelButtonPlacement {
        public int y_position {get; set;}
        public int x_position {get; set;}
        public string panel_name {get; set;}
        public string panel_type {get; set;}
    }
    private class speech_button {
        public int id { get; set;}
        public string script {get; set;}
        public string filename {get; set;}
        public string label_text {get; set;}
        public string sort {get; set;}
    }

    public class elder_panel {
        public string name {get; set;}
        public int horizontalPos {get; set;}
        public int verticalPos {get; set;}
    }

    public class elder_button {
        public int id {get; set;}
        public string script {get; set;}
        public string label_text {get; set;}
        public string panel {get; set;}
    }

    private const string SubscriptionKey = "81816fccffb44ee1bbd0bace06044270";
    private const string Region = "westus";

    public GameObject canvas;
    public GameObject mainPanel;
    public GameObject sidePanel;
    public GameObject speechButtonPrefab;
    public TextAsset buttonCsv;
    public AudioSource canvasAudioSource;
    private const int SampleRate = 24000;
    private SpeechSynthesizer synthesizer;
    private SpeechConfig speechConfig;
    private bool audioSourceNeedStop;
    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;
    public bool panelIsOpen = false;
    public GameObject[] sidePanels;

    public GameObject sidePanelPrefab;

    // elderly script csv file
    public TextAsset elderScript;

    Dictionary<string, int> field_data = new Dictionary<string, int>();
    
    public void speakOnClick(string speechText) {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = null;
        var startTime = DateTime.Now;
        string speakMessage = speechText;
        string textToSpeech = "<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xmlns:mstts='https://www.w3.org/2001/mstts' xml:lang='en-US'><voice name='en-US-JessaNeural'><mstts:express-as type='chat' role ='SeniorFemale'>"+speakMessage+"</mstts:express-as></voice></speak>";
        
        using (var result = synthesizer.SpeakSsmlAsync(textToSpeech).Result)
        {
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, 
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); 
                        audioSourceNeedStop = true;
                    }
                });

            canvasAudioSource.clip = audioClip;
            canvasAudioSource.Play();
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    // elder csv

    public Dictionary<string, int> fieldData = new Dictionary<string, int>();

    public void closeCurrentSidePanel() {
        // Debug.Log("close the side panel that is currently open");
        sidePanels = GameObject.FindGameObjectsWithTag("sidePanel");
        foreach (GameObject panel in sidePanels) {
            Debug.Log(panel.name);
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
        // Debug.Log("The panel name is " + panelName);
        if (panelIsOpen) {
            panelIsOpen = false;
            closeCurrentSidePanel();
        } else {
            openSidePanel(panelName);
            panelIsOpen = true;
        }
        
    }

    public void createPanelToggleButtons(Dictionary<GameObject, string> panelObjects) {
        // Debug.Log("add buttons to main panel, that toggle its own panel off and on");
        float verticalPositionMainPanel = 184;
        float horizontalPositionMainPanel = -418;
        foreach(KeyValuePair<GameObject, string> panel in panelObjects) {
            // Debug.Log(panel.Key);
            // Debug.Log(panel.Value);
            GameObject buttonInstance = Instantiate(speechButtonPrefab, mainPanel.transform);
            UnityEngine.UI.Button buttonComponent = buttonInstance.GetComponent<Button>();
            if(horizontalPositionMainPanel < 130) {
                    horizontalPositionMainPanel = horizontalPositionMainPanel + 300;
                } else {
                    horizontalPositionMainPanel = -418;
                    verticalPositionMainPanel = verticalPositionMainPanel - 80;
                }
        
            buttonComponent.transform.localPosition = new Vector3(horizontalPositionMainPanel, verticalPositionMainPanel, 0);
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
            GameObject newPanel = Instantiate(sidePanelPrefab, canvas.transform);
            newPanel.transform.parent = panelContainer.transform;
            newPanel.name = panelObject.name;
            newPanel.tag = "sidePanel";
            // newPanel.GetComponent<Image>().enabled = false;
            // newPanel.gameObject.SetActive(false);
            // Debug.Log(panelObject.name);
            newPanel.GetComponent<CanvasGroup>().alpha = 0;
            newPanel.GetComponent<CanvasGroup>().interactable = false;
            newPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            panelGameObject.Add(newPanel);
            panelDict.Add(newPanel, newPanel.name);
            // newPanel.gameObject.SetActive(false);
        }
        
        return panelDict;
    }
    public List<elder_button> processElderCsv(TextAsset eldercsv) {
        string[] csvData = eldercsv.text.Split('\n');
        List<elder_button> elder_buttons = new List<elder_button>();
        List<string> panelsNames = new List<string>();
        Dictionary<GameObject, string> panelObjects = new Dictionary<GameObject, string>();

        for (var j=1; j <csvData.Length; j++) {
            // Debug.Log(csvData[j]);
            string[] elementInThisRow = csvData[j].Split(',');
            if (!panelsNames.Contains(elementInThisRow[2])) {
                // if panel is not present, add it to list
                // Debug.Log(elementInThisRow[2]);
                panelsNames.Add(elementInThisRow[2]);
            }
        }

        panelObjects = createElderPanels(panelsNames);
        createPanelToggleButtons(panelObjects);

        foreach (string row in csvData) {
            // Debug.Log(row);
            

            if (fieldData.Count == 0) {
                string[] columnHeader = row.Split(',');
                for (int i = 0; i < columnHeader.Length; i++) {
                    fieldData[columnHeader[i].ToLower()] = i;
                }
            } else {
                string[] currentFields = row.Split(',');
                if (currentFields[fieldData["id"]].Length > 0) {
                    elder_button currentButton = new elder_button();
                    currentButton.id = int.Parse(currentFields[fieldData["id"]].Trim());
                    string buttonLabel = currentFields[fieldData["buttonname"]].Trim();
                    currentButton.label_text = buttonLabel;
                    currentButton.panel = currentFields[fieldData["type"]].Trim();
                    elder_buttons.Add(currentButton);

                }
            }
        }
        return elder_buttons;
    }

    public void createElderButtons(List<elder_button> elderbuttons) {
        Dictionary<string, panelButtonPlacement> panelButtonLocation = new Dictionary<string, panelButtonPlacement>();

        foreach(var button in elderbuttons) {
            GameObject panelObj = GameObject.Find(button.panel);

            GameObject buttonInstance = Instantiate(speechButtonPrefab, panelObj.transform);
            UnityEngine.UI.Button buttonComponent = buttonInstance.GetComponent<Button>();

            if(panelButtonLocation.ContainsKey(button.panel)) {
                // Debug.Log(button.panel + "It exists, get location");
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
            buttonComponent.onClick.AddListener(delegate{speakOnClick(button.label_text);});
            
        }
    }



    public void attachMainButtonsToMainElderPanel() {


    }

    public void attachSidePanelButtons() {


    }

    void Start()
    {
        // List<speech_button> buttons = process_csv();
        // createSpeechButtons(buttons);
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        List<elder_button> elderButtons = processElderCsv(elderScript);
        createElderButtons(elderButtons);

        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };
        
    }

    void Update()
    {
        lock (threadLocker)
        {
            if (audioSourceNeedStop)
            {
                canvasAudioSource.Stop();
                audioSourceNeedStop = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }

    private List<speech_button> process_csv () {
        string[] csvData = buttonCsv.text.Split('\n');
        List<speech_button> speech_buttons = new List<speech_button>();
        foreach (string row in csvData) {
            if (field_data.Count == 0) {
                string[] column_header = row.Split(',');
                for (int i = 0; i < column_header.Length; i++) {
                    field_data[column_header[i].ToLower()] = i;
                }
            } else {
                string[] currentFields = row.Split(',');
                if (currentFields[field_data["id"]].Length > 0) {
                    speech_button currentButton = new speech_button();
                    currentButton.id = int.Parse(currentFields[field_data["id"]].Trim());
                    string buttonLabel = currentFields[field_data["button text"]].Trim();
                    currentButton.label_text = buttonLabel;
                    currentButton.sort = currentFields[field_data["sort"]].Trim();
                    speech_buttons.Add(currentButton);

                }
            }
        }
        return speech_buttons;
    }

    private void createSpeechButtons (List<speech_button> buttons) {
        float verticalPositionMainPanel = 184;
        float horizontalPositionMainPanel = -418;
        float verticalPositionSidePanel = 200;
        float horizontalPositionSidePanel = -168;
        int i = 1;
        foreach (speech_button b in buttons) {
            //where to attach the button
            GameObject speechButtonInstance = (b.sort == "main" ? Instantiate(speechButtonPrefab, mainPanel.transform) : Instantiate(speechButtonPrefab, sidePanel.transform));
            UnityEngine.UI.Button speechButtonComponent = speechButtonInstance.GetComponent<Button>();
            if (b.sort == "main") {
                speechButtonComponent.transform.localPosition = new Vector3(horizontalPositionMainPanel, verticalPositionMainPanel, 0);
                horizontalPositionMainPanel = horizontalPositionMainPanel + 200;
            } 
            if (b.sort == "side") {
                speechButtonComponent.transform.localPosition = new Vector3(horizontalPositionSidePanel, verticalPositionSidePanel, 0);
                verticalPositionSidePanel = verticalPositionSidePanel + 50;
                speechButtonComponent.transform.localScale = new Vector3(4, 1, 1);
            }

            speechButtonComponent.name = b.label_text;
            speechButtonComponent.GetComponentInChildren<TextMeshProUGUI>().text = b.label_text;
            speechButtonComponent.onClick.AddListener(delegate{speakOnClick(b.label_text);});

        }
    }


}
