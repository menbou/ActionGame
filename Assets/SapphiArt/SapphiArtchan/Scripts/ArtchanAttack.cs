using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtchanAttack : MonoBehaviour
{

    //PlayerのAnimatorコンポーネント保存用
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerのAnimatorコンポーネントを取得する
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        ////qを押すとjab
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    animator.setbool("jab", true);
        //}
    }
}
