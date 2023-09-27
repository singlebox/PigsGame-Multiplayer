using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverMenuUI : MonoBehaviour
{
    public TextMeshProUGUI namePlayerWinnerTmp;     //œ‘ æ §¿˚’ﬂ

    public void SetNamePlayerWinner(string name)
    {
        GameManager.GetInstance().gameState = GameManager.GameState.GameOver;
        namePlayerWinnerTmp.text ="Winner "+name;
        gameObject.SetActive(true);
    }

}
