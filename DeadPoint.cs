using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//このスクリプトは使わない
public class DeadPoint : MonoBehaviour
{
    [Header("ステージコントローラー")] public StageCtrl ctrl;

    private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == playerTag)
        {
            if (GManager.instance != null && ctrl != null)
            {
                GManager.instance.SubHeartNum();
                if (!GManager.instance.isGameOver)
                {
                    ctrl.PlayerSetContinuePoint();
                }
            }
            else
            {
                Debug.Log("設定が足りません");
            }
        }
    }
}