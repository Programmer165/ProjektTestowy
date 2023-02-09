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
        // ustawiam dane gracza (hp oraz info �e �yje)
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
        // ustalam wst�pny wygl�d cia�a gracza
        bodyMeshRender.material.color = Color.red;

        // gdy trafi w gracza wtedy UI pokazuje trafienie
        if (Object.HasInputAuthority)
            uiDamageImage.color = uiDamageHitColor;

        yield return new WaitForSeconds(0.2f);

        // po trafieniu przywraca ustawienia cia�a do normalnych
        bodyMeshRender.material.color = _defaultBodyMeshColor;

        // je�li gracz prze�y� wtedy poka� ekran trafienia
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
        // je�li umarli�my ma ju� nie odbira� �ycia
        if (isDead) return;

        _hp -= 1;

        if (_hp <= 0)
        {
            // stwierdzam �e zmar�
            isDead = true;

            // odliczam czas do ponownego spawnu
            StartCoroutine(OnServerReviveCO());
        }
    }

    private static void OnHPChange(Changed<HPHandler> changed)
    {
        //Debug.Log($"OnHPChange warto�� {changed.Behaviour._hp}");

        // ustalam nowe hp jako aktualne( byte poniewa� nie trzeba wielkich liczb 32bit do przechowywania hp = 5 )
        byte _newHP = changed.Behaviour._hp;

        // �aduj� star� warto�� dla por�wnania
        changed.LoadOld();
        byte _oldHP = changed.Behaviour._hp;

        // je�li nowe HP jest mniejsze wtedy mam zmniejsza� hp przeciwnika
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
        // zapami�tuj� aktualny stan �mieci
        bool _isDeadCurrent = changed.Behaviour.isDead;

        // �aduj� stary i zapami�tuj� dla por�wnania
        changed.LoadOld();
        bool _isDeadOld = changed.Behaviour.isDead;

        // zak�adam �e je�li zmar� wtedy zabijam go a je�li nie zmar� ale o�y� wtedy trzeba go respawnowa�
        if (_isDeadCurrent)
            // znaczy �e znar�
            changed.Behaviour.OnDeath();
        else if (!_isDeadCurrent && _isDeadOld)
            changed.Behaviour.OnRevive();
    }

    void OnDeath()
    {
        // wy��czam model, hitbox i ruch gracza
        playerModel.gameObject.SetActive(false);
        _hitboxRoot.HitboxRootActive = false;
        _characterMovementHandler.SetCharacterControlerEnabled(false);

        // respawnuj� go w randomowej pozycji
        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    void OnRevive()
    {
        // gdy gracz odrodzony wtedy usu� ekran �mieci
        if (Object.HasInputAuthority)
            uiDamageImage.color = new Color(0, 0, 0, 0);

        // w��cz model hitbox i ruch gracza
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
