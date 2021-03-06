﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerTPMove : MonoBehaviour
{

    public float WalkSpeed = 2.0f;
    public float RunSpeed = 4.0f;
    public float jumpVelocity = 3.0f;
    public Transform virtualCamera;//与角色等距不变的一个虚拟GameObject记录摄像头位置

    private Animator PlayerAnimator;
    private Camera maincamera;
    private float horizonCrossInput;
    private float vertiCrossInput;
    private Rigidbody playerRigidbody;
    private bool isGrounded;
    private CapsuleCollider capsuleCollider;
    private float MoveSpeed;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        PlayerAnimator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerHealth = GetComponent<PlayerHealth>();
        maincamera = Camera.main;
        MoveSpeed = WalkSpeed;

    }

    void CrossMove(float h_cInput,float v_cInput)
    {
        maincamera.transform.position =new Vector3(maincamera.transform.position.x, virtualCamera.transform.position.y, maincamera.transform.position.z);
        
        if (h_cInput != 0.0f || v_cInput != 0.0f)
        {
            if (Input.GetKey(KeyCode.LeftShift)&&playerHealth.MPSlider.value>=5)
            {
                MoveSpeed = RunSpeed;
                PlayerAnimator.SetBool("isRun", true);
                playerHealth.TakePlayerMP(0.1f);
            }
            else
            {
                MoveSpeed = WalkSpeed;
                PlayerAnimator.SetBool("isRun", false);
            }
            //Debug.Log("horizonInput:" + h_cInput+"====="+"vertical"+ v_cInput);
            transform.rotation = maincamera.transform.rotation;//当有输入时，使得角色的旋转与摄像机旋转保持一致
            maincamera.transform.position = virtualCamera.transform.position;//当角色移动时，保证角色与摄像机距离一致
            PlayerAnimator.SetBool("isMove", true);
            if (v_cInput > 0)
            {
                transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime * v_cInput);
            }
            else if (v_cInput < 0)
            {
                transform.Translate(Vector3.forward * MoveSpeed / 2 * Time.deltaTime * v_cInput);
            }

            if (h_cInput < 0)
            {
                transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime * h_cInput);
            }
            else if (h_cInput > 0)
            {
                transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime * h_cInput);
            }
        }
        else
        {
            PlayerAnimator.SetBool("isMove", false);
        }
        
    }

    /*void JumpForce()
    {
        playerRigidbody.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
        playerHealth.TakePlayerMP(10.0f);
    }*/

    void PlayerJump(bool isGround)
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && playerHealth.MPSlider.value >= 10.0f)
        {
            PlayerAnimator.SetBool("isJump", true);
            //Invoke("JumpForce", 0.3f);
            playerRigidbody.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            playerHealth.TakePlayerMP(10.0f);
        }
        else
        {
            PlayerAnimator.SetBool("isJump", false);
        }
    }

    void CheckGround()
    {
        RaycastHit hitInfo;
        float shellOffset = 0.01f;
        float groundCheckDistance = 0.01f;
        Vector3 currentPos = transform.position;
        currentPos.y += capsuleCollider.height / 2f;
        if (Physics.SphereCast(currentPos, capsuleCollider.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
            ((capsuleCollider.height / 2f) - capsuleCollider.radius) + groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    void Update()
    {
        horizonCrossInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        vertiCrossInput = CrossPlatformInputManager.GetAxisRaw("Vertical");
        CrossMove(horizonCrossInput, vertiCrossInput);
        PlayerJump(isGrounded);
        //if (jumpTimer > jumpInterval)
        //{
        //    PlayerJump(isGrounded);
        //   jumpTimer = 0.0f;
        //}
        //jumpTimer += Time.deltaTime;

        //playerRigidbody.AddForce(-Vector3.up*3, ForceMode.VelocityChange);
    }
    private void FixedUpdate()
    {
        CheckGround();
        if (isGrounded == false)
            PlayerAnimator.SetBool("isJump", false);
    }
}