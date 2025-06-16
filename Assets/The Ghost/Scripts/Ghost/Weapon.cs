using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float TimePressed { get; private set; }
    public float ShootThreshold { 
        get { return _shootThreshold; } 
    }

    public float NextBulletLoadingTimer
    {
        get { return _nextBulletLoadingTimer; }
    }

    public WeaponHandPositionSetting WeaponHandPositionSetting 
    {
        get { return _weaponHandPositionSetting; }
    }

    public Transform SecondHandHoldingPoint
    {
        get { return _secondHandHoldingPoint;  }
    }

    public WeaponRecoil WeaponRecoil
    {
        get { return _weaponRecoil; }
    }

    [SerializeField] private float _nextBulletLoadingTimer;
    [SerializeField] private float nextBulletLoadingElapse;
    [SerializeField] private float _shootThreshold;
    [SerializeField] private Transform _secondHandHoldingPoint;

    [SerializeField] private WeaponRecoil _weaponRecoil;

    public delegate void OnWeaponShoot();
    public OnWeaponShoot onWeaponShoot;

    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private GameObject muzzleFlashParent;
    [SerializeField] private GameObject bulletStartingPoint;
    [SerializeField] private ParticleSystem[] particleSystem;
    [SerializeField] private WeaponHandPositionSetting _weaponHandPositionSetting;

    public bool Equip { get; set; }

    private bool pullTrigger;

    // Start is called before the first frame update
    void Awake()
    {
        _weaponRecoil = GetComponent<WeaponRecoil>();

        particleSystem = muzzleFlashParent.GetComponentsInChildren<ParticleSystem>();
        muzzleFlashParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (! Equip) return;

        if(pullTrigger)
        {
            TimePressed += Time.deltaTime * 1f;


            if (TimePressed < _shootThreshold)
                return;

            TimePressed = _shootThreshold;

            if (_nextBulletLoadingTimer <= 0)
            {
                muzzleFlashParent.SetActive(true);
                Debug.Log("Shoot");

                onWeaponShoot?.Invoke();

                PlayParticleEffects();

                CreateProjectile();

                _weaponRecoil.StartWeaponRecoil();
                //TimePressed = 0;
                _nextBulletLoadingTimer = nextBulletLoadingElapse;
            }
        }
        else
        {
            TimePressed = 0;
            muzzleFlashParent.SetActive(false);
        }

        if(_nextBulletLoadingTimer >= 0)
            _nextBulletLoadingTimer -= Time.deltaTime;
    }

    private void PlayParticleEffects()
    {
        for(int x = 0; x < particleSystem.Length; x++)
        {
            particleSystem[x].Play();
        }
    }

    private void CreateProjectile()
    {
        Rigidbody bullet =  Instantiate(bulletPrefab, bulletStartingPoint.transform.position, bulletStartingPoint.transform.rotation) as Rigidbody;
        bullet.AddForce(bullet.transform.forward * 1000000 * Time.deltaTime);
        
    }

    public void Shoot(bool input)
    {
        pullTrigger = input;
    }

}
