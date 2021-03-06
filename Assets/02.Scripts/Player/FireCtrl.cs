﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlayerSFX
{
	public AudioClip[] fireClips;
	public AudioClip[] reloadClips;
}
public class FireCtrl : MonoBehaviour {

	// 무기 타입을 숫자로 정의할 열거형 자료
	public enum WeaponType
	{
		RIFLE = 0,
		SHOTHUN,
	}

	// 주인공이 현재 들고 있는 무기의 타입을 저장할 변수
	public WeaponType currentWeapon = WeaponType.RIFLE;

    // 총알 프리팹
    public GameObject bulletPrefab;

	// 각종 SFX 클립들을 로드할 클래스 변수
	public PlayerSFX playerSFX;

	AudioSource audioSource;

    Shaker shaker;

    // 탄창 ImageUI
    public Image magazineImg;

    // 남은 총알 TextUI
    public Text magazineText;

    // 최대 총알수
    public int maxBullet = 10;

    // 남은 총알수
    public int remainBullet = 10;

    public float reloadTime = 2.0f;

    public bool isReloading = false;

    public Sprite[] weaponIcons;

    public Image weaponChangeImage;

    // 변경할 무기 Object 컴포넌트
    public Weapon[] weapons;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
        shaker = GameObject.Find("CameraRig").GetComponent<Shaker>();
    }
	
	// Update is called once per frame
	void Update () {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            --remainBullet;

            UpdateBullet();
            // 총알 발사 함수 호출
            Fire();

            if (remainBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
	}

    IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        audioSource.PlayOneShot(playerSFX.reloadClips[(int)currentWeapon], 1.0f);

        yield return new WaitForSeconds(playerSFX.reloadClips[(int)currentWeapon].length + 0.1f);

        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainBullet = maxBullet;

        UpdateBullet();
    }

    void UpdateBullet()
    {
        magazineImg.fillAmount = remainBullet / (float)maxBullet;

        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainBullet, maxBullet);
    }
    // 총알 발사 함수
    void Fire()
    {
        // 카메라 Shaker 효과 발동
        shaker.StartCoroutine(shaker.ShakeCamera());
        // Bullet 프리팹을 복사해 인스턴스화
        Weapon cweapon = weapons[(int)currentWeapon];
        Transform firePos = cweapon.firePos;

        var bullet = GameManager.instance.GetBullet();
        if(bullet != null)
        {
            bullet.transform.position = firePos.position;
            bullet.transform.rotation = firePos.rotation;
            bullet.SetActive(true);
        }
        
        cweapon.catrige.Play();
        cweapon.muzzleFlash.Play();
		FireSFX();
    }

	void FireSFX()
	{
		// 현재 들고있는 무기 타입에 맞게 오디오 클립을 가져옴
		var _SFX = playerSFX.fireClips[(int)currentWeapon];
		audioSource.pitch = 1.0f + Random.Range(-0.2f, 0.2f);
		audioSource.PlayOneShot(_SFX, 1.0f);
	}

    public void OnChangeWeapon()
    {
        weapons[(int)currentWeapon].gameObject.SetActive(false);
        currentWeapon = (WeaponType)((int)++currentWeapon % 2);
        weaponChangeImage.sprite = weaponIcons[(int)currentWeapon];
        weapons[(int)currentWeapon].gameObject.SetActive(true);

        audioSource.PlayOneShot(playerSFX.reloadClips[(int)currentWeapon], 1.0f);
    }
}
