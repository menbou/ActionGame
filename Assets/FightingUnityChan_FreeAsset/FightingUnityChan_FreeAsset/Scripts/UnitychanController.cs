using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class UnitychanController : MonoBehaviour
{

    const int DefaultLife = 3;
    const float StunDuration = 1.5f;

    int life = DefaultLife;
    float recoverTime = 0.0f;
    Vector3 collisionNormal = Vector3.zero;

    //重力等の変更ができるようにパブリック変数とする
    public float gravity;
    public float speed;
    public float jumpSpeed;
    public float rotateSpeed;

    //外部から値が変わらないようにPrivateで定義
    private CharacterController characterController;
    private Animator animator;

    //左手のコライダー
    private Collider handCollider;
    //右足のコライダー
    private Collider footCollider;

    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //左手のコライダーを取得
        parent = GameObject.Find("Character1_LeftForeArm");
        //child = GameObject.Find("Character1_LeftHand");
        handCollider = parent.transform.Find("Character1_LeftHand").GetComponent<SphereCollider>();

        parent = GameObject.Find("Character1_RightFoot");
        footCollider = parent.transform.Find("Character1_RightToeBase").GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection;

        moveDirection = Vector3.zero;

        if (IsStan())
        {
            recoverTime -= Time.deltaTime;


            transform.rotation = Quaternion.LookRotation(-collisionNormal);
            Debug.Log(collisionNormal);
        }
        else
        {
            //rayを使用した接地判定
            if (CheckGrounded() == true)
            {
                moveDirection = CalcMovDirection();
            }
            else
            {
                //後で実装　ジャンプ中の移動量を設定する必要がある
            }
        }

        OperatorCharacter(moveDirection);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        handCollider.enabled = stateInfo.IsName("Jab");
        footCollider.enabled = stateInfo.IsName("Hikick") || stateInfo.IsName("Spinkick") || stateInfo.IsName("ScrewKick");


        ////Aを押すとjab
        ////if (Input.GetKeyDown(KeyCode.A))
        ////{
        ////    animator.SetBool("Jab", true);
        ////}

        ////Sを押すとHikick
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    animator.SetBool("Hikick", true);
        //}

        ////Dを押すとSpinkick
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    animator.SetBool("Spinkick", true);
        //}
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Run.ScrewKick"))
        {
            animator.SetBool("ScrewKick", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Run.Headspring"))
        {
            animator.SetBool("DamageDown", false);
        }

    }


    private void OperatorCharacter(Vector3 moveDirection)
    {

        Vector3 moveDirectionXZ = moveDirection;
        moveDirectionXZ.y = 0.0f;


        if (moveDirectionXZ != Vector3.zero)
        {
            ////回転角度
            //var targetAngle = Vector3.Angle(transform.forward, moveDirectionXZ);

            //if (Vector3.Cross(transform.forward, moveDirectionXZ).z < 0)
            //{
            //    targetAngle *= -1;
            //}
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


        ////キャラクターコントローラ使う場合
        //characterController.Move(moveDirection * Time.deltaTime);

        //Transformを使う場合
        moveDirection *= Time.deltaTime;
        this.gameObject.transform.Translate(moveDirection, Space.World);

        //var targetRotation = Quaternion.Euler(gameObject.transform.localEulerAngles);

        //速度が０以上の時、Runを実行する
        animator.SetBool("Run", moveDirectionXZ.magnitude > 0.0f);


        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    }

    private Vector3 CalcMovDirection()
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
        //moveDirection.y -= gravity * Time.deltaTime;

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


    public void OnClick_Maru()
    {
        if (IsStan()) return;
        animator.SetBool("Jab", true);
    }

    public void OnClick_Sankaku()
    {
        if (IsStan()) return;
        animator.SetBool("Spinkick", true);
    }

    public void OnClick_Shikaku()
    {
        if (IsStan()) return;
        animator.SetBool("Hikick", true);
    }

    public void OnClick_Batu()
    {
        if (IsStan()) return;
        animator.SetBool("ScrewKick", true);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    //攻撃した相手がEnemyの場合
    //    if (other.CompareTag("Enemy"))
    //    {
    //        life--;
    //        recoverTime = StunDuration;

    //        animator.SetBool("DamageDown", true);

    //    }
    //}

    void OnCollisionEnter(Collision collision)
    {
        //衝突した点
        ContactPoint point = collision.contacts[0];

        //攻撃した相手がEnemyの場合
        if (collision.gameObject.tag == "Enemy")
        {
            collisionNormal = point.normal;

            recoverTime = StunDuration;

            collisionNormal.y = 0.0f;

            //transform.rotation = Quaternion.LookRotation(-collisionNormal);

            animator.SetBool("DamageDown", true);

            //OperatorCharacter(transform.TransformPoint(-collisionNormal));
            //OperatorCharacter(transform.TransformPoint(-collisionNormal));
        }
    }

    public int Life()
    {
        return life;
    }

    public bool IsStan()
    {
        return recoverTime > 0.0f || life <= 0;
    }

}
