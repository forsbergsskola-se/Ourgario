using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class ChatScript : MonoBehaviour
{
    void Start()
    {
        try
        {
            using var client = new TcpClient("127.0.0.1", 44444);
            using var reader = new StreamReader(client.GetStream());
            GetComponent<TMP_Text>().text = reader.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            GetComponent<TMP_Text>().text = "Couldn't get time from server. Try again later...";
            GetComponent<TMP_Text>().color = Color.red;
        }
    }
}