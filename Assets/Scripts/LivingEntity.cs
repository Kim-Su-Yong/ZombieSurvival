using System;
using UnityEngine;
//부모클래스 슈퍼클래스 상위클래스 다형성
//1.게임에서 적과 플레이어는 체력을 가진
//2.체력을 회복한다.
//3.공격을 받는다.
//4.살거나 죽을 수 있다.
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;
    public float health { get; protected set; }
    public bool dead { get; protected set; }
    public event Action onDeath;
    public event Func<int> OnDelete;
    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if(health <= 0 && !dead)
        {
            Die();
        }
    }
    public virtual void RestoreHealth(float newhealth)
    {
        if (dead) return;
        //체력 추가
        health += newhealth;
    }
    public virtual void Die()
    {   //onDeath 이벤트에 등록된 메서드가 있다면 실행
        if(onDeath != null)
        {
            onDeath();
        }
        //사망한 상태를 참으로
        dead = true;
    }
}
