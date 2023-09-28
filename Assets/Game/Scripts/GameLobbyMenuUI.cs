using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyMenuUI : MonoBehaviour        //大厅场景管理类
{
    [SerializeField] Button startGameBtn;
    [SerializeField] Button exitLobbyBtn;

    private void Start()
    {
        if(NetworkServer.active)            //判断是否是主机，主机的话将Start Host变成Start Game
        {
            startGameBtn.gameObject.SetActive(true);
            startGameBtn.onClick.AddListener(() => StartGame());        //添加监听
        }
        else
        {
            startGameBtn.gameObject.SetActive(false);       //不是主机隐藏开始按钮
        }

        exitLobbyBtn.onClick.AddListener(() => ExitLobby());        //添加监听事件
    }

    private void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Online Scene");     //切换场景
        //GameManager.GetInstance().StartGame();
    }

    private void ExitLobby()
    {
        if(NetworkServer.active&&NetworkClient.isConnected)     //是主机直接停止Host
        {
            NetworkManager.singleton.StopHost();

        }
        else
        {
            if(NetworkClient.isConnected)               //不是主机停止客户端连接
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}
