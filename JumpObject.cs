using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JumpObject : MonoBehaviour
{
    private ObjectCollision oc;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        oc = GetComponent<ObjectCollision>();
        anim = GetComponent<Animator>();
        if (oc == null || anim == null)
        {
            Debug.Log("ジャンプ台の設定が足りていません");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oc.playerStepOn)
        {
            //anim.SetTrigger("on");
            anim.Play("jump_step");
            oc.playerStepOn = false;
        }
    }
}