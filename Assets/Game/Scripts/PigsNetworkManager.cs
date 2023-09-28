using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PigsNetworkManager : NetworkManager
{
    public List<PlayerController> players = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)          //当玩家加入到游戏使调用这个方法
    {                   //切换场景之后会再调用一次
        base.OnServerAddPlayer(conn);

        if (!SceneManager.GetActiveScene().name.Equals("Online Scene")) return;    //在大厅界面因为没有GameUi对象会导致执行出错
        PlayerController player = conn.identity.GetComponent<PlayerController>();  //获取加入游戏的玩家
        players.Add(player);

        player.SetNickName("Player " + players.Count);          //修改玩家名称

        GameManager.GetInstance().AddPlayerName(player.GetNickName());      //将玩家名称加入到列表中

        if(players.Count>=2)            //当玩家大于等于2时可以成功开启游戏
        {
            GameManager.GetInstance().StartGame();
        }
    }
}
