using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

/// <summary>
/// This is a MonoBehaviour.
/// It is usually added to the DontDestroyOnLoad Context.
/// To make sure it doesn't get destroyed when switching Scenes from Lobby to Game.
/// It holds session information, e.g. are we the Host or the Server, what's the IP Address, the UdpClient for communication
/// It's also responsible for loading the Game Scene and instantiating the Player.
/// </summary>
public class GameSession : MonoBehaviour
{
    private const int portNumber = 44445;
    private bool _finishedLoading;
    private PlayerController _playerController;
    private UdpClient _udpClient;
    private bool _isServer;
    
    #region ----- Client -----
    private IPEndPoint _serverEndPoint;
    #endregion

    private async void FixedUpdate()
    {
        if (_isServer)
        {
            await ReceivePositionsToServer();
        }
        else
        {
            await SendPositionsToServer();
        }
    }

    private async Task ReceivePositionsToServer()
    {
        
    }
    
    private async Task SendPositionsToServer()
    {
        
    }

    private static GameSession CreateNew()
    {
        var go = new GameObject("GameSession");
        DontDestroyOnLoad(go);
        return go.AddComponent<GameSession>();
    }

    private static PlayerController SpawnPlayer()
    {
        var prefab = Resources.Load<PlayerController>("Player");
        return Instantiate(prefab);
    }
    
    public static void HostGame()
    {
        var session = CreateNew();
        session.StartCoroutine(session.Co_HostGame());
    }

    private IEnumerator Co_HostGame()
    {
        yield return SceneManager.LoadSceneAsync("Game");
        var player = SpawnPlayer();
    }

    public static void JoinGame(string hostName)
    {
        var session = CreateNew();
        session.StartCoroutine(session.Co_JoinGame(hostName));
    }

    private IEnumerator Co_JoinGame(string hostName)
    {
        yield return SceneManager.LoadSceneAsync("Game");
        var player = SpawnPlayer();
    }
}