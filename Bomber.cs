using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    [Header("加算スコア")] public int myScore;
    [Header("移動経路")] public GameObject[] movePoint;
    [Header("速さ")] public float speed = 1.0f;
    [Header("画面外でも行動する")] public bool nonVisibleAct;
    [Header("やられた時に鳴らすSE")] public AudioClip deadSE;
    [Header("攻撃オブジェクト")] public GameObject attackObj;
    [Header("攻撃間隔")] public float interval;
    [Header("飛行機の向きを固定する")] public bool FixDirection;

    private Rigidbody2D rb;
    private int nowPoint = 0;
    private bool returnPoint = false;
    private Vector2 oldPos = Vector2.zero;
    private Vector2 myVelocity = Vector2.zero;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private CapsuleCollider2D capcol = null;
    private SpriteRenderer sr = null;
    private bool isDead = false;
    private float timer;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        oc = GetComponent<ObjectCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        capcol = GetComponent<CapsuleCollider2D>();
        if (movePoint != null && movePoint.Length > 0 && rb != null)
        {
            rb.position = movePoint[0].transform.position;
            oldPos = rb.position;
        }
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

    public Vector2 GetVelocity()
    {
        return myVelocity;
    }

    private void FixedUpdate()
    {
        if (movePoint != null && movePoint.Length > 1 && rb != null)
        {
            if (!oc.playerStepOn)
            {
                if (sr.isVisible || nonVisibleAct)
                {
                    //通常進行
                    if (!returnPoint)
                    {
                        int nextPoint = nowPoint + 1;

                        //目標ポイントとの誤差がわずかになるまで移動
                        if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                        {
                            //現在地から次のポイントへのベクトルを作成
                            Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                            //飛行機の向き
                            direction();

                            //次のポイントへ移動
                            rb.MovePosition(toVector);

                            Attack();

                        }
                        //次のポイントを１つ進める
                        else
                        {
                            rb.MovePosition(movePoint[nextPoint].transform.position);
                            ++nowPoint;
                            //現在地が配列の最後だった場合
                            if (nowPoint + 1 >= movePoint.Length)
                            {
                                returnPoint = true;
                            }
                        }
                    }
                    //折返し進行
                    else
                    {
                        int nextPoint = nowPoint - 1;

                        //目標ポイントとの誤差がわずかになるまで移動
                        if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                        {

                            //現在地から次のポイントへのベクトルを作成
                            Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                            //飛行機の向き
                            direction();

                            //次のポイントへ移動
                            rb.MovePosition(toVector);

                            Attack();

                        }
                        //次のポイントを１つ戻す
                        else
                        {
                            rb.MovePosition(movePoint[nextPoint].transform.position);
                            --nowPoint;
                            //現在地が配列の最初だった場合
                            if (nowPoint <= 0)
                            {
                                returnPoint = false;
                            }
                        }
                    }
                    myVelocity = (rb.position - oldPos) / Time.deltaTime;
                    oldPos = rb.position;
                }
            }
            else
            {
                if (!isDead)
                {

                    anim.Play("bomber_dead");
                    //rb.velocity = new Vector2(0, -gravity);
                    rb.velocity = new Vector2(0, 0);
                    isDead = true;
                    capcol.enabled = false;
                    if (GManager.instance != null)
                    {
                        GManager.instance.PlaySE(deadSE);
                        GManager.instance.score += myScore;
                    }
                    Destroy(gameObject, 0.7f);
                }
                else
                {
                    //transform.Rotate(new Vector3(0, 0, 5));
                }
            }
            
        }
    }
    private void direction()
    {
        if (!FixDirection)
        { 
            if (myVelocity.x < 0.0f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (myVelocity.x > 0.0f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
    private void Attack()
    {
        if (timer > interval && interval > 0)
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
            g.transform.parent = null;
            g.SetActive(true);
            timer = 0.0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}