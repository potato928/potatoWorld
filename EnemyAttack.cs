using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAttack : MonoBehaviour
{
    [Header("スピード")] public float speed = 3.0f;
    [Header("最大移動距離")] public float maxDistance = 100.0f;
    [Header("画面外でも行動する")] public bool nonVisibleAct;
    [Header("飛んでいく方向,1左,2右,3上,4下")] public int Direction=1;

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
        switch (Direction) 
        { 
            case 1:
                rb.transform.Rotate(0, 0, 90);
                 break;
            case 2:
                rb.transform.Rotate(0, 0, 270);
                break;
            case 3:
                rb.transform.Rotate(0, 0, 0);
                break;
            case 4:
                rb.transform.Rotate(0, 0, 180);
                 break;
            default:
                Debug.Log("設定が足りません");
                Destroy(this.gameObject);
                break;
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
                switch (Direction)
                {
                    case 1:
                        rb.MovePosition(transform.position += Vector3.left * Time.deltaTime * speed);
                        break;
                    case 2:
                        rb.MovePosition(transform.position += Vector3.right * Time.deltaTime * speed);
                        break;
                    case 3:
                        rb.MovePosition(transform.position += Vector3.up * Time.deltaTime * speed);
                        break;
                    case 4:
                        rb.MovePosition(transform.position += Vector3.down * Time.deltaTime * speed);
                        break;
                }

            }

        }
        else
        {
            rb.Sleep();
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