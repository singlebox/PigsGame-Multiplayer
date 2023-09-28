using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameMainMenuUi : MonoBehaviour     //主菜单
{
    [SerializeField] Button startHostBtn;
    [SerializeField] Button joinGameBtn;

    private void Start()        //两个按钮分别是创建主机和加入游戏
    {
        startHostBtn.onClick.AddListener(() => StartLobby());
        joinGameBtn.onClick.AddListener(() => JoinGame());
    }

    private void StartLobby() => NetworkManager.singleton.StartHost();
    private void JoinGame() => NetworkManager.singleton.StartClient();
}
