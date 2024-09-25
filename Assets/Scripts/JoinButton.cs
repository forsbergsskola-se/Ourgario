using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
