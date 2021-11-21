using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageCtrl : MonoBehaviour
{
    [Header("クリアした時次のシーンへ行くか")] public bool goNextScene = true;
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;
    [Header("ゲームオーバー")] public GameObject gameOverObj;
    [Header("フェード")] public FadeImage fade;
    [Header("ゲームオーバー時に鳴らすSE")] public AudioClip gameOverSE;
    [Header("ステージクリアーSE")] public AudioClip stageClearSE;
    [Header("ステージクリア")] public GameObject stageClearObj;
    [Header("ステージクリア判定")] public PlayerTriggerOn stageClearTrigger;

    private Player p;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;
    private bool doClear = false;

    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && gameOverObj != null && stageClearObj != null)
        {
            gameOverObj.SetActive(false);
            stageClearObj.SetActive(false);
            playerObj.transform.position = continuePoint[0].transform.position;
            p = playerObj.GetComponent<Player>();
            if (p == null)
            {
                Debug.Log("プレイヤーが設定されていません");
                Destroy(this);
            }
        }
        else
        {
            Debug.Log("ステージコントローラーの設定が足りていません");
            Destroy(this);
        }
    }

    // Update is called once per frame 
    void Update()
    {
        //ゲームオーバー
        if (GManager.instance.isGameOver && !doGameOver)
        {
            gameOverObj.SetActive(true);
            GManager.instance.PlaySE(gameOverSE);
            doGameOver = true;       
        }
        //プレイヤーがダメージを受けた
        else if (p.IsDownAnimEnd() && !doGameOver)
        {
            PlayerSetContinuePoint();
        }
        //ステージを切り替える
        if (fade != null && startFade)
        {
            if (fade.IsFadeOutComplete())
            {
                if (nextStageNum < 4)
                {
                    GManager.instance.stageNum = nextStageNum;
                    SceneManager.LoadScene("Stage" + nextStageNum);
                }
                else
                {
                     SceneManager.LoadScene("Ending");
                }
            }
        }
        if (stageClearTrigger != null && stageClearTrigger.IsPlayerOn() && !doClear)
        {
            StageClear();
            doClear = true;
        }
        //リトライかタイトルに戻る
        if (doGameOver)
        {
            if (Input.GetButton("Submit"))
            {
                Retry();
            }
            if (Input.GetButton("Cancel"))
            {
                ReturnToTitle();
            }
        }
    }

    /// <summary> 
    /// プレイヤーをコンティニューポイントへ移動する 
    /// </summary> 
    public void PlayerSetContinuePoint()
    {
        playerObj.transform.position = continuePoint[GManager.instance.continueNum].transform.position;
        p.ContinuePlayer();
    }

    /// <summary> 
    ///プレイ中のステージからリトライ 
    /// </summary> 
    public void Retry()
    {
        GManager.instance.RetryGame();
        ChangeScene(GManager.instance.stageNum); //プレイ中のステージからリトライ 
    }

    public void ReturnToTitle()
    {
         SceneManager.LoadScene("Title"); //タイトルに戻る
    }

    /// <summary> 
    /// ステージを切り替えます。
    /// </summary> 
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num)
    {
        if (fade != null)
        {
            nextStageNum = num;
            fade.StartFadeOut();
            startFade = true;
        }
    }

    /// <summary>
    /// ステージをクリアした
    /// </summary>
    public void StageClear()
    {
        GManager.instance.isStageClear = true;
        stageClearObj.SetActive(true);
        GManager.instance.PlaySE(stageClearSE);
        if (goNextScene)
        {
            ChangeScene(GManager.instance.stageNum + 1);
        }
    }
}