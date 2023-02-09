using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer _local { get; set; }

    public Transform _playerModel;

    private void Awake()
    {
        if (_playerModel != null) return;
        _playerModel = transform.GetChild(0).GetComponent<Transform>();
    }

    void Start()
    {
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // NOTA: Tutaj definiuje siê lokalnego gracza.
            //       Tutaj ustala siê wszystko co bêdzie siê dzia³o po stronie lokalnego gracza bez ingerencji w gracza globalnego.
            _local = this;

            // Usuwam widok lokalnego gracza dla siebie
            Utils.SetRenderLayerInChildren(_playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
            Camera.main.gameObject.SetActive(false);
        }
        else
        {
            // NOTA: Tutaj znajduje siê obs³uga dla innego gracza.
            //       Zamiarem jest wy³¹czenie wszystkiego co posiada gracz lokalny aby ni kolidowa³o to z lokalnymi ustawieniami
            //       oraz nie zaburza³o porz¹dku gry.

            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener _audioListiner = GetComponentInChildren<AudioListener>();
            _audioListiner.enabled = false;

        }

        transform.name = $"Player_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        // Ustalam co ma siê staæ po opuszczeniu serwera przez gracza
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
}
