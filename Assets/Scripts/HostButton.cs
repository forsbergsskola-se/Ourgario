using UnityEngine;
using UnityEngine.SceneManagement;

public class HostButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
