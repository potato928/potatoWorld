using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNum : MonoBehaviour
{
    private Text stageText;
    private int oldStageNum;

    void Start()
    {
        stageText = GetComponent<Text>();
        if (stageText != null && GManager.instance != null)
        {
            stageText.text = "Stage " + GManager.instance.stageNum.ToString();
        }
    }

    void Update()
    {
        if (stageText != null && GManager.instance != null)
        {
            if (oldStageNum != GManager.instance.stageNum)
            {
                stageText.text = "Stage " + GManager.instance.stageNum.ToString();
                oldStageNum = GManager.instance.stageNum;
            }
        }
    }
}