using System.Collections;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostButton : MonoBehaviour
{
    public TMP_Text hostIpLabel;

    void Start()
    {
        var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        var ipv4Address = addresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
    
        if (ipv4Address != null)
        {
            hostIpLabel.text = ipv4Address.ToString();
        }
        else
        {
            hostIpLabel.text = "No IPv4 Address Found";
        }
    }

    
    public void OnButtonClick()
    {
        StartCoroutine(Co_StartHosting());
    }

    private IEnumerator Co_StartHosting()
    {
        yield return SceneManager.LoadSceneAsync("GameScene");
        var playerPrefab = Resources.Load<PlayerController>("Player");
        var player = Instantiate(playerPrefab);
        GameSession.HostGame();
    }
}
