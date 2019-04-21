using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    //PlayerのAnimatorコンポーネント保存用
    private Animator animator;

    //左手のコライダー
    private Collider handCollider;
    //右足のコライダー
    private Collider footCollider;

    GameObject parent, child;

    // Use this for initialization
    void Start()
    {
        //PlayerのAnimatorコンポーネントを取得する
        animator = GetComponent<Animator>();

        //左手のコライダーを取得
        parent = GameObject.Find("Character1_LeftForeArm");
        child = GameObject.Find("Character1_LeftHand");
        handCollider = parent.transform.Find("Character1_LeftHand").GetComponent<SphereCollider>();

        parent = GameObject.Find("Character1_RightFoot");
        footCollider = parent.transform.Find("Character1_RightToeBase").GetComponent<SphereCollider>();

        //handCollider = GameObject.Find("Character1_LeftHand").GetComponent<SphereCollider>();
        //右足のコライダーを取得
        //footCollider = GameObject.Find("Character1_RightToeBase").GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //	if (handCollider == null) {
        //		handCollider = GameObject.Find("Character1_LeftHand").GetComponent<SphereCollider>();
        //	}
        handCollider.enabled = stateInfo.IsName("Jab");
        footCollider.enabled = stateInfo.IsName("Hikick") || stateInfo.IsName("Spinkick");


        //Aを押すとjab
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("Jab", true);
        }

        //Sを押すとHikick
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("Hikick", true);
        }

        //Dを押すとSpinkick
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("Spinkick", true);
        }
    }
}
