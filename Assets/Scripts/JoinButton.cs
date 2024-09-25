using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinButton : MonoBehaviour
{
    public TMP_InputField hostIpInputField;
    public void OnButtonClick()
    {
        SceneManager.LoadScene("Game");
        var playerPrefab = Resources.Load<PlayerController>("Player");
        var player = Instantiate(playerPrefab);
        player.StartClient(hostIpInputField.text);
    }
}