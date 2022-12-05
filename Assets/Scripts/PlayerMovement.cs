using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; //앞뒤 움직임의 속도
    public float rotateSpeed = 180f; //좌우 회전 속도
    
    private PlayerInput playerInput; //플레이어 입력을 알려주는 컴포넌트

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    private readonly int hashMove = Animator.StringToHash("Move");

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    
    void FixedUpdate()
    {
        Rotate();
        Move();

        playerAnimator.SetFloat(hashMove, playerInput.move);
    }

    void Move()
    {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }
    void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}
