using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TextSpeech;

public class VoiceRecognitionAndTextToSpeech : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public Button recordButton;
    public Button speakButton;

    private bool isRecording = false;

    void Start()
    {
        // Initialize TextToSpeech instance
        TextToSpeech.Instance.Setting("en-US", 1f, 1f);

        // Assign onClick listeners to buttons
        recordButton.onClick.AddListener(StartRecording);
        speakButton.onClick.AddListener(SpeakText);

        // Set up the callback to update input field with recognized speech
        SpeechToText.Instance.onResultCallback += OnSpeechRecognized;
    }

    void OnDestroy()
    {
        // Unsubscribe from the callback to avoid memory leaks
        SpeechToText.Instance.onResultCallback -= OnSpeechRecognized;
    }

    void Update()
    {
        // Continuously check for recording while the record button is held down
        if (isRecording)
        {
            // You can add a visual cue or animation here if needed
        }
    }

    void StartRecording()
    {
        // Start recording when the record button is pressed
        isRecording = true;
        SpeechToText.Instance.StartRecording("Speak now...");
    }

    void SpeakText()
    {
        // Speak the text from inputField when the speak button is pressed
        TextToSpeech.Instance.StartSpeak(inputField.text);
    }

    void OnSpeechRecognized(string text)
    {
        // Update input field with the recognized speech
        inputField.text = text;
    }
    public void SpeakMessage(string message)
{
    // Speak the message using text-to-speech
    TextToSpeech.Instance.StartSpeak(message);
}

    void OnDisable()
    {
        // Stop recording and speaking when the script is disabled or destroyed
        isRecording = false;
        SpeechToText.Instance.StopRecording();
        TextToSpeech.Instance.StopSpeak();
    }
}
