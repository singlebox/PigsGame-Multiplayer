using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameBtn;
    [SerializeField] Button exitLobbyBtn;

    private void Start()
    {
        if(NetworkServer.active)
        {
            startGameBtn.gameObject.SetActive(true);
            startGameBtn.onClick.AddListener(() => StartGame());
        }
        else
        {
            startGameBtn.gameObject.SetActive(false);
        }

        exitLobbyBtn.onClick.AddListener(() => ExitLobby());
    }

    private void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Online Scene");
        //GameManager.GetInstance().StartGame();
    }

    private void ExitLobby()
    {
        if(NetworkServer.active&&NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();

        }
        else
        {
            if(NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}
