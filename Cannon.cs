using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("攻撃オブジェクト")] public GameObject attackObj;
    [Header("攻撃間隔")] public float interval;

   //private Animator anim;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        if (attackObj == null)
        {
            Debug.Log("設定が足りません");
            Destroy(this.gameObject);
        }
        else
        {
            attackObj.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
             if (timer > interval)
            {
                //anim.SetTrigger("attack");
                //anim.Play("attack");
                Attack();
                timer = 0.0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
    }
    private void Attack()
    {
        GameObject g = Instantiate(attackObj);
        g.transform.SetParent(transform);
        g.transform.position = attackObj.transform.position;
        if (attackObj.transform.eulerAngles.y == 0)
        {
            g.transform.rotation = attackObj.transform.rotation;
        }
        else if (attackObj.transform.eulerAngles.y == 180)
        {
            g.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -attackObj.transform.eulerAngles.z);
        }
        g.SetActive(true);
    }
}
