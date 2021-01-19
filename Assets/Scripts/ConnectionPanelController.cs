using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ConnectionPanelController : MonoBehaviourPunCallbacks
{
    public Text connectionStatusText;
    public GameObject informText;
    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    [SerializeField] double timer = 10f;
    ExitGames.Client.Photon.Hashtable roomProps;

    // Start is called before the first frame update
    void Start()
    {
        informText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            connectionStatusText.text = PhotonNetwork.NetworkClientState.ToString();
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = GameManager.instance.nickname;

        }

        if (startTimer)
        {
            if(timerIncrementValue >= timer / 2) connectionStatusText.text = "STARTING MATCH IN " + (timer - timerIncrementValue).ToString(".00");

            timerIncrementValue = PhotonNetwork.Time - startTime;
            if (timerIncrementValue >= timer)
            {
                connectionStatusText.text = "STARTING MATCH";

                //SceneManager.LoadScene("DefaultLevel");
            StartCoroutine(LoadLevelScene(2f));
            }

        }
    }

    public override void OnConnected()
    {

        PhotonNetwork.LocalPlayer.NickName = GameManager.instance.nickname;

        ExitGames.Client.Photon.Hashtable playerSelectionProp =
        new ExitGames.Client.Photon.Hashtable { { CustomProps.CHARACTER_SELECTION_NAME, GameManager.instance.characterSelected } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");

        PhotonNetwork.JoinRandomRoom();

    }

    #region UI Callback Methods

    public void OnQuitMatchButtonClicked()
    {

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();

        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }


    }

    #endregion

    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        connectionStatusText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playersCount < 4)
        {
            int currentCount = 4 - playersCount;
            connectionStatusText.text = "JOINED " 
            + PhotonNetwork.CurrentRoom.Name + "\nWAITING FOR " 
            + currentCount + (currentCount == 1 ? " MORE PLAYER." : " MORE PLAYERS...");
        }
        else
        {
            connectionStatusText.text = "STARTING MATCH";

            StartCoroutine(LoadLevelScene(2f));
        }

    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;

        int currentCount = 4 - playersCount;
            connectionStatusText.text = "JOINED " 
            + PhotonNetwork.CurrentRoom.Name + "\nWAITING FOR " 
            + currentCount + (currentCount == 1 ? " MORE PLAYER." : " MORE PLAYERS...");

        informText.SetActive(true);

        informText.GetComponent<Text>().text = newPlayer.NickName + " JUST JOINED!";

        StartCoroutine(DeactivateAfterSeconds(informText, 2f));

        if (PhotonNetwork.IsMasterClient && playersCount >= 2)
        {
            roomProps = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            roomProps.Add(CustomProps.START_TIMER, startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }


        if (PhotonNetwork.CurrentRoom.PlayerCount == 4) StartCoroutine(LoadLevelScene(2f));

    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        object timer;
        if (propertiesThatChanged.TryGetValue(CustomProps.START_TIMER, out timer))
        {
            startTime = double.Parse(timer.ToString());
            startTimer = true;
        }
    }

    #endregion

    #region Private Methods
    void CreateAndJoinRoom()
    {
        string randomRoomName = "ROOM" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        //creating the room
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        obj.SetActive(false);
    }

    IEnumerator LoadLevelScene(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        SceneManager.LoadScene("DefaultLevel");
    }
    #endregion
}
