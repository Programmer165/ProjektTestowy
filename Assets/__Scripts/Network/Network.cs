using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Network : MonoBehaviour
{
    public NetworkRunner _networkRunnerPrefab;
    private NetworkRunner _networkRunner;

    private void Awake()
    {
        // jeœli networkRunnerPrefab nie jest null wtedy pomiñ. A jeœli wtedy ustaw go
        if (_networkRunnerPrefab != null) return;
        _networkRunnerPrefab = Resources.Load("NetworkRunnerPrefab").GetComponent<NetworkRunner>();
    }


    void Start()
    {
        // instancjuje network jako woartoœci gracza wraz z nazw¹
        _networkRunner = Instantiate(_networkRunnerPrefab);
        _networkRunner.name = "Network runner";

        /*var clientTask = */InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        //Debug.Log($"Server NetworkRunner started.");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initilized)
    {
        // pobiera i zapisuje ka¿dy komponent MonoBehaviour typu INetworkSceneObjectProvider
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        // jeœli brak danych
        if (sceneManager == null)
        {
            // przechowuje po³¹czone obiekty które ju¿ istniej¹ na scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        // zwraca start multi
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initilized,
            SceneManager = sceneManager
        });
    }
}
