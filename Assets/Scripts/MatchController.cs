using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class MatchController : MonoBehaviourPun
{
    public GameObject winPanel;
    private Dictionary<int, Photon.Realtime.Player> players = new Dictionary<int, Photon.Realtime.Player>();
    private bool isDead;
    private int deathCounts = 0;
    // Start is called before the first frame update
    void Start()
    {
        players = PhotonNetwork.CurrentRoom.Players;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = GetComponent<BattleScript>().isDead;

        foreach (Photon.Realtime.Player player in players.Values)
        {

            if (GetComponent<BattleScript>().isDead)
            {
                deathCounts++;
            }

        }

        if(!isDead) print("deathcounts for alive player " + deathCounts );


        if (deathCounts == PhotonNetwork.CurrentRoom.PlayerCount - 1 || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (photonView.IsMine && !isDead)
            {
                GetComponent<PlayerController>().enabled = false;
                winPanel.GetComponentInChildren<Text>().text = "You Won!";
                winPanel.SetActive(true);
                GameObject.Find("Close Button").SetActive(false);
                this.enabled = false;
            }
        }
    }
}
