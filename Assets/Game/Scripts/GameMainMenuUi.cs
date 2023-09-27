using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameMainMenuUi : MonoBehaviour
{
    [SerializeField] Button startHostBtn;
    [SerializeField] Button joinGameBtn;

    private void Start()
    {
        startHostBtn.onClick.AddListener(() => StartLobby());
        joinGameBtn.onClick.AddListener(() => JoinGame());
    }

    private void StartLobby() => NetworkManager.singleton.StartHost();
    private void JoinGame() => NetworkManager.singleton.StartClient();
}
