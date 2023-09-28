using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyMenuUI : MonoBehaviour        //��������������
{
    [SerializeField] Button startGameBtn;
    [SerializeField] Button exitLobbyBtn;

    private void Start()
    {
        if(NetworkServer.active)            //�ж��Ƿ��������������Ļ���Start Host���Start Game
        {
            startGameBtn.gameObject.SetActive(true);
            startGameBtn.onClick.AddListener(() => StartGame());        //��Ӽ���
        }
        else
        {
            startGameBtn.gameObject.SetActive(false);       //�����������ؿ�ʼ��ť
        }

        exitLobbyBtn.onClick.AddListener(() => ExitLobby());        //��Ӽ����¼�
    }

    private void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Online Scene");     //�л�����
        //GameManager.GetInstance().StartGame();
    }

    private void ExitLobby()
    {
        if(NetworkServer.active&&NetworkClient.isConnected)     //������ֱ��ֹͣHost
        {
            NetworkManager.singleton.StopHost();

        }
        else
        {
            if(NetworkClient.isConnected)               //��������ֹͣ�ͻ�������
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}
