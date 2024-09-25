using System;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostButton : MonoBehaviour
{
    public TMP_Text hostIPAddress;
    private void Start()
    {
        hostIPAddress.text = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First().ToString();
    }
    public void OnButtonClick()
    {
        SceneManager.LoadScene("GameScene");
        var playerPrefab = Resources.Load<PlayerController>("Player");
        var player = Instantiate(playerPrefab);
        player.StartHosting();
    }

}
