﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region フィールド

    private Transform _transform;
    private Rigidbody _rigidbody;
    private Animator animator;

    private float moveSpeed;
    private float rotateSpeed;
    private Vector3 moveDirection;
    private Vector3 rotateDirection;

    #endregion


    #region プロパティ

    private bool IsRunning { get { return moveDirection.z > 0f; } }
    private bool IsTurning { get { return rotateDirection.magnitude != 0f; } }

    #endregion


    #region Unity メソッド

    /// <summary> 
    /// 初期化処理
    /// </summary>
    void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        moveSpeed = 20.0f;
        rotateSpeed = 3.0f;
        moveDirection = Vector3.zero;
        rotateDirection = Vector3.zero;
    }

    /// <summary> 
    /// 1frame毎の更新処理
    /// </summary>
    void Update()
    {
        InputDirection();
    }

    /// <summary> 
    /// 一定秒数毎の更新処理
    /// </summary>
    void FixedUpdate()
    {
        Turn();
        Move();
        Animate();
    }

    #endregion


    #region メソッド

    /// <summary>
    /// プレイヤーの入力方向取得
    /// </summary>
    private void InputDirection()
    {
        float y = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(0f, 0f, z);
        rotateDirection = new Vector3(0f, y, 0f);
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void Move()
    {
        if (!IsRunning)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }
        _rigidbody.velocity = _transform.forward * moveSpeed;
    }

    /// <summary>
    /// プレイヤーの回転処理
    /// </summary>
    private void Turn()
    {
        if (!IsTurning)
            return;
        
        _transform.Rotate(rotateDirection * rotateSpeed);
    }

    /// <summary>
    /// プレイヤーのアニメーション処理
    /// </summary>
    private void Animate()
    {
        animator.SetBool("isRunning", IsRunning);
    }

    #endregion

}