using System.Collections;
using TMPro;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
    public TMP_InputField hostIpInputField;
    
    public void OnButtonClick()
    {
        var hostName = hostIpInputField.text;

        if (string.IsNullOrWhiteSpace(hostName))
        {
            Debug.LogError("Host IP cannot be empty!");
            return;
        }
        StartCoroutine(Co_TryJoinGame(hostName));
    }

    private IEnumerator Co_TryJoinGame(string hostName)
    {
        bool connectedSuccessfully = GameSession.TryJoinGame(hostName);

        if (connectedSuccessfully)
        {
            Debug.Log("Connected successfully! Loading the game scene...");

            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene");

            var playerPrefab = Resources.Load<PlayerController>("Player");
            var player = Instantiate(playerPrefab);
        }
        else
        {
            Debug.LogError("Failed to connect to the server!!!");
        }
    }
}