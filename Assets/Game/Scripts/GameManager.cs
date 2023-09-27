using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour         //����Ϸ������й���
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

    [HideInInspector]       //ʹ�ֶβ������չʾ
    public PlayerController localPlayer;        

    public static GameManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// �ж���Ϸ�Ƿ����
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        return playerNames.Count == 1;          //����Ϸֻʣһ����ʱ ��Ϸ����
    }

    public void GameOver()
    {
        CmdGameOver();
    }

    private void Awake()
    {           //�������ģʽ
        instance = this;
    }

    [Server]                //ִֻ���ڷ������˵��ǲ��ܱ�ʶһЩ���⺯��
    public void StartGame()     //��ʼ��Ϸ���е���ʱ
    {
        RpcStartGame();
    }

    [ClientRpc]             //���������� �ͻ���ִ��
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
    ///�����ͬ����ʤ����Ϣ 
    /// </summary>
    [ClientRpc]
    private void RpcShowWinner()
    {
        gameUI.OnShowWinner(playerNames[0]+playerNames[1]);
    }

    /// <summary>
    /// ��ɫ������Ϸʱ�����ּ����б�
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
    /// ��ɫ�������Ƴ���ɫ����
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
