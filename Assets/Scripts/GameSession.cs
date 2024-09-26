using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    
    #region    ------ Server -------
    private Dictionary<IPEndPoint, OpponentController> _opponents = new();
    #endregion

    private async void FixedUpdate()
    {
        if (!_finishedLoading) return;
        
        if (_isServer)
            await ReceivePositions();
        else
            await SendPositionToServer();
    }

    private async Task ReceivePositions()
    {
        while (_udpClient.Available > 0)
        {
            var receiveResult = await _udpClient.ReceiveAsync();
            var fromEndpoint = receiveResult.RemoteEndPoint;
            var bytes = receiveResult.Buffer;  // A7 A3 A8 A9 B8 47 38 91 04 ( 010101010 10101 1001 10101010 1010101010 )
            var chars = Encoding.UTF8.GetString(bytes); // "{"x":13.2,"y":9.7}"
            var position = JsonUtility.FromJson<Vector3>(chars); // Vector2 object with two fields: x: 13.2f y: 9,7f
            EnsureOpponentAndUpdatePosition(fromEndpoint, position);
        }
    }

    private void EnsureOpponentAndUpdatePosition(IPEndPoint opponentEndpoint, Vector3 opponentPosition)
    {
        if (!_opponents.TryGetValue(opponentEndpoint, out var opponentController)) // Is there 192.168.1.72 -----> Prefab of Opponent?
        { // If not:
            opponentController = SpawnOpponent(); // Create Prefab of Opponent
            _opponents[opponentEndpoint] = opponentController; // Assign 192.168.1.72 -----> Prefab of Opponent
        }
        opponentController.transform.position = opponentPosition;
    }

    private async Task SendPositionToServer()
    {
        var position = _playerController.transform.position; // Vector2 object with two fields: x: 13.2f y: 9,7f
        var chars = JsonUtility.ToJson(position); // "{"x":13.2,"y":9.7}"
        var bytes = Encoding.UTF8.GetBytes(chars); // A7 A3 A8 A9 B8 47 38 91 04 ( 010101010 10101 1001 10101010 1010101010 )
        await _udpClient.SendAsync(bytes, bytes.Length, _serverEndpoint);
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

    private static OpponentController SpawnOpponent()
    {
        var prefab = Resources.Load<OpponentController>("Opponent");
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
        yield return SceneManager.LoadSceneAsync("GameScene");
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