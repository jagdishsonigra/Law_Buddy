using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioInputClient : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public TextMeshProUGUI messagePrefab;
    public TMP_InputField inputField;
    public Button sendButton;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private UnityMainThreadDispatcher dispatcher;

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
            client = new TcpClient("192.168.253.67", 12345); // Change IP address and port to your server
            stream = client.GetStream();

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
            string messageToSend = inputField.text;
            byte[] data = Encoding.UTF8.GetBytes(messageToSend);
            stream.Write(data, 0, data.Length);

            AddMessage("You", messageToSend);

            inputField.text = ""; // Clear input field after sending message
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
                string[] messageChunks = receivedMessage.Split('\n');

                foreach (string chunk in messageChunks)
                {
                    string message = chunk;
                    dispatcher.Enqueue(() => AddMessage("Server", message));
                }

                BeginReceive();
            }
            else
            {
                Debug.Log("Connection closed.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    void AddMessage(string sender, string message)
    {
        TextMeshProUGUI messageText = Instantiate(messagePrefab, content);

        bool isFirstMessageFromServer = (sender == "Server" && content.childCount == 0);

        if (isFirstMessageFromServer)
        {
            messageText.text = "<color=blue><b>Server:</b></color> " + message;
        }
        else
        {
            messageText.text = message;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // Scroll to bottom

        LayoutElement layoutElement = messageText.GetComponent<LayoutElement>();
        layoutElement.flexibleWidth = 9999; // Stretch width
        float contentWidth = content.rect.width;
        float preferredHeight = LayoutUtility.GetPreferredHeight(messageText.rectTransform);
        messageText.rectTransform.sizeDelta = new Vector2(contentWidth, preferredHeight);
    }

    void OnDestroy()
    {
        stream.Close();
        client.Close();
    }
}