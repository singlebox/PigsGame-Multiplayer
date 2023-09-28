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
        while (GameManager.GetInstance() == null || GameManager.GetInstance().localPlayer == null)      //检测是否有GameManager单例对象以及GameManager对象是否有PlayerController对象
            yield return null;      //暂停一帧
    }

    /// <summary>
    /// 开始游戏时执行 进行倒计时
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
