using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioInputClient : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the TMP Input Field in the Unity Inspector
    public Button sendButton; // Reference to the Button in the Unity Inspector
    public TMP_Text responseText; // Reference to the TMP Text component to display server responses

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    void Start()
    {
        ConnectToServer();
        sendButton.onClick.AddListener(SendMessage); // Attach SendMessage method to the Button's onClick event
    }

    void ConnectToServer()
    {
        try
        {
            // Connect to the Python server
            client = new TcpClient("192.168.253.67", 12345);
            stream = client.GetStream();

            // Receive the welcome message from the server
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string welcomeMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("Server says: " + welcomeMessage);
            UpdateResponseText("Server says: " + welcomeMessage); // Display welcome message
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
            // Clear the response text
            responseText.text = "";

            // Send a message to the server
            string messageToSend = inputField.text; // Read text from TMP Input Field
            byte[] data = Encoding.UTF8.GetBytes(messageToSend);
            stream.Write(data, 0, data.Length);

            // If the user types 'bye', close the connection
            if (messageToSend.ToLower() == "bye")
            {
                stream.Close();
                client.Close();
            }
            
            // Receive a message from the server
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("Server says: " + receivedMessage);
            UpdateResponseText("Server says: " + receivedMessage); // Display received message
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void UpdateResponseText(string message)
    {
        responseText.text = message;
    }
}
