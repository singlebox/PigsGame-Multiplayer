using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCountDownMenuUI : MonoBehaviour            //负责进行倒计时
{
    public TextMeshProUGUI textMeshProUGUI;

    public void StartCountDown()
    {       //开启倒计时协程
        StartCoroutine(StartCountDownRoutine());
    }
    
    IEnumerator StartCountDownRoutine()
    {
        UpdateText("Loading...");
        yield return new WaitForSeconds(0.5f);     
        UpdateText("3");
        yield return new WaitForSeconds(1f);       //协程等待1s后继续
        UpdateText("2");
        yield return new WaitForSeconds(1f);      
        UpdateText("1");
        yield return new WaitForSeconds(1f);      
        UpdateText("Fight");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        GameManager.GetInstance().gameState = GameManager.GameState.GameStart;
    }

    public void UpdateText(string text)     //改变文字
    {
        textMeshProUGUI.text = text;
    }
}
