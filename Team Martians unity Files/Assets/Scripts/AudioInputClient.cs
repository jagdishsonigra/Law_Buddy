using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AudioInputClient : MonoBehaviour
{
    public TMP_InputField messageInputField;
    public Button sendButton;
    public ScrollRect scrollRect;
    public RectTransform content;
    public TextMeshProUGUI messagePrefab;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private UnityMainThreadDispatcher dispatcher;
    private Queue<string> messageQueue = new Queue<string>(); // Queue to store messages

    void Start()
    {
        dispatcher = FindObjectOfType<UnityMainThreadDispatcher>();
        if (dispatcher == null)
        {
            Debug.LogError("UnityMainThreadDispatcher not found in the scene.");
            return;
        }

        ConnectToServer();
        sendButton.onClick.AddListener(SendMessage);
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient("192.168.253.67", 1194); // Change IP address and port to your server
            stream = client.GetStream();
            // Start receiving messages from the server in a separate thread
            BeginReceive();
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void SendMessage()
    {
        try
        {
            string messageToSend = messageInputField.text;
            byte[] data = Encoding.UTF8.GetBytes(messageToSend);
            stream.Write(data, 0, data.Length);

            AddMessage("You", messageToSend);

            messageInputField.text = ""; // Clear input field after sending message
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void BeginReceive()
    {
        try
        {
            AsyncCallback callback = new AsyncCallback(ReceiveCallback);
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, callback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead > 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                string[] messageChunks = receivedMessage.Split(''); // Split message into chunks

                foreach (string chunk in messageChunks)
                {
                    string message = chunk.Trim(); // Trim whitespace
                    messageQueue.Enqueue(message); // Enqueue the message
                }

                // Speak messages from the queue
                SpeakNextMessage();

                // Continue listening for new messages
                BeginReceive();
            }
            else
            {
                // Connection closed
                Debug.Log("Connection closed.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void SpeakNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            dispatcher.Enqueue(() => AddMessage("Server", nextMessage));
        }
    }

    void AddMessage(string sender, string message)
    {
        // Create a new TextMeshProUGUI object for the message
        TextMeshProUGUI messageText = Instantiate(messagePrefab, content);
        
        // Check if this is the first message from the server
        bool isFirstMessageFromServer = (sender == "Server" && content.childCount == 0);

        // If it's the first message from the server, format it accordingly
        if (isFirstMessageFromServer)
        {
            // Format the message with a different color and prefix
            messageText.text = "<color=blue><b>Server:</b></color> " + message;
        }
        else
        {
            // Format other messages without the prefix
            messageText.text = message;
        }

        // Ensure proper vertical layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // Scroll to bottom

        // Speak the message
        VoiceRecognitionAndTextToSpeech vc = GetComponent<VoiceRecognitionAndTextToSpeech>();
        vc.SpeakMessage(message);

        // Speak the next message in the queue
        SpeakNextMessage();
    }

    void OnDestroy()
    {
        stream.Close();
        client.Close();
    }
}
