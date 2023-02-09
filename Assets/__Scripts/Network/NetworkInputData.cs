using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 _movementInput;
    public Vector3 _aimForwardVector;
    public NetworkBool _isJumpPressed;
    public NetworkBool _isFireButtonPressed;
}
