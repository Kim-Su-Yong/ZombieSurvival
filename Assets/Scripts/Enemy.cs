using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Action() Func Random.Range(1,5)
using UnityEngine.AI;
public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget; //������� ���̾�
    private LivingEntity targetEntity; //���� ���
    private NavMeshAgent pathFinder; //��ΰ�� AI ������Ʈ
    public ParticleSystem hitEffect; //�ǰ� �� ����� ��ƼŬ ȿ��
    public AudioClip deathSound; //��� �� ����� �Ҹ�
    public AudioClip hitSound; //�ǰݽ� ����Ҹ�

    private Animator enemyAnimator; //�ִϸ����� ������Ʈ
    private AudioSource enemyAudioPlayer;
    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f; //���� ����
    private float lastAttackTime; //������ ���� ����

    private bool hashTarget //������Ƽ
    {
        get 
        {
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }
    void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        //������ ���۳�Ʈ�� �ڽİ��ӿ�����Ʈ�� ����
        meshRenderer = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
    }
    //�� AI�� �ʱ� ������ �������� �¾� �޼��� �� �����̳� ������ ���⼭ �������� ����
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        meshRenderer.material.color = skinColor;
    }
    private void Start()
    {
        StartCoroutine(UpdatePath());
    }
    private void Update()
    {
        enemyAnimator.SetBool("HasTarget", hashTarget);
    }
    //�ֱ������� ������ ����� ��ġ�� ã�� ��� ����
    IEnumerator UpdatePath()
    {   //��� �ִ� ���� ���� ����
        while(!dead)
        {
            if(hashTarget)
            {
                //���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                pathFinder.isStopped = true;

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                //��� �ݶ��̴��� ��ȸ�ϸ鼭
                for(int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if(livingEntity != null && !livingEntity.dead)
                    {   //���� ����� �ش� LivingEntity�� ����
                        targetEntity = livingEntity;
                        //for�� ���� ��� ����
                        break;
                    }
                }
            }
            //0.25�� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }
    //������� �Ծ����� ������ ó��
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void Die()
    {
        base.Die();
    }
    private void OnTriggerStay(Collider other)
    {
        //Ʈ���Ű� �浹�� ���� ���ӿ�����Ʈ�� ��������̶�� ���� ����

    }
}
