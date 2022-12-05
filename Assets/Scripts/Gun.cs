using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        READY, EMPTY, RELOADING
        //�߻��غ�, źâ�� ��, ��������
    }
    public State state { get; private set; } //���� ���� ����
    public Transform fireTransform; //ź���� �߻�� ��ġ
    public ParticleSystem muzzleFlash; //�ѱ� ȭ��ȿ��
    public ParticleSystem shellEject; //ź�� ���� ȿ��

    private LineRenderer bulletLineRenderer; //ź�� ������ �׸��� ���� ������
    private AudioSource gunAudioPlayer; //�ѼҸ� �����
    public AudioClip shotClip; //�߻� �Ҹ�
    public AudioClip reloadClip; //�������Ҹ�
    public float damage = 25f;
    public float fireDistance = 50f; //�����Ÿ�
    public int ammoRemain = 100; //���� ��ü ź��
    public int magCapacity = 25; //źâ �뷮
    public int magAmmo; //���� źâ�� ���� �ִ� ź��

    public float timeBetFire = 0.12f;
    public float reloadTime = 1.8f; //������ �ҿ� �ð�
    private float lastFireTime; //���� ���������� �߻��� ����
    void Awake()
    {
        //����� ������Ʈ �ʱ�ȭ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        //����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        //���� ������ ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        //�� ���� �ʱ�ȭ
        //���� ���� ���¸� ���� �� �غ� �� ���·� ����
        magAmmo = magCapacity;
        //���� ���� ���¸� ���� �� �غ� �� ���·� ����
        state = State.READY;
    }
    public void Fire() //�߻� �õ�
    {   //���� ���°� �߻� ������ ����
        //������ �߻� �������� timeBetFire �̻��� �ð��� ����
        if(state == State.READY && Time.time >= lastFireTime+timeBetFire)
        {   //������ �� �߻� ���� ����
            lastFireTime = Time.time;
            Shot();
        }
    }
    void Shot() //���� �߻�
    {
        //���� ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //���̰� � ��ü�� �浹�� ���
            //�浹�� �������κ��� IDamageable ������Ʈ�� �������� �õ�
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if(target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPos = hit.point;
            //���̰� �浹�� ��ġ ����
        }
        else //���̰� �ٸ� ��ü�� �浹���� �ʾҴٸ�
        {   //ź���� �ִ� �����Ÿ����� ���ư����� ��ġ�� �浹 ��ġ�� ���
            hitPos = fireTransform.position + fireTransform.forward * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPos));

        //���� ź�� ���� -1
        magAmmo--;
        if(magAmmo <= 0)
        {
            state = State.EMPTY;
        }
    }
    //�߻� ����Ʈ�� �Ҹ��� ����ϰ� ź�� ������ �׸�
    IEnumerator ShotEffect(Vector3 hitPos)
    {
        muzzleFlash.Play();
        shellEject.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        //���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //���� ������ �Է����� ���� �浹 ��ġ ���� ��ġ
        bulletLineRenderer.SetPosition(1, hitPos);
        //���� ������ Ȱ��ȭ�Ͽ� ź�� ������ �׸�
        bulletLineRenderer.enabled = true;
        //0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);
        //���� �������� ��Ȱ��ȭ�Ͽ� ź�� ������ ����
        bulletLineRenderer.enabled = false;
    }
    public bool Reload() //������ �õ�
    {
        if(state == State.RELOADING || ammoRemain <=0 || magAmmo >= magCapacity)
            return false;
        //������ ó�� ����
        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine()
    {   //���� ���¸� ������ �� ���·� ��ȯ
        state = State.RELOADING;
        //������ �Ҹ����
        gunAudioPlayer.PlayOneShot(reloadClip);
        //������ �ҿ�ð���ŭ ó�� ����
        yield return new WaitForSeconds(reloadTime);
        //źâ�� ä�� ź���� ���
        int ammoToFill = magCapacity - magAmmo;
        //źâ�� ä���� �� ź���� ���� ź�˺��� ���ٸ�
        //ä���� �� ź�� ���� ���� ź�� ���� ���� ����
        if(ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        //źâ�� ä��
        magAmmo += ammoToFill;
        //���� ź�˿��� źâ�� ä�ŭ ź���� ��
        ammoRemain -= ammoToFill;
        //���� ������¸� �߻��غ� ���·� ����
        state = State.READY;
    }
}
