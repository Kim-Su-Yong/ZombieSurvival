using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Gun gun; //사용할 총
    public Transform gunPivot; //총 배치의 기준점
    public Transform leftHandMount; //총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; //총의 오른쪽 손잡이, 오른손이 위치할 지점
    PlayerInput playerInput; //플레이어의 입력
    Animator playerAnimator; //애니메이터 컴포넌트
    private readonly int hashReload = Animator.StringToHash("Reload");
    private void OnEnable()
    {   //슈터가 활성화될 때 총도 같이 활성화
        gun.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        //슈터가 비활성화될 때 총도 같이 비활성화
        gun.gameObject.SetActive(false);
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        //입력을 감지하고 총을 발사하거나 재장전
        if(playerInput.fire)
        {
            gun.Fire();
        }
        else if(playerInput.reload)
        {   //재장전 입력 감지시 재장전
            if(gun.Reload())
            {   //재장전 성공 시에만 재장전 애니메이션 재생
                playerAnimator.SetTrigger(hashReload);
            }
        }
        //남은 탄알UI 갱신
        UpdateUI();
    }
    void UpdateUI()
    {
        if(gun != null && UIManager.instance != null)
        {   //UI매니저의 탄알 텍스트에 탄창의 탄알과 남은 전체 탄알 표시
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }
    //애니메이션 실행시 관절이 IK로 무기 중심으로 움직인다.
    private void OnAnimatorIK(int layerIndex)
    {
        //2가지의 일을 한다.
        //1. 총을 상체와 함께 흔들기
        //2. 캐릭터의 양손을 총의 양쪽 손잡이에 위치 시키기
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        //회전이나 이동 가중치를 100% 만약에 가중치 0.5%가 50%라면
        //원래 위치와 IK목표위치가 절반씩 섞여 적용됨

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        //IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandMount.rotation);
    }
}
