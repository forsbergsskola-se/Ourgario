using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    private const int portNumber = 44445;
    private bool _finishedLoading;
    private PlayerController _playerController;
    private UdpClient _udpClient;
    private bool _isServer;

    #region Client
    private IPEndPoint _serverEndpoint;
    #endregion

    #region Server
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
            var bytes = receiveResult.Buffer;
            var chars = Encoding.UTF8.GetString(bytes);
            var position = JsonUtility.FromJson<Vector3>(chars);
            EnsureOpponentAndUpdatePosition(fromEndpoint, position);
        }
    }

    private void EnsureOpponentAndUpdatePosition(IPEndPoint opponentEndpoint, Vector3 opponentPosition)
    {
        if (!_opponents.TryGetValue(opponentEndpoint, out var opponentController))
        {
            opponentController = SpawnOpponent();
            _opponents[opponentEndpoint] = opponentController;
        }

        opponentController.transform.position = opponentPosition;
    }

    private async Task SendPositionToServer()
    {
        if (_playerController == null) return;

        var position = _playerController.transform.position;
        var chars = JsonUtility.ToJson(position);
        var bytes = Encoding.UTF8.GetBytes(chars);
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

    public static void JoinGame(string hostName)
    {
        var session = CreateNew();
        session._isServer = false;
        session._udpClient = new UdpClient();
        session._serverEndpoint = GetIPEndPoint(hostName, portNumber);
        session.StartCoroutine(session.Co_LaunchGame());
        Debug.Log("Joined Game" + hostName);
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

    private void RemoveDisconnectedOpponent(IPEndPoint opponentEndpoint)
    {
        if (_opponents.TryGetValue(opponentEndpoint, out var opponentController))
        {
            Destroy(opponentController.gameObject);
            _opponents.Remove(opponentEndpoint);
        }
    }
}
