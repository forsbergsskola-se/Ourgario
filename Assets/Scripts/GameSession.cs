using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    #region    ------ Client -------
    private IPEndPoint _serverEndpoint;
    #endregion

    private async void FixedUpdate()
    {
        if (_isServer)
            await ReceivePositions();
        else
            await SendPositionToServer();
    }

    private async Task ReceivePositions()
    {
        
    }

    private async Task SendPositionToServer()
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
        session._isServer = true;
        session._udpClient = new UdpClient(portNumber);
        session.StartCoroutine(session.Co_LaunchGame());
    }

    private IEnumerator Co_LaunchGame()
    {
        yield return SceneManager.LoadSceneAsync("Game");
        _playerController = SpawnPlayer();
        _finishedLoading = true;
    }

    private static IPEndPoint GetIPEndPoint(string hostName, int port)
    {
        var address = Dns.GetHostAddresses(hostName).First();
        return new IPEndPoint(address, port);
    }
    
    public static void JoinGame(string hostName)
    {
        var session = CreateNew();
        session._isServer = false;
        session._udpClient = new UdpClient();
        session._serverEndpoint = GetIPEndPoint(hostName, portNumber);
        session.StartCoroutine(session.Co_LaunchGame());
    }
}