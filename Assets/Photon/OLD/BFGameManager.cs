//using System.Collections;

//using UnityEngine;
//using UnityEngine.UI;

//using Photon.Realtime;
//using Photon.Pun.UtilityScripts;
//using Photon.Pun;
//using Hashtable = ExitGames.Client.Photon.Hashtable;
//using Cinemachine;

//public class BFGameManager : MonoBehaviourPunCallbacks
//{
//    public static BFGameManager Instance = null;
//    public Text InfoText;
//    public GameObject spawn1;
//    public GameObject spawn2;
//    public GameObject enemies;
//    public CinemachineVirtualCamera virtualCam;

//    public void Awake()
//    {
//        Instance = this;
//    }

//    public override void OnEnable()
//    {
//        base.OnEnable();

//        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
//    }

//    // Start is called before the first frame update
//    public void Start()
//    {
//        Hashtable props = new Hashtable
//            {
//                {BFGame.PLAYER_LOADED_LEVEL, true}
//            };
//        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//    }

//    public override void OnDisable()
//    {
//        base.OnDisable();

//        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
//    }

//    #region PUN CALLBACKS

//    public override void OnDisconnected(DisconnectCause cause)
//    {
//        UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
//    }

//    public override void OnLeftRoom()
//    {
//        PhotonNetwork.Disconnect();
//    }

//    public override void OnMasterClientSwitched(Player newMasterClient)
//    {
//        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
//        {
//           // StartCoroutine(SpawnAsteroid());
//        }
//    }

//    public override void OnPlayerLeftRoom(Player otherPlayer)
//    {
//        CheckEndOfGame();
//    }

//    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
//    {
//        if (changedProps.ContainsKey(BFGame.PLAYER_LIVES))
//        {
//            CheckEndOfGame();
//            return;
//        }

//        if (!PhotonNetwork.IsMasterClient)
//        {
//            return;
//        }


//        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
//        int startTimestamp;
//        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

//        if (changedProps.ContainsKey(BFGame.PLAYER_LOADED_LEVEL))
//        {
//            if (CheckAllPlayerLoadedLevel())
//            {
//                if (!startTimeIsSet)
//                {
//                    CountdownTimer.SetStartTime();
//                }
//            }
//            else
//            {
//                // not all players loaded yet. wait:
//                Debug.Log("setting text waiting for players! ", this.InfoText);
//                InfoText.text = "Waiting for other players...";
//            }
//        }

//    }

//    #endregion

//    private bool CheckAllPlayerLoadedLevel()
//    {
//        foreach (Player p in PhotonNetwork.PlayerList)
//        {
//            object playerLoadedLevel;

//            if (p.CustomProperties.TryGetValue(BFGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
//            {
//                if ((bool)playerLoadedLevel)
//                {
//                    continue;
//                }
//            }

//            return false;
//        }

//        return true;
//    }
//    private void StartGame()
//    {
//        Debug.Log("StartGame!");

//        Vector3 position = new Vector3(24, 0.0f, 0);
//        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

//        GameObject player = PhotonNetwork.Instantiate("PlayerCharacter", position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)
//        if(player.GetPhotonView().Owner.ActorNumber == 1)
//        {
//            player.transform.position = spawn1.transform.position;
//        }
//        else if (player.GetPhotonView().Owner.ActorNumber == 2)
//        {
//            player.transform.position = spawn2.transform.position;
//        }


//        if (player.GetComponent<PhotonView>().IsMine)
//        {
//            virtualCam.Follow = player.transform;
//            virtualCam.LookAt = player.transform;
   
//        }
      
//        if (PhotonNetwork.IsMasterClient)
//        {
//            SpawnWeapons();
//        }
//    }
    
//    private void SpawnWeapons()
//    {
//        //Positions taken from the existing ghosts
//        Vector3[] positionArray = new[] { new Vector3(0, 1, 0),
//                                          new Vector3(0, 1, 9.5f),
//                                          new Vector3(0, 1, -9.5f) };




//        GameObject obj = PhotonNetwork.InstantiateRoomObject("PickupSMG", positionArray[0], Quaternion.identity);
//        obj = PhotonNetwork.InstantiateRoomObject("PickupSniper", positionArray[1], Quaternion.identity);
//        obj = PhotonNetwork.InstantiateRoomObject("PickupShotgun", positionArray[2], Quaternion.identity);


//    }
//    private void CheckEndOfGame()
//    {
//    }

//    private void OnCountdownTimerIsExpired()
//    {
//        StartGame();
//    }
//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
