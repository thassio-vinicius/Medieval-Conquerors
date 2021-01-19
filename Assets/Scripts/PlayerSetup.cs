using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PlayerSetup : MonoBehaviourPun
{
    public Text playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerName();
        if (photonView.IsMine)
        {
            transform.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            transform.GetComponent<PlayerController>().enabled = false;
        }
    }

    void SetPlayerName()
    {
        if(playerNameText != null)
        {
            if(photonView.IsMine)
            { playerNameText.text = "YOU";
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = (string.IsNullOrEmpty(photonView.Owner.NickName)) ? "player" + photonView.Owner.UserId : photonView.Owner.NickName;
            }
        }
    }
}