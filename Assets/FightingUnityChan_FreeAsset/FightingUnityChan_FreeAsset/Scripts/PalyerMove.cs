using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PalyerMove : MonoBehaviour
{
    //重力等の変更ができるようにパブリック変数とする
    public float gravity;
    public float speed;
    public float jumpSpeed;
    public float rotateSpeed;

    //外部から値が変わらないようにPrivateで定義
    private CharacterController characterController;
    private Animator animator;
    //private Vector3 moveDirection = Vector3.zero;


    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //rayを使用した接地判定
        if (CheckGrounded() == true)
        {
            var moveDirection = CalcMovDirection();
            Debug.Log(moveDirection);
            OperatorCharacter(moveDirection);
            
        }
    }

    private void OperatorCharacter(Vector3 moveDirection)
    {
         
        Vector3 moveDirectionXZ = moveDirection;
        moveDirectionXZ.y = 0.0f;

        if (moveDirectionXZ != Vector3.zero)
        {
            //回転角度
            var targetAngle = Vector3.Angle(transform.forward, moveDirectionXZ);
            
            if(Vector3.Cross(transform.forward, moveDirectionXZ).z < 0)
            {
                targetAngle *= -1;
            }
            //Debug.Log(moveDirectionXZ + " " + targetAngle + " " + transform.rotation + " " + transform.forward + " " + Quaternion.Euler(0, targetAngle, 0));
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), 1.0f);
            transform.rotation = Quaternion.LookRotation(moveDirectionXZ);
        }


        // if (targetAngle < Angle_th)
        // {
        //移動の実行
        //   Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //   characterController.Move(globalDirection * Time.deltaTime);
        // }


        //Vector3 globalDirection = transform.TransformDirection(moveDirection);
        
        characterController.Move(moveDirection * Time.deltaTime);


        //var targetRotation = Quaternion.Euler(gameObject.transform.localEulerAngles);

        //速度が０以上の時、Runを実行する
        animator.SetBool("Run", moveDirectionXZ.magnitude > 0.0f);


        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    }

    private Vector3 CalcMovDirection( )
    {
        Vector3 moveDirection = Vector3.zero;

        //前進処理
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.z = speed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.z = -speed;
        }
        else
        {
            moveDirection.z = 0;
        }

        //方向転換
        //方向キーのどちらも押されている時
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            //向きを変えない
        }
        //左方向キーが押されている時
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.Rotate(0, rotateSpeed * -1, 0);
            moveDirection.x = -speed;
        }
        //右方向キーが押されている時
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //transform.Rotate(0, rotateSpeed, 0);
            moveDirection.x = speed;
        }

        // Stick
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        moveDirection = new Vector3(horizontal, 0.0f, vertical) * speed;

        ////jump
        ////if (Input.GetKeyDown(KeyCode.Space))
        //if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Run.Jump"))
        //{
        //    moveDirection.y = jumpSpeed;
        //}
        //重力を発生させる
        moveDirection.y -= gravity * Time.deltaTime;

        //回転
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 90, 0);
        }

        return moveDirection;

    }

    //rayを使用した接地判定メソッド
    public bool CheckGrounded()
    {

        //初期位置と向き
        var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        //rayの探索範囲
        var tolerance = 0.3f;

        //rayのHit判定
        //第一引数：飛ばすRay
        //第二引数：Rayの最大距離
        return Physics.Raycast(ray, tolerance);
    }
}
