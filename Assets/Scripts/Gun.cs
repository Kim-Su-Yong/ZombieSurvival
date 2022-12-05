using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        READY, EMPTY, RELOADING
        //발사준비, 탄창이 빔, 재장전중
    }
    public State state { get; private set; } //현재 총의 상태
    public Transform fireTransform; //탄알이 발사될 위치
    public ParticleSystem muzzleFlash; //총구 화염효과
    public ParticleSystem shellEject; //탄피 배출 효과

    private LineRenderer bulletLineRenderer; //탄알 궤적을 그리기 위한 렌더러
    private AudioSource gunAudioPlayer; //총소리 재생기
    public AudioClip shotClip; //발사 소리
    public AudioClip reloadClip; //재장전소리
    public float damage = 25f;
    public float fireDistance = 50f; //사정거리
    public int ammoRemain = 100; //남은 전체 탄알
    public int magCapacity = 25; //탄창 용량
    public int magAmmo; //현재 탄창에 남아 있는 탄알

    public float timeBetFire = 0.12f;
    public float reloadTime = 1.8f; //재장잔 소요 시간
    private float lastFireTime; //총일 마지막으로 발사한 시점
    void Awake()
    {
        //사용할 컴포넌트 초기화 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        //사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        //라인 렌더러 비활성화
        bulletLineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        //총 상태 초기화
        //총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        magAmmo = magCapacity;
        //총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        state = State.READY;
    }
    public void Fire() //발사 시도
    {   //현재 상태가 발사 가능한 상태
        //마지막 발사 시점에서 timeBetFire 이상의 시간이 지남
        if(state == State.READY && Time.time >= lastFireTime+timeBetFire)
        {   //마지막 총 발사 시점 갱신
            lastFireTime = Time.time;
            Shot();
        }
    }
    void Shot() //실제 발사
    {
        //레이 캐스트에 의한 충돌 정보를 저장하는 컨테이너
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //레이가 어떤 물체와 충돌한 경우
            //충돌한 상대방으로부터 IDamageable 오브젝트를 가져오기 시도
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if(target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPos = hit.point;
            //레이가 충돌한 위치 저장
        }
        else //레이가 다른 물체와 충돌하지 않았다면
        {   //탄알이 최대 사정거리까지 날아갔을때 위치를 충돌 위치로 사용
            hitPos = fireTransform.position + fireTransform.forward * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPos));

        //남은 탄알 수를 -1
        magAmmo--;
        if(magAmmo <= 0)
        {
            state = State.EMPTY;
        }
    }
    //발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    IEnumerator ShotEffect(Vector3 hitPos)
    {
        muzzleFlash.Play();
        shellEject.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        //선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //선의 끝점은 입력으로 들어온 충돌 위치 맞은 위치
        bulletLineRenderer.SetPosition(1, hitPos);
        //라인 렌더러 활성화하여 탄알 궤적을 그림
        bulletLineRenderer.enabled = true;
        //0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);
        //라인 렌더러를 비활성화하여 탄알 궤적을 지움
        bulletLineRenderer.enabled = false;
    }
    public bool Reload() //재장전 시도
    {
        if(state == State.RELOADING || ammoRemain <=0 || magAmmo >= magCapacity)
            return false;
        //재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine()
    {   //현재 상태를 재장전 중 상태로 전환
        state = State.RELOADING;
        //재장전 소리재생
        gunAudioPlayer.PlayOneShot(reloadClip);
        //재장전 소요시간만큼 처리 쉬기
        yield return new WaitForSeconds(reloadTime);
        //탄창에 채울 탄알을 계산
        int ammoToFill = magCapacity - magAmmo;
        //탄창에 채워야 할 탄알이 남은 탄알보다 많다면
        //채워야 할 탄알 수를 남은 탄알 수에 맞춰 줄임
        if(ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        //탄창을 채움
        magAmmo += ammoToFill;
        //남은 탄알에서 탄창을 채운만큼 탄알을 뺌
        ammoRemain -= ammoToFill;
        //총의 현재상태를 발사준비 상태로 변경
        state = State.READY;
    }
}
