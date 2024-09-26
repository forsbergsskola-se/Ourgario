using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinButton : MonoBehaviour
{
    public TMP_InputField hostIpInputField;
    
    public void OnButtonClick()
    {
        StartCoroutine(Co_StartHosting());
    }

    private IEnumerator Co_StartHosting()
    {
        yield return SceneManager.LoadSceneAsync("Game");
        var playerPrefab = Resources.Load<PlayerController>("Player");
        var player = Instantiate(playerPrefab);
        var hostName = hostIpInputField.text;
    }
}