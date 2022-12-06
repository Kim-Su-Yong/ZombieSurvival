using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;

    public AudioClip deathClip; //����Ҹ�
    public AudioClip hitClip; //�ǰݼҸ�
    public AudioClip itemPickUpClip; //������ ����Ҹ�

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
        //LivingEntity�� OnEnable() ���� ���� �ʱ�ȭ
        base.OnEnable();
        //ü�½����̴� Ȱ��ȭ
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }
    public override void RestoreHealth(float newhealth)
    {
        //LivingEntity�� RestoreHealth() ���� (ü������)
        base.RestoreHealth(newhealth);

        healthSlider.value = health;
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {  
        if(!dead)
        {   //������� ���� ��쿡�� ȿ���� ���
            playerAudioPlayer.PlayOneShot(hitClip);
        }
        //LivingEntity�� OnDamage() ���� (����� ����)
        base.OnDamage(damage, hitPoint, hitNormal);
        //���ŵ� ü���� ü�� �����̴��� �ݿ�
        healthSlider.value = health;
    }
    void Update()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);

        playerAudioPlayer.PlayOneShot(deathClip);
        playerAnimator.SetTrigger("Die");

        //�÷��̾� ������ �޴� ������Ʈ ��Ȱ��ȭ
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //������ �����Ҷ� �����۰� �浹�� ���
        if(!dead)
        {   //�浹�� �������κ��� IItem �����ͳ�Ʈ�� ��������
            IItem item = other.GetComponent<IItem>();
            if(item != null)
            {
                item.Use(gameObject);
                playerAudioPlayer.PlayOneShot(itemPickUpClip);
            }
        }
    }
}
