using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_Bullet : MonoBehaviour
{
    [Header("スピード")] public float speed = 3.0f;
    [Header("最大移動距離")] public float maxDistance = 100.0f;
    [Header("画面外でも行動する")] public bool nonVisibleAct;
 //   [Header("飛んでいく方向,1左,2右,3上,4下")] public int Direction = 1;

    private Rigidbody2D rb;
    private SpriteRenderer sr = null;
    private Vector3 defaultPos;
    private string groundTag = "Ground";
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (rb == null)
        {
            Debug.Log("設定が足りません");
            Destroy(this.gameObject);
        }
        defaultPos = transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = Vector3.Distance(transform.position, defaultPos);
        if (sr.isVisible || nonVisibleAct)
        {
            //最大移動距離を超えている
            if (d > maxDistance)
            {
                Destroy(this.gameObject);
            }
            else
            {

                Vector3 directionnew = new Vector3(
                Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad),
                0
                 );
                rb.MovePosition(transform.position += directionnew * Time.deltaTime * speed);
            }
        }
        else
        {
            //rb.Sleep();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            speed = 0.0f;
            anim.Play("fire_explosion");
        }
    }
    public void DestroyExplosion()
    {
        Destroy(this.gameObject);
    }
}