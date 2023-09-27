using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUi : MonoBehaviour             //��UI���й���
{
    public GameObject hudUI;
    public GameCountDownMenuUI countDownUI;
    public GameOverMenuUI whoWinUI;

    IEnumerator Start()
    {
        while (GameManager.GetInstance() == null || GameManager.GetInstance().localPlayer == null)
            yield return null;
    }

    /// <summary>
    /// ��ʼ��Ϸʱִ��
    /// </summary>
    public void OnStartGame()
    {
        countDownUI.StartCountDown();
    }

    /// <summary>
    /// ������Ϸʱִ��
    /// </summary>
    public void OnShowWinner(string winnerName)
    {
        whoWinUI.SetNamePlayerWinner(winnerName);
    }
}
