using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCountDownMenuUI : MonoBehaviour            //������е���ʱ
{
    public TextMeshProUGUI textMeshProUGUI;

    public void StartCountDown()
    {
        StartCoroutine(StartCountDownRoutine());
    }
    
    IEnumerator StartCountDownRoutine()
    {
        UpdateText("Loading...");
        yield return new WaitForSeconds(0.5f);      //Э�̵ȴ�1s�����
        UpdateText("3");
        yield return new WaitForSeconds(1f);      
        UpdateText("2");
        yield return new WaitForSeconds(1f);      
        UpdateText("1");
        yield return new WaitForSeconds(1f);      
        UpdateText("Fight");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        GameManager.GetInstance().gameState = GameManager.GameState.GameStart;
    }

    public void UpdateText(string text)
    {
        textMeshProUGUI.text = text;
    }
}
