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
        // Wy³¹czam kamere po to by j¹ zsynchronizowaæ z graczem a nie serwerem. ( Synchronizacja z serwerem sprawi opóŸnienia w obrocie kamer¹ a tego nie chcemy! )
        if (_localCamera.enabled)
            _localCamera.transform.parent = null;
    }

    private void LateUpdate()
    {
        if (_cameraAnchorPoint == null) return;
        if(!_localCamera.enabled) return;

        // Porusza kamer¹ z t¹ sam¹ prêdkoœci¹ co gracz
        _localCamera.transform.position = _cameraAnchorPoint.position;

        // Wyliczam rotacjê
        _cameraRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom._viewUpDownRotationSpeed;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);

        _cameraRotationY += _viewInput.x * Time.deltaTime * _networkCharacterControllerPrototypeCustom._rotationSpeed;

        // I ustawiam jej wartoœci jako wartoœci rotacji kamery
        _localCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 inputView)
    {
        this._viewInput = inputView;
    }
}
