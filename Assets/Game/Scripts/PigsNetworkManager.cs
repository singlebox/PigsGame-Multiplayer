using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PigsNetworkManager : NetworkManager
{
    public List<PlayerController> players = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)          //����Ҽ��뵽��Ϸʹ�����������
    {                   //�л�����֮����ٵ���һ��
        base.OnServerAddPlayer(conn);

        if (!SceneManager.GetActiveScene().name.Equals("Online Scene")) return;    //�ڴ���������Ϊû��GameUi����ᵼ��ִ�г���
        PlayerController player = conn.identity.GetComponent<PlayerController>();  //��ȡ������Ϸ�����
        players.Add(player);

        player.SetNickName("Player " + players.Count);          //�޸��������

        GameManager.GetInstance().AddPlayerName(player.GetNickName());      //��������Ƽ��뵽�б���

        if(players.Count>=2)            //����Ҵ��ڵ���2ʱ���Գɹ�������Ϸ
        {
            GameManager.GetInstance().StartGame();
        }
    }
}
