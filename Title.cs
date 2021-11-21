using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Title : MonoBehaviour
{
    [Header("フェード")] public FadeImage fade;
    [Header("ゲームスタート時に鳴らすSE")] public AudioClip startSE;

    private bool firstPush = false;
    private bool goNextScene = false;

    private void Start()
    {
        GManager.instance.InitializeGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Submit") && fade.IsFadeInComplete())
        {
            PressStart();
        }
        if(!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("stage1");
            goNextScene = true;
        }
    }

    public void PressStart()
    {
        Debug.Log("Press Start!");
        if (!firstPush)
        {
            if (fade != null)
            {
                GManager.instance.PlaySE(startSE);
                fade.StartFadeOut();
                firstPush = true;
            }
        }
    }
}