using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinButton : MonoBehaviour
{
    public TMP_InputField hostIpInputField;
    
    public void OnButtonClick()
    {
        StartCoroutine(Co_JoinGame());
    }

    private IEnumerator Co_JoinGame()
    {
        var hostName = hostIpInputField.text;
        GameSession.JoinGame(hostName);
        yield return SceneManager.LoadSceneAsync("GameScene");
        var playerPrefab = Resources.Load<OpponentController>("Opponent");
        var player = Instantiate(playerPrefab);
    }
}