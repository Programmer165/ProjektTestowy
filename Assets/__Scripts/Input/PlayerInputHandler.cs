using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    Vector2 _moveInputVec = Vector2.zero;
    Vector2 _viewInputVec = Vector2.zero;
    bool _isJumpBtnPressed = false;
    bool _isFireBtnPressed = false;

    private LocalCameraHandler _localCameraHandler;
    private CharacterMovementHandler _characterMovementHandler;

    private void Awake()
    {
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        _characterMovementHandler = GetComponentInChildren<CharacterMovementHandler>();
    }

    void Start()
    {
        // Blokuj� i wy��czam widoczno�� myszki
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Je�li mamy kontrol� wtedy przechodzi je�li nie wtedy wraca ( chroni przed kontrolowaniem innego u�ytkownika )
        if (!_characterMovementHandler.Object.HasInputAuthority) return;

        // Myszka ruch
        _viewInputVec.x = Input.GetAxis("Mouse X");
        _viewInputVec.y = Input.GetAxis("Mouse Y") * -1; // odwr�cenie widoku

        // Przyciski ruchu
        _moveInputVec.x = Input.GetAxis("Horizontal");
        _moveInputVec.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            _isJumpBtnPressed = true;

        if (Input.GetButtonDown("Fire1"))
            _isFireBtnPressed = true;

        _localCameraHandler.SetViewInputVector(_viewInputVec);

        // TODO: usun�� jutor i zrobi� lepsze opuszczanie gry.
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        // Dane widoku
        networkInputData._aimForwardVector = _localCameraHandler.transform.forward;

        // Dane ruchu
        networkInputData._movementInput = _moveInputVec;

        // Dane strza�u
        networkInputData._isFireButtonPressed = _isFireBtnPressed;

        // Dane skoku
        networkInputData._isJumpPressed = _isJumpBtnPressed;

        _isJumpBtnPressed = false;
        _isFireBtnPressed = false;

        return networkInputData;
    }
}
