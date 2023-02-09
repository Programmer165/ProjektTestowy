using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponRecoil : MonoBehaviour
{
    private Vector3 _currentRotation, _targetRotation, _targetPosition, _currentPosition, _initialGunPosition;

    public Transform cam;

    [SerializeField] float _recoilX = -2;
    [SerializeField] float _recoilY = 2;
    [SerializeField] float _recoilZ = 7;

    [SerializeField] float _kickBackZ = 0.2f;

    public float _snippiness = 5, _returnAmmount = 8;
    void Start()
    {
        // lokalna pozycja broni
        _initialGunPosition = transform.localPosition;
    }

    void Update()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, Time.deltaTime * _returnAmmount);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, Time.fixedDeltaTime * _snippiness);
        transform.localRotation = Quaternion.Euler(_currentRotation);
        cam.localRotation = Quaternion.Euler(_currentRotation);

        back();
    }

    public void recoil()
    {
        _targetPosition -= new Vector3(0, 0, _kickBackZ);
        _targetRotation += new Vector3(_recoilX, Random.Range(-_recoilY, _recoilY), Random.Range(-_recoilZ, _recoilZ));
    }

    private void back()
    {
        _targetPosition = Vector3.Lerp(_targetPosition, _initialGunPosition, Time.deltaTime * _returnAmmount);
        _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, Time.fixedDeltaTime * _snippiness);
    }
}
