using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

[Serializable]
public struct PositionPackage
{
    public Vector2 position;
    public string guid;
}

public class PlayerController : MonoBehaviour
{
    private bool _isServer;
    private Blob _blob;
    private UdpClient _udpClient;
    private Dictionary<string, OpponentController> _opponentPlayers;
    private Dictionary<string, IPEndPoint> _opponentEndpoints;
    
    private void Start()
    {
        _blob = GetComponent<Blob>();
    }
    
    void Update()
    {
        var cursorInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _blob.direction = cursorInWorld - transform.position;
    }

    private async void FixedUpdate()
    {
        if (!_isServer)
        {
            var blob = GetComponent<Blob>();
            var package = new PositionPackage { position = blob.transform.position, guid = blob.Guid };
            var packageString = JsonUtility.ToJson(package);
            var bytes = System.Text.Encoding.UTF8.GetBytes(packageString);
            await _udpClient.SendAsync(bytes, bytes.Length);
            return;
        }
        
        // go through all blob
        foreach (var blob in FindObjectsOfType<Blob>())
        {
            // create and serialize a position update package
            var package = new PositionPackage { position = blob.transform.position, guid = blob.Guid };
            var packageString = JsonUtility.ToJson(package);
            var bytes = System.Text.Encoding.UTF8.GetBytes(packageString);
            
            // and send it to all players...
            foreach (var player in _opponentEndpoints)
            {
                // ...but not that controls this blob!
                if (player.Key == blob.Guid) continue;
            
                // send the package we received to the other players:
                await _udpClient.SendAsync(bytes, bytes.Length);
            }
        }
    }

    public async void StartClient(string hostName)
    {
        _opponentPlayers = new();
        _opponentEndpoints = new();
        GetComponent<Blob>().Guid = System.Guid.NewGuid().ToString();
        _udpClient = new UdpClient(hostName, 44445);
        while (true)
        {
            var receivePackage = await _udpClient.ReceiveAsync();
            var byteEncoded = receivePackage.Buffer;
            var jsonEncoded = System.Text.Encoding.UTF8.GetString(byteEncoded);
            var positionPackage = JsonUtility.FromJson<PositionPackage>(jsonEncoded);
        
            if(_opponentPlayers.TryGetValue(positionPackage.guid, out var opponentController))
            {
                // we already instantiated this player. Just update his position
                opponentController.transform.position = positionPackage.position;
            }
            else // the player is not in the dictionary, yet, he just joined
            {
                // instantiate a new prefab for this player who just joined
                var newOpponentController = Instantiate(Resources.Load<OpponentController>("Opponent"));
                newOpponentController.GetComponent<Blob>().Guid = positionPackage.guid;
                // put the prefab to the position where the player currently says he is
                newOpponentController.transform.position = positionPackage.position;
                // save the instnatiated prefab in a dictionary so we can update the player in the future
                _opponentPlayers[positionPackage.guid] = newOpponentController;
                // remember the ip address and port of this player so we can send him updates
                _opponentEndpoints[positionPackage.guid] = receivePackage.RemoteEndPoint;
            }
        }
    }

    public async void StartHosting()
    {
        _opponentPlayers = new();
        _opponentEndpoints = new();
        GetComponent<Blob>().Guid = System.Guid.NewGuid().ToString();
        _isServer = true;
        
        _udpClient = new UdpClient(44445);
        while (true)
        {
            
            var receivePackage = await _udpClient.ReceiveAsync();
            var byteEncoded = receivePackage.Buffer;
            var jsonEncoded = System.Text.Encoding.UTF8.GetString(byteEncoded);
            var positionPackage = JsonUtility.FromJson<PositionPackage>(jsonEncoded);
        
            if(_opponentPlayers.TryGetValue(positionPackage.guid, out var opponentController))
            {
                // we already instantiated this player. Just update his position
                opponentController.transform.position = positionPackage.position;
            }
            else // the player is not in the dictionary, yet, he just joined
            {
                // instantiate a new prefab for this player who just joined
                var newOpponentController = Instantiate(Resources.Load<OpponentController>("Opponent"));
                newOpponentController.GetComponent<Blob>().Guid = positionPackage.guid;
                // put the prefab to the position where the player currently says he is
                newOpponentController.transform.position = positionPackage.position;
                // save the instnatiated prefab in a dictionary so we can update the player in the future
                _opponentPlayers[positionPackage.guid] = newOpponentController;
                // remember the ip address and port of this player so we can send him updates
                _opponentEndpoints[positionPackage.guid] = receivePackage.RemoteEndPoint;
            }
        }
    }
}