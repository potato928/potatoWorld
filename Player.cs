using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプする長さ")] public float jumpLimitTime;
    [Header("接地判定")] public GroundCheck ground;
    [Header("天井判定")] public GroundCheck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    [Header("踏みつけ判定の高さの割合(%)")] public float stepOnRate;
    [Header("ジャンプする時に鳴らすSE")] public AudioClip jumpSE;
    [Header("やられた鳴らすSE")] public AudioClip downSE;
    [Header("コンティニュー時に鳴らすSE")] public AudioClip continueSE;
    [Header("無敵時間")] public float invincible;
    [Header("ステージコントローラー")] public StageCtrl ctrl;
    #endregion

    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDown = false;
    private bool isOtherJump = false;
    private bool isClearMotion = false;
    private bool isSquat = false;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float otherJumpSpeed = 0.0f;
    private float dashTime, jumpTime;
    private float beforeKey;
    private string enemyTag = "Enemy";
    private string moveFloorTag = "MoveFloor";
    private string jumpStepTag = "JumpStep";
    private string enemyAttackTag = "EnemyAttack";
    private string deadPointTag = "DeadPoint";
    private bool isContinue = false;
    private float continueTime, blinkTime;
    private SpriteRenderer sr = null;
    private MoveObject moveObj;
    #endregion

    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        GManager.instance.isStageClear = false;
        ContinuePlayer();
    }

    private void Update()
    {
        if (isContinue)
        {
            //明滅　ついている時に戻る
            if (blinkTime > 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えているとき
            else if (blinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅　ついているとき
            else
            {
                sr.enabled = true;
            }
            //指定時間たったら明滅終わり
            if (continueTime > invincible)
            {
                isContinue = false;
                blinkTime = 0f;
                continueTime = 0f;
                sr.enabled = true;
            }
            else
            {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDown && !GManager.instance.isGameOver && !GManager.instance.isStageClear)
        {
            //接地判定を得る
            isGround = ground.IsGround();
            isHead = head.IsGround();

            //各種座標軸の速度を求める
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();
            //しゃがみを行う
            GetSquat();

            //アニメーションを適用
            SetAnimation();

            //移動速度を設定
            Vector2 addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                addVelocity = moveObj.GetVelocity();
            }
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
        }
        else
        {
            if (!isClearMotion && GManager.instance.isStageClear)
            {
                anim.Play("player_clear");
                isClearMotion = true;
            }
            rb.velocity = new Vector2(0, -gravity);
        }
    }


    /// <summary>  
    /// しゃがみ動作実装のための関数 
    /// </summary>  
    /// <returns>Y軸の速さ</returns>  
    private void GetSquat()
    {
        float verticalKey = Input.GetAxis("Vertical");
        if (verticalKey < 0)
         {
                isSquat = true;
         }
        else
         {
                isSquat = false;
         }

    }
        /// <summary>  
        /// Y成分で必要な計算をし、速度を返す。  
        /// </summary>  
        /// <returns>Y軸の速さ</returns>  
        private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        //何かを踏んだ際のジャンプ
        if (isOtherJump)
        {
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (canHeight && canTime && !isHead)
            {
                //ySpeed = jumpSpeed;
                ySpeed = otherJumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }
        //地面にいるとき
        else if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
                GManager.instance.PlaySE(jumpSE);
            }
            else
            {
                isJump = false;
                isOtherJump = false;
            }
        }
        //ジャンプ中
        else if (isJump)
        {
            //上ボタンを押されている。かつ、現在の高さがジャンプした位置から自分の決めた位置より下ならジャンプを継続する
            if (verticalKey > 0 && jumpPos + jumpHeight > transform.position.y && jumpTime < jumpLimitTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }
        if (isJump || isOtherJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        return ySpeed;
    }

    /// <summary>  
    /// X成分で必要な計算をし、速度を返す。  
    /// </summary>  
    /// <returns>X軸の速さ</returns>  
    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;
        if (horizontalKey > 0 && !isSquat)
        {
            //transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0 && !isSquat)
        {
            //transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            isRun = false;
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }
        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }
        beforeKey = horizontalKey;
        xSpeed *= dashCurve.Evaluate(dashTime);
        beforeKey = horizontalKey;
        return xSpeed;
    }

    /// <summary>  
    /// アニメーションを設定する  
    /// </summary>  
    private void SetAnimation()
    {
        anim.SetBool("jump", isJump || isOtherJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);
        anim.SetBool("squat",isSquat);
    }

    /// <summary>  
    /// ダウンアニメーションが終わっているかどうか  
    /// </summary>  
    /// <returns>終了しているかどうか</returns>   
    public bool IsDownAnimEnd()
    {
        if (isDown && anim != null)
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("player_down"))
            {
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>  
    /// コンティニューする  
    /// </summary>  
    public void ContinuePlayer()
    {
        isDown = false;
        isSquat = false;
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        anim.Play("player_stand");     
        continueTime = 0.0f;
        GManager.instance.PlaySE(continueSE);
    }

    #region//接触判定  
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GManager.instance.isStageClear && !GManager.instance.isGameOver)
        {
            //敵
            if (collision.collider.tag == enemyTag)
            {
                //踏みつけ判定になる高さ
                float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
                //踏みつけ判定のワールド座標
                float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

                foreach (ContactPoint2D p in collision.contacts)
                {
                    if (p.point.y < judgePos)
                    {
                        ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                        if (o != null)
                        {
                            jumpPos = transform.position.y; //ジャンプした位置を記録する
                            otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                            otherJumpSpeed = o.jumpSpeed;       //踏んづけたものから跳ねる速さを取得する
                            o.playerStepOn = true;        //踏んづけたものに対して踏んづけた事を通知する
                            isOtherJump = true;
                            isJump = false;
                            jumpTime = 0.0f;
                        }
                        else
                        {
                            Debug.Log("ObjectCollisionが付いてないよ!");
                        }
                    }
                    else if(!isDown && !isContinue)
                    {
                        anim.Play("player_down");
                        isDown = true;
                        GManager.instance.SubHeartNum();
                        GManager.instance.PlaySE(downSE);
                        break;
                    }
                }
            }
             //ジャンプ台
            else if (collision.collider.tag == jumpStepTag)
            {
                //踏みつけ判定になる高さ
                float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
                //踏みつけ判定のワールド座標
                float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;
                foreach (ContactPoint2D p in collision.contacts)
                {
                    if (p.point.y < judgePos)
                    {
                        ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                        if (o != null)
                        {
                            jumpPos = transform.position.y; //ジャンプした位置を記録する
                            otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                            otherJumpSpeed = o.jumpSpeed;       //踏んづけたものから跳ねる速さを取得する
                            o.playerStepOn = true;        //踏んづけたものに対して踏んづけた事を通知する
                            isOtherJump = true;
                            isJump = false;
                            jumpTime = 0.0f;
                        }
                        else
                        {
                            Debug.Log("ObjectCollisionが付いてないよ!");
                        }
                    }
                }
            }
            //動く床
            else if (collision.collider.tag == moveFloorTag)
            {
                //踏みつけ判定になる高さ
                float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
                //踏みつけ判定のワールド座標
                float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

                foreach (ContactPoint2D p in collision.contacts)
                {
                    //動く床に乗っている
                    if (p.point.y < judgePos)
                    {
                        moveObj = collision.gameObject.GetComponent<MoveObject>();
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == moveFloorTag)
        {
            //動く床から離れた
            moveObj = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GManager.instance.isStageClear && !GManager.instance.isGameOver)
        {
            //敵の攻撃
            if (collision.tag == enemyAttackTag &&  !isDown && !isContinue)
            {
                anim.Play("player_down");
                isDown = true;
                GManager.instance.SubHeartNum();
                GManager.instance.PlaySE(downSE);
            }
            //DeadPoint
            if (collision.tag == deadPointTag)
            {
                if (!isDown)
                {
                    GManager.instance.SubHeartNum();
                }
                if (!GManager.instance.isGameOver)
                {
                    ctrl.PlayerSetContinuePoint();
                }
            }
        }
    }
    #endregion
}