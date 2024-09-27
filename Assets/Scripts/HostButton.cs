using System.Collections;
using System.Collections.Generic;
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
        hostIpLabel.text = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First().ToString();
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
    }
}
