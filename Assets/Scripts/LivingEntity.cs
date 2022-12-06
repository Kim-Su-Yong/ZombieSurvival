using System;
using UnityEngine;
//�θ�Ŭ���� ����Ŭ���� ����Ŭ���� ������
//1.���ӿ��� ���� �÷��̾�� ü���� ����
//2.ü���� ȸ���Ѵ�.
//3.������ �޴´�.
//4.��ų� ���� �� �ִ�.
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
        //ü�� �߰�
        health += newhealth;
    }
    public virtual void Die()
    {   //onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����
        if(onDeath != null)
        {
            onDeath();
        }
        //����� ���¸� ������
        dead = true;
    }
}
