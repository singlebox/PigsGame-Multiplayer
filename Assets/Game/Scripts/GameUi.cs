using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUi : MonoBehaviour             //对UI进行管理
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
    /// 开始游戏时执行
    /// </summary>
    public void OnStartGame()
    {
        countDownUI.StartCountDown();
    }

    /// <summary>
    /// 结束游戏时执行
    /// </summary>
    public void OnShowWinner(string winnerName)
    {
        whoWinUI.SetNamePlayerWinner(winnerName);
    }
}
