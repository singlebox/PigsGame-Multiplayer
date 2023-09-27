using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour         //对游戏总体进行管理
{
    public static GameManager instance;

    public enum GameState
    {
        None,
        GamePause,
        GameStart,
        GameOver
    }

    public GameUi gameUI;

    public readonly SyncList<string> playerNames = new SyncList<string>();

    public GameState gameState = GameState.None;

    [HideInInspector]       //使字段不在面板展示
    public PlayerController localPlayer;        

    public static GameManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// 判断游戏是否结束
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        return playerNames.Count == 1;          //当游戏只剩一个人时 游戏结束
    }

    public void GameOver()
    {
        CmdGameOver();
    }

    private void Awake()
    {           //单例设计模式
        instance = this;
    }

    [Server]                //只执行在服务器端但是不能标识一些特殊函数
    public void StartGame()     //开始游戏进行倒计时
    {
        RpcStartGame();
    }

    [ClientRpc]             //服务器调用 客户端执行
    private void RpcStartGame()
    {
        gameUI.OnStartGame();
    }

    [Command(requiresAuthority =false)]
    private void CmdGameOver()
    {
        RpcShowWinner();
    }

    /// <summary>
    ///服务端同步获胜者信息 
    /// </summary>
    [ClientRpc]
    private void RpcShowWinner()
    {
        gameUI.OnShowWinner(playerNames[0]+playerNames[1]);
    }

    /// <summary>
    /// 角色加入游戏时把名字加入列表
    /// </summary>
    /// <param name="name"></param>
    public void AddPlayerName(string name)         
    {
        playerNames.Add(name);
        //foreach(var item in playerNames)
        //{
        //    Debug.Log(item);
        //}
        //Debug.Log("--------");
    }

    /// <summary>
    /// 角色死亡后移除角色名称
    /// </summary>
    /// <param name="name"></param>
    public void RemovePlayerName(string name)
    {
        playerNames.Remove(name);
        //foreach (var item in playerNames)
        //{
        //    Debug.Log(item);
        //}
        //Debug.Log("--------");
    }
}
