using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChange))]
    private byte _hp { get; set; }

    [Networked(OnChanged = nameof(OnStateChange))]
    public bool isDead { get; set; }
    private bool _isInitialized = false;

    const byte _startingHP = 5;

    public Color uiDamageHitColor;
    public Image uiDamageImage;
    public MeshRenderer bodyMeshRender;
    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;

    private Color _defaultBodyMeshColor;

    private CharacterMovementHandler _characterMovementHandler;
    private HitboxRoot _hitboxRoot;

    public Text _hp_text;

    private void Awake()
    {
        _characterMovementHandler = GetComponent<CharacterMovementHandler>();
        _hitboxRoot = GetComponentInChildren<HitboxRoot>();
        //_hp_text = GetComponentInParent<Text>();
    }

    void Start()
    {
        // ustawiam dane gracza (hp oraz info ¿e ¿yje)
        _hp = _startingHP;
        isDead = false;

        // ustalam aktualny kolor gracza
        _defaultBodyMeshColor = bodyMeshRender.material.color;
        _isInitialized = true;
    }

    private void Update()
    {
        if (Object.HasInputAuthority)
            _hp_text.text = $"HP: {_hp}";
    }

    IEnumerator OnHitCO()
    {
        // ustalam wstêpny wygl¹d cia³a gracza
        bodyMeshRender.material.color = Color.red;

        // gdy trafi w gracza wtedy UI pokazuje trafienie
        if (Object.HasInputAuthority)
            uiDamageImage.color = uiDamageHitColor;

        yield return new WaitForSeconds(0.2f);

        // po trafieniu przywraca ustawienia cia³a do normalnych
        bodyMeshRender.material.color = _defaultBodyMeshColor;

        // jeœli gracz prze¿y³ wtedy poka¿ ekran trafienia
        if (Object.HasInputAuthority && !isDead)
            uiDamageImage.color = new Color(0, 0, 0, 0);
    }

    IEnumerator OnServerReviveCO()
    {
        yield return new WaitForSeconds(3.0f);
        _characterMovementHandler.RequestSpawn();
    }

    public void OnTakeDamage()
    {
        // jeœli umarliœmy ma ju¿ nie odbiraæ ¿ycia
        if (isDead) return;

        _hp -= 1;

        if (_hp <= 0)
        {
            // stwierdzam ¿e zmar³
            isDead = true;

            // odliczam czas do ponownego spawnu
            StartCoroutine(OnServerReviveCO());
        }
    }

    private static void OnHPChange(Changed<HPHandler> changed)
    {
        //Debug.Log($"OnHPChange wartoœæ {changed.Behaviour._hp}");

        // ustalam nowe hp jako aktualne( byte poniewa¿ nie trzeba wielkich liczb 32bit do przechowywania hp = 5 )
        byte _newHP = changed.Behaviour._hp;

        // ³adujê star¹ wartoœæ dla porównania
        changed.LoadOld();
        byte _oldHP = changed.Behaviour._hp;

        // jeœli nowe HP jest mniejsze wtedy mam zmniejszaæ hp przeciwnika
        if (_newHP < _oldHP)
            changed.Behaviour.OnHPReduced();
    }

    private void OnHPReduced()
    {
        if (!_isInitialized) return;

        // wyliczam odejmowanie hp
        StartCoroutine(OnHitCO());
    }

    private static void OnStateChange(Changed<HPHandler> changed)
    {
        // zapamiêtujê aktualny stan œmieci
        bool _isDeadCurrent = changed.Behaviour.isDead;

        // ³adujê stary i zapamiêtujê dla porównania
        changed.LoadOld();
        bool _isDeadOld = changed.Behaviour.isDead;

        // zak³adam ¿e jeœli zmar³ wtedy zabijam go a jeœli nie zmar³ ale o¿y³ wtedy trzeba go respawnowaæ
        if (_isDeadCurrent)
            // znaczy ¿e znar³
            changed.Behaviour.OnDeath();
        else if (!_isDeadCurrent && _isDeadOld)
            changed.Behaviour.OnRevive();
    }

    void OnDeath()
    {
        // wy³¹czam model, hitbox i ruch gracza
        playerModel.gameObject.SetActive(false);
        _hitboxRoot.HitboxRootActive = false;
        _characterMovementHandler.SetCharacterControlerEnabled(false);

        // respawnujê go w randomowej pozycji
        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    void OnRevive()
    {
        // gdy gracz odrodzony wtedy usuñ ekran œmieci
        if (Object.HasInputAuthority)
            uiDamageImage.color = new Color(0, 0, 0, 0);

        // w³¹cz model hitbox i ruch gracza
        playerModel.gameObject.SetActive(true);
        _hitboxRoot.HitboxRootActive = true;
        _characterMovementHandler.SetCharacterControlerEnabled(true);
    }

    public void OnRespawned()
    {
        // Respawn przywraca ponownie statystyki gracza
        _hp = _startingHP;
        isDead = false;
    }
}
