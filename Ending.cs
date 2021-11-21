using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [Header("フェード")] public FadeImage fade;
    [Header("タイトル画面に戻るときに鳴らすSE")] public AudioClip startSE;

    private bool firstPush = false;
    private bool goNextScene = false;

    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButton("Submit") || Input.GetButton("Cancel")) && fade.IsFadeInComplete())
        {
            ReturnToTitle();
        }
        if (!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("Title");
            goNextScene = true;
        }
    }

    public void ReturnToTitle()
    {
        Debug.Log("Return to Title!");
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
