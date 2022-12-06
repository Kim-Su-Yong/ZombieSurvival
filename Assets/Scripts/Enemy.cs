using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Action() Func Random.Range(1,5)
using UnityEngine.AI;
public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget; //추적대상 레이어
    private LivingEntity targetEntity; //추적 대상
    private NavMeshAgent pathFinder; //경로계산 AI 에이전트
    public ParticleSystem hitEffect; //피격 시 재생할 파티클 효과
    public AudioClip deathSound; //사망 시 재생할 소리
    public AudioClip hitSound; //피격시 재생소리

    private Animator enemyAnimator; //애니메이터 컴포넌트
    private AudioSource enemyAudioPlayer;
    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f; //공격 간격
    private float lastAttackTime; //마지막 공격 시점

    private bool hashTarget //프로퍼티
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

        //렌더러 컴퍼넌트는 자식게임오브젝트에 있음
        meshRenderer = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
    }
    //적 AI의 초기 스펙을 결정짓는 셋업 메서드 적 색깔이나 맷집을 여기서 랜덤으로 설정
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
    //주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    IEnumerator UpdatePath()
    {   //살아 있는 동안 무한 루프
        while(!dead)
        {
            if(hashTarget)
            {
                //추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                pathFinder.isStopped = true;

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                //모든 콜라이더를 순회하면서
                for(int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if(livingEntity != null && !livingEntity.dead)
                    {   //추적 대상을 해당 LivingEntity로 설정
                        targetEntity = livingEntity;
                        //for문 루프 즉시 정지
                        break;
                    }
                }
            }
            //0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }
    //대미지를 입었을때 실행할 처리
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
        //트리거가 충돌한 상대방 게임오브젝트가 추적대상이라면 공격 실행

    }
}
