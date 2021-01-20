using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPositions;
    public GameObject Environment;
    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #region Photon Callback Methods

    void OnEvent(EventData onEvent)
    {
        if (onEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])onEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            string receivedPlayerSelectionData = (string)data[3];

            //GameObject playerSelected = GameManager.instance.characters.Find(a => a.name.ToLower() == receivedPlayerSelectionData.ToLower());

            //GameObject player = Instantiate(playerSelected, receivedPosition + battleArena.transform.position, receivedRotation);
            GameObject player = PhotonNetwork.Instantiate(receivedPlayerSelectionData, receivedPosition + Environment.transform.position, receivedRotation);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.ViewID = (int)data[2];
        }
    }

    #endregion

    #region Private Methods
    void SpawnPlayer()
    {

        object playerSelected;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProps.CHARACTER_SELECTION_NAME, out playerSelected))
        {
            string temp = playerSelected as string;

            int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);

            Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;

            GameObject selected = GameManager.instance.characters.Find(a => a.name.ToLower() == temp.ToLower());

            //GameObject player = Instantiate(selected, instantiatePosition, Quaternion.identity);
            GameObject player = PhotonNetwork.Instantiate(temp, instantiatePosition, Quaternion.identity);


            PhotonView photonView = player.GetComponent<PhotonView>();

            
                if(photonView.Owner.IsMasterClient)
                {
                    if (PhotonNetwork.AllocateRoomViewID(photonView))
                {
                    object[] data = new object[]
                    {
                    //player.transform.position - battleArena.transform.position, player.transform.rotation, photonView.ViewID, playerSelected
                    player.transform.position - Environment.transform.position, player.transform.rotation, photonView.ViewID, playerSelected

                    };

                    RaiseEventOptions raise = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others,
                        CachingOption = EventCaching.AddToRoomCache
                    };

                    SendOptions options = new SendOptions
                    {
                        Reliability = true
                    };

                    //Raise events
                    PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raise, options);
                }
                else
                {
                    Debug.Log("Failed to allocate a viewID");
                    //Destroy(player);
                }
        }
                }
        else
        {
            Debug.Log("cant spawn player");

        }

    }
    #endregion
}
