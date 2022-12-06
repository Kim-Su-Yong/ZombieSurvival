using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;

    public AudioClip deathClip; //사망소리
    public AudioClip hitClip; //피격소리
    public AudioClip itemPickUpClip; //아이템 습득소리

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;
    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }
    protected override void OnEnable()
    {
        //LivingEntity의 OnEnable() 실행 상태 초기화
        base.OnEnable();
        //체력슬라이더 활성화
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }
    public override void RestoreHealth(float newhealth)
    {
        //LivingEntity의 RestoreHealth() 실행 (체력증가)
        base.RestoreHealth(newhealth);

        healthSlider.value = health;
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {  
        if(!dead)
        {   //사망하지 않은 경우에만 효과음 재생
            playerAudioPlayer.PlayOneShot(hitClip);
        }
        //LivingEntity의 OnDamage() 실행 (대미지 적용)
        base.OnDamage(damage, hitPoint, hitNormal);
        //갱신된 체력을 체력 슬라이더에 반영
        healthSlider.value = health;
    }
    void Update()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);

        playerAudioPlayer.PlayOneShot(deathClip);
        playerAnimator.SetTrigger("Die");

        //플레이어 조작을 받는 컴포넌트 비활성화
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //아이템 습득할때 아이템과 충돌한 경우
        if(!dead)
        {   //충돌한 상대방으로부터 IItem 컴포터넌트를 가져오기
            IItem item = other.GetComponent<IItem>();
            if(item != null)
            {
                item.Use(gameObject);
                playerAudioPlayer.PlayOneShot(itemPickUpClip);
            }
        }
    }
}
