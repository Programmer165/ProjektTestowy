using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.Diagnostics;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer _playerPrefab;

    private PlayerInputHandler _playerInputHandler;

    private void Awake()
    {
        // Jeœli playerPrefab nie jest pusty wtedy wyjdŸ. Jeœli jednak go nie ma wtedy ma za³adowaæ
        if (_playerPrefab != null) return;
        _playerPrefab = Resources.Load("Player").GetComponent<NetworkPlayer>();
    }

    void Start()
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            //Debug.Log("OnPlayerJoined jestem serwerem. Spawnowanie gracza");
            runner.Spawn(_playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
        }
        else Debug.Log("OnPlayerJoined");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_playerInputHandler == null && NetworkPlayer._local != null)
            _playerInputHandler = NetworkPlayer._local.GetComponent<PlayerInputHandler>();

        if (_playerInputHandler != null)
            input.Set(_playerInputHandler.GetNetworkInput());
    }

    public void OnConnectedToServer(NetworkRunner runner) { /*Debug.Log("OnConnectedToServer");*/ }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { /*Debug.Log("OnShutDown"); */ }
    public void OnDisconnectedFromServer(NetworkRunner runner) { /*Debug.Log("OnDisconnectedFromServer"); */ }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { /*Debug.Log("OnConnectRequets"); */ }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { /*Debug.Log("OnConnectFailed"); */ }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
