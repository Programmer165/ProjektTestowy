using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Unity.VisualScripting;

public class WeponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    public ParticleSystem fireParticle;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    public WeponRecoil _weponRecoil;
    public Transform particleSpawner;

    private float _lastTimeFired = 0;

    private HPHandler _hpHandler;
    private Transform _bulletPrefub;

    private void Awake()
    {
        fireParticle = Resources.Load("Particle/Shot_Particle").GetComponent<ParticleSystem>();
        _hpHandler = GetComponent<HPHandler>();
        _weponRecoil = GetComponentInChildren<WeponRecoil>();
        _bulletPrefub = Resources.Load("Bullet_Prefab").GetComponent<Transform>();
    }

    void Start()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (_hpHandler.isDead) return;

        // Wyci¹ga wejœcie z po³¹czenia
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData._isFireButtonPressed)
                Fire(networkInputData._aimForwardVector);
        }
    }

    void Fire(Vector3 aimForwardVector)
    {
        // Limit szybkostrzelnoœci
        if (Time.time - _lastTimeFired < 0.15f) return;

        StartCoroutine(FireEffectCO());

        // Tworzê promieñ 
        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100f, Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100f;
        bool isHitOtherPlayer = false;

        Vector3 hitPos = Vector3.zero;
        Quaternion hitRot = Quaternion.identity;

        // Gdy trafiê w siebie mam przestaæ trafiaæ
        if (hitInfo.Distance > 0)
            hitDistance = hitInfo.Distance;


        // Jeœli trafiê w Hitbox lub Collider
        if(hitInfo.Hitbox != null)
        {
            // odbieraj HP gdy trafiony w Hitbox
            if (Object.HasStateAuthority)
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage();

            isHitOtherPlayer = true;
        }

        if (isHitOtherPlayer)
        {
            //Debug.DrawRay(aimPoint.position, aimForwardVector * _hitDistance, Color.red, 1);
        }
        else
        {
            //Debug.DrawRay(aimPoint.position, aimForwardVector * _hitDistance, Color.green, 1);
            // ustalam punkt trafienia oraz rotacjê a nastêpnie spawnuje pocisk w miejscu trafienia
            hitPos = hitInfo.Point;
            hitRot = Quaternion.FromToRotation(Vector3.forward, hitInfo.Normal);
            Instantiate(_bulletPrefub, hitPos, hitRot);
        }


        _lastTimeFired = Time.time;
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;
        var particle = Instantiate(fireParticle, particleSpawner.position, Quaternion.identity);
        _weponRecoil.recoil();
        yield return new WaitForSeconds(.277f);

        Destroy(particle);
        isFiring = false;
    }

    static void OnFireChanged(Changed<WeponHandler> changed)
    {
        bool isFiringCurrent = changed.Behaviour.isFiring;

        // £aduje star¹ wartoœc
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
            changed.Behaviour.OnFireRemote();
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority) fireParticle.Play();
    }
}
