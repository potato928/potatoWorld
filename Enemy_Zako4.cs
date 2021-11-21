using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Zako4 : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("加算スコア")] public int myScore;
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("画面外でも行動する")] public bool nonVisibleAct;
    [Header("接触判定")] public EnemyCollisionCheck checkCollision;
    [Header("地面判定")] public GroundCheck enemyGroundCheck;
    [Header("やられた時に鳴らすSE")] public AudioClip deadSE;
    [Header("攻撃オブジェクト")] public GameObject attackObj;
    [Header("攻撃間隔")] public float interval;
    #endregion

    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private bool isDead = false;
    private float timer;
    private string deadPointTag = "DeadPoint";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<BoxCollider2D>();
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

    void FixedUpdate()
    {
        if (!oc.playerStepOn)
        {
            if (sr.isVisible || nonVisibleAct)
            {
                
                if (transform.eulerAngles.y == 0)
                {                
                    rb.velocity = new Vector2(-1 * speed, -gravity);
                    if (checkCollision.isOn || !enemyGroundCheck.IsGround())
                    {
                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        Attack();
                    }
                }
                else if (transform.eulerAngles.y == 180)
                {
                    rb.velocity = new Vector2(speed, -gravity);
                    if (checkCollision.isOn || !enemyGroundCheck.IsGround())
                    {
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        Attack();
                    }
                }                 
            }
            else
            {
                rb.Sleep();
            }
        }
        else
        {
            if (!isDead)
            {
                anim.Play("enemy3_dead");
                //rb.velocity = new Vector2(0, -gravity);
                rb.velocity = new Vector2(0, 0);
                isDead = true;
                col.enabled = false;
                if (GManager.instance != null)
                {
                    GManager.instance.PlaySE(deadSE);
                    GManager.instance.score += myScore;
                }
                Destroy(gameObject, 0.7f);
            }
            else
            {
                // transform.Rotate(new Vector3(0, 0, 5));
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == deadPointTag)
        {
            anim.Play("enemy1_dead");
            rb.velocity = new Vector2(0, 0);
            isDead = true;
            col.enabled = false;
            Destroy(gameObject, 0.7f);
        }
    }
}