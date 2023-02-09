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
            // NOTA: Tutaj definiuje si� lokalnego gracza.
            //       Tutaj ustala si� wszystko co b�dzie si� dzia�o po stronie lokalnego gracza bez ingerencji w gracza globalnego.
            _local = this;

            // Usuwam widok lokalnego gracza dla siebie
            Utils.SetRenderLayerInChildren(_playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
            Camera.main.gameObject.SetActive(false);
        }
        else
        {
            // NOTA: Tutaj znajduje si� obs�uga dla innego gracza.
            //       Zamiarem jest wy��czenie wszystkiego co posiada gracz lokalny aby ni kolidowa�o to z lokalnymi ustawieniami
            //       oraz nie zaburza�o porz�dku gry.

            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener _audioListiner = GetComponentInChildren<AudioListener>();
            _audioListiner.enabled = false;

        }

        transform.name = $"Player_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        // Ustalam co ma si� sta� po opuszczeniu serwera przez gracza
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
}
