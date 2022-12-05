using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Gun gun; //����� ��
    public Transform gunPivot; //�� ��ġ�� ������
    public Transform leftHandMount; //���� ���� ������, �޼��� ��ġ�� ����
    public Transform rightHandMount; //���� ������ ������, �������� ��ġ�� ����
    PlayerInput playerInput; //�÷��̾��� �Է�
    Animator playerAnimator; //�ִϸ����� ������Ʈ
    private readonly int hashReload = Animator.StringToHash("Reload");
    private void OnEnable()
    {   //���Ͱ� Ȱ��ȭ�� �� �ѵ� ���� Ȱ��ȭ
        gun.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        //���Ͱ� ��Ȱ��ȭ�� �� �ѵ� ���� ��Ȱ��ȭ
        gun.gameObject.SetActive(false);
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        //�Է��� �����ϰ� ���� �߻��ϰų� ������
        if(playerInput.fire)
        {
            gun.Fire();
        }
        else if(playerInput.reload)
        {   //������ �Է� ������ ������
            if(gun.Reload())
            {   //������ ���� �ÿ��� ������ �ִϸ��̼� ���
                playerAnimator.SetTrigger(hashReload);
            }
        }
        //���� ź��UI ����
        UpdateUI();
    }
    void UpdateUI()
    {
        if(gun != null && UIManager.instance != null)
        {   //UI�Ŵ����� ź�� �ؽ�Ʈ�� źâ�� ź�˰� ���� ��ü ź�� ǥ��
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }
    //�ִϸ��̼� ����� ������ IK�� ���� �߽����� �����δ�.
    private void OnAnimatorIK(int layerIndex)
    {
        //2������ ���� �Ѵ�.
        //1. ���� ��ü�� �Բ� ����
        //2. ĳ������ ����� ���� ���� �����̿� ��ġ ��Ű��
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� �����̿� ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        //ȸ���̳� �̵� ����ġ�� 100% ���࿡ ����ġ 0.5%�� 50%���
        //���� ��ġ�� IK��ǥ��ġ�� ���ݾ� ���� �����

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        //IK�� ����Ͽ� �������� ��ġ�� ȸ���� ���� ������ �����̿� ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandMount.rotation);
    }
}
