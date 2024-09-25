using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinButton : MonoBehaviour
{
    public InputField hostIpInputField;
    public void OnButtonClick()
    {
        SceneManager.LoadScene("Game");
        var playerPrefab = Resources.Load<PlayerController>("Player");
        var player = Instantiate(playerPrefab);
        player.StartClient(hostIpInputField.text);
    }
}