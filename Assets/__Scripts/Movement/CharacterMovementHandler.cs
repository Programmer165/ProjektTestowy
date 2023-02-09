using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    private bool _isRespawnedRequested = false;

    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    private HPHandler _hpHandler;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        _hpHandler = GetComponent<HPHandler>();
    }

    void Start()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority)
        {
            if(_isRespawnedRequested)
            {
                Respawn();
                return;
            }

            if (_hpHandler.isDead) return;
        }

        // pobieram input z po³¹czenia
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Rotacja transforma. ( Nie jest to najlepszy sposób. Przy ekstremalnych lagach gracz mo¿e zacz¹æ bardzo szybko siê obracaæ! )
            transform.forward = networkInputData._aimForwardVector; // TODO: Poprawiæ na coœ lepszego! dobrze bêdzie to lerpowaæ

            // Aby zapobiegaæ obracaniu siê modelu wraz z myszk¹ trzeba przerobiæ rotacjê
            //  po wykluczeniu osi x wszystko powinno dobrze dzia³aæ
            Quaternion _rotation = transform.rotation;
            _rotation.eulerAngles = new Vector3(0, _rotation.eulerAngles.y, _rotation.eulerAngles.z);
            transform.rotation = _rotation;

            // (Ruch) Tworzê wartoœci ruchu kierunkowego i je normalizujê
            Vector3 _moveDirection = transform.forward * networkInputData._movementInput.y + transform.right * networkInputData._movementInput.x;
            _moveDirection.Normalize();

            // Ustawiam wartoœci kierunkowe jako ruch
            _networkCharacterControllerPrototypeCustom.Move(_moveDirection);

            // Skok
            if (networkInputData._isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();

            // Funkcja spawnuje gracza na mapie gdyby przypadkiem wypad³ poza mapê
            CheckFallRespawn();
        }
    }

    private void CheckFallRespawn()
    {
        if (transform.position.y < -10)
            if (Object.HasStateAuthority)
                Respawn();
    }

    public void RequestSpawn()
    {
        _isRespawnedRequested = true;
    }

    private void Respawn()
    {
        Destroy(GameObject.Find("KilledAnim(Clone)"));
        _networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());
        _hpHandler.OnRespawned();

        _isRespawnedRequested = false;
    }

    public void SetCharacterControlerEnabled(bool isEnabled)
    {
        _networkCharacterControllerPrototypeCustom.enabled = isEnabled;
    }
}
