using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform _cameraAnchorPoint;

    // Input
    Vector2 _viewInput;

    // Rotacja
    float _cameraRotationX = 0;
    float _cameraRotationY = 0;

    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    private Camera _localCamera;

    private void Awake()
    {
        _localCamera = GetComponent<Camera>();
        _networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        // Wy��czam kamere po to by j� zsynchronizowa� z graczem a nie serwerem. ( Synchronizacja z serwerem sprawi op�nienia w obrocie kamer� a tego nie chcemy! )
        if (_localCamera.enabled)
            _localCamera.transform.parent = null;
    }

    private void LateUpdate()
    {
        if (_cameraAnchorPoint == null) return;
        if(!_localCamera.enabled) return;

        // Porusza kamer� z t� sam� pr�dko�ci� co gracz
        _localCamera.transform.position = _cameraAnchorPoint.position;

        // Wyliczam rotacj�
        _cameraRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom._viewUpDownRotationSpeed;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);

        _cameraRotationY += _viewInput.x * Time.deltaTime * _networkCharacterControllerPrototypeCustom._rotationSpeed;

        // I ustawiam jej warto�ci jako warto�ci rotacji kamery
        _localCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 inputView)
    {
        this._viewInput = inputView;
    }
}
