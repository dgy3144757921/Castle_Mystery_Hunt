using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameEnding : MonoBehaviour
{
    //PlayerAttributes
    public PlayerAttributes playerAttributes;
    //游戏胜利图片淡入的时间
    public float fadeDuration = 1f;
    //图片显示时间
    public float displayDuration = 1f;
    //游戏角色对象
    public GameObject player;
    //画布背景
    public CanvasGroup exitBK;
    //失败背景
    public CanvasGroup failBK;
    //血条金币数背景
    public CanvasGroup gameBK;
    //Exit提示文字
    public TMP_Text exitText;
    //血条Slider
    public Slider healthSlider;
    //金币Text
    public TMP_Text coinText;
    //游戏胜利所需金币值
    public int exitMinCoin;
    //游戏胜利的bool值
    bool isExit;
    //游戏失败的bool值
    bool isFail;
    //用于图片淡入与显示的计时器
    float timer = 0f;
    //用于Exit文字淡入的计时器
    float timer1 = 0f;
    //音频组件
    public AudioSource winAudio;//胜利音频
    public AudioSource failAudio;//失败音频
    //判断游戏音频是否正在播放的bool值
    bool isPlay;

    // Start is called before the first frame update
    void Start()
    {
        isExit = false;
        isFail = false;
        isPlay = false;
        exitBK.alpha = 0f;
        exitText.alpha = 0f;
        gameBK.alpha = 1f;
        //初始化血条金币UI
        UpdateHp();
        UpdateCoin();
    }

    private void OnTriggerEnter(Collider other)//进入终点范围
    {
        if(other.CompareTag("Player"))//游戏角色碰到触发器
        {
            isExit = true;//判断游戏胜利
        }
    }

    private void OnTriggerExit(Collider other)//离开终点范围
    {
        if (other.CompareTag("Player"))//游戏角色碰到触发器
        {
            isExit = false;
        }
    }

    //失败函数
    public void Caught()
    {
        isFail = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExit)
        {
            EndLevel(exitBK,false,winAudio);
        }
        else if (isFail)
        {
            EndLevel(failBK,true,failAudio);
        }
    }

    public void UpdateHp()
    {
        healthSlider.value = playerAttributes.hp / playerAttributes.maxHp;
    }

    public void UpdateCoin()
    {
        coinText.text = playerAttributes.coin.ToString();
    }
    private void EndLevel(CanvasGroup imgcanvasGroup, bool doRestart ,AudioSource playAudio)
    {
        if (!doRestart && playerAttributes.coin >= exitMinCoin)//游戏胜利，退出游戏
        {
            gameBK.alpha = 0f;//隐藏血条金币
            timer += Time.deltaTime;//计时器开始计时
            imgcanvasGroup.alpha = timer / fadeDuration;//让图片的不透明度随计时器增加

            if (!isPlay)
            {
                playAudio.Play();
                isPlay = true;
            }

            if (timer > fadeDuration)
            {
                timer1 += Time.deltaTime;
                exitText.alpha = timer1 / fadeDuration;
                if (timer1 > fadeDuration && Input.GetKeyDown(KeyCode.Escape))
                {
                    Exit();
                }
            }
        }
        else if (doRestart)//游戏失败，重启游戏
        {
            gameBK.alpha = 0f;//隐藏血条金币
            timer += Time.deltaTime;//计时器开始计时
            imgcanvasGroup.alpha = timer / fadeDuration;//让图片的不透明度随计时器增加

            if (!isPlay)
            {
                playAudio.Play();
                isPlay = true;
            }

            if (timer > fadeDuration + displayDuration)
            {
                SceneManager.LoadScene("Main");//重新加载游戏场景
            }
        }
    }

    private void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
