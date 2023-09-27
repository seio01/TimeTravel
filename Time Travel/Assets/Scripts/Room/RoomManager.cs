using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    public Image[] playerListImg;
    public Image[] orderList;
    public bool isReady;

    public GameObject readyText;
    public GameObject infoText;
    public GameObject timeText;
    public GameObject setOrderPanel;
    public GameObject gameStartText;
    public GameObject forcedStartTimeText;
    public TMP_Text playerCountText;
    public TMP_Text pickCardText;

    public Button pickCardBtn;
    public Button leaveRoomBtn;


    public Sprite[] itemImg;
    public Image[] items;

    public List<int> itemList = new List<int>();

    public List<int> playerOrderList = new List<int>();

    public int readyCounts;
    public int playerIndexWithOrder; //순서 섞은 후 플레이어가 몇번째 순서인지

    public bool setTimer;

    public PhotonView PV;
    public forcedStartTimer timerScript;

    public Action quitEvent;
    public bool isApplicationQuit;
    public GameObject MiddleQuitPanel;

    public Button YesButton;
    public Button NoButton;
    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName);
        }
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
        InitializeApplicationQuit();
        PhotonNetwork.AutomaticallySyncScene = true;
        isReady = false;
        playerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerListImg[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("playerList" + i + PhotonNetwork.PlayerList[i].NickName);
            playerListImg[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[i].NickName;

        }
        //timer기능
        Timer();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerOrderList.Add(i);
        }

        PV.RPC("receieveReadyInformation", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
    }

    void Start()
    {
        YesButton.onClick.AddListener(gameQuit);
        //NoButton.onClick.AddListener(no);
    }

    public void Timer()
    {
        setTimer = true;
        timeText.SetActive(true);
        infoText.SetActive(true);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //same nickname
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (newPlayer.NickName == player.NickName && player != newPlayer)
            {
                Debug.Log("same nickname");
                return;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
                StartGame();
        }
        Debug.Log(newPlayer.NickName);

        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(playerListImg[i].GetComponentInChildren<TMP_Text>().text == "")
            {
                playerListImg[i].GetComponentInChildren<TMP_Text>().text = newPlayer.NickName;
                break;
            }
        }
        playerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //same nickname
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (otherPlayer.NickName == player.NickName && player != otherPlayer)
            {
                Debug.Log("leave same nickname");
                return;
            }
        }

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (playerListImg[i].GetComponentInChildren<TMP_Text>().text == otherPlayer.NickName)
            {
                playerListImg[i].GetComponentInChildren<TMP_Text>().text = "";
                if (playerListImg[i].transform.transform.Find("Ready Text").gameObject.activeSelf == true)
                {
                    readyCounts--;
                    playerListImg[i].transform.transform.Find("Ready Text").gameObject.SetActive(false);
                }
                break;
            }
        }
        playerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    public void PickItems()
    {
        SoundManager.instance.SoundPlayerStop();
        SoundManager.instance.SoundPlayer("ItemPick");
        setTimer = false;
        timeText.SetActive(false);
        pickCardBtn.interactable = false;
        isReady = true;
        leaveRoomBtn.interactable = false;
        pickCardText.text = "뽑은 카드 위로 마우스를 올리면 \n설명을 볼 수 있습니다.\n";
        int ran;
        //아이템 뽑기
        for(int i = 0; i < 3; i++)
        {
            ran = UnityEngine.Random.Range(0, 3);
            items[i].sprite = itemImg[ran];
            itemList.Add(ran); 
        }
        ran = UnityEngine.Random.Range(3, 6);
        items[3].sprite = itemImg[ran];
        itemList.Add(ran);

        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(true);
            Debug.Log(itemList[i]);
        }

        //아이템 뽑기 완료하면 자동 레디되게
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerListImg[i].GetComponent<playerPanel>().setReadyToOther(PhotonNetwork.LocalPlayer.NickName);
            if (playerListImg[i].GetComponentInChildren<TMP_Text>().text == PhotonNetwork.LocalPlayer.NickName)
            {
                playerListImg[i].transform.transform.Find("Ready Text").gameObject.SetActive(true);
            }
        }




        // 레디했는데 인원이 max인원보다 적을 경우 60초 기다림. 새로운 사람이 들어왔을 때는 갱신 x, ready했을 때만 갱신.
        //15초 이하 남기고 사람 들어왔을 때에는 갱신 없이 마지막 인원의 ready or 나가기 선택 기다리고 게임 시작.
        if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PV.RPC("checkTime", RpcTarget.AllViaServer);
        }
    }


    public List<int> ShuffleOrder(List<int> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            int ran = UnityEngine.Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[ran];
            list[ran] = temp;
        }

        return list;
    }

    public void UpdateListToOthers()
    {
        PV.RPC("UpdateList", RpcTarget.All, playerOrderList.ToArray());
    }

    [PunRPC]
    void UpdateList(int[] updatedList)
    {
        List<int> updatedPlayerList = new List<int>(updatedList);
        setOrderPanel.SetActive(true);
        SoundManager.instance.SoundPlayer("ShowPanel2");
        leaveRoomBtn.interactable = false;

        for (int i = 0; i < updatedPlayerList.Count; i++)
        {
            orderList[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + (i + 1) + ". " + PhotonNetwork.PlayerList[updatedPlayerList[i]].NickName;
            orderList[i].gameObject.SetActive(true);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < updatedList.Length; i++)
            {
                playerOrderList[i] = updatedList[i];
            }
        }
        DontDestroyObjects.instance.setPlayerListWithOrder(updatedPlayerList);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public void UpdateItemListToOthers()
    {
        setPlayerIndexWithOrder();
        
        PV.RPC("UpdateItemList", RpcTarget.AllBuffered, playerIndexWithOrder, itemList.ToArray() );
    }

    [PunRPC]
    void UpdateItemList(int playerIndexWithOrder, int[] updatedList)
    {
        DontDestroyObjects.instance.fillItemList(playerIndexWithOrder, updatedList);
    }

    IEnumerator StartGameRoutine()
    {
        forcedStartTimeText.SetActive(false);
        //순서 정하기
        if (RoomManager.instance.readyCounts != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            int diff = PhotonNetwork.CurrentRoom.MaxPlayers - RoomManager.instance.readyCounts;
            for (int i = 1; i <= diff; i++)
            {
                playerOrderList.RemoveAt(PhotonNetwork.CurrentRoom.MaxPlayers - i);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            playerOrderList = ShuffleOrder(playerOrderList);
            UpdateListToOthers();
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        yield return new WaitForSeconds(2f);
        //아이템 뽑은 것 다른 사람들에게 전송.
        UpdateItemListToOthers();
        gameStartText.SetActive(true);
        SoundManager.instance.SoundPlayer("ShowPanel");
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("loadNextScene", RpcTarget.All);
        }

        //일단은 모든 클라이언트에서 씬 로드 하는걸로 하는데 이걸 나중에 master에서만 로드하고 동기화할지 결정해야할듯

    }

    public void LeaveRoom()
    {
        SoundManager.instance.SoundPlayer("Button");
        SoundManager.instance.SoundPlayerStop();
        pickCardBtn.interactable = true;
        leaveRoomBtn.interactable = true;
        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(false);
        }

        Destroy(DontDestroyObjects.instance);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Room")
        {
            SceneManager.LoadScene("Main");
        }
    }

    void setPlayerIndexWithOrder()
    {
        List<Photon.Realtime.Player> playerListWithOrder = DontDestroyObjects.instance.playerListWithOrder;
        for (int i = 0; i <playerListWithOrder.Count ; i++)
        {
            if (playerListWithOrder[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                playerIndexWithOrder = i;
                break;
            }
        }
        
    }
    [PunRPC]
    void loadNextScene()
    {
        PhotonNetwork.LoadLevel("Loading");
    }

    [PunRPC]
    void updateTimer()
    {
        forcedStartTimeText.SetActive(false);
        forcedStartTimeText.SetActive(true);
    }

    [PunRPC]
    void checkTime()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        else
        {
            if (timerScript.time > 15)
            {
                PV.RPC("updateTimer", RpcTarget.AllViaServer);
            }
            else
            {
                PV.RPC("syncTimer", RpcTarget.Others, timerScript.time);
            }
        }
    }

    [PunRPC]
    void syncTimer(float masterClientTime)
    {
        forcedStartTimeText.SetActive(true);
        timerScript.time = masterClientTime;
    }

    [PunRPC]
    void receieveReadyInformation(string newPlayerName)
    {
        List<string> readyPlayerNames = new List<string>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (playerListImg[i].transform.GetChild(2).gameObject.activeSelf == true)
            {
                readyPlayerNames.Add(playerListImg[i].GetComponentInChildren<TMP_Text>().text);
            }
        }
        PV.RPC("setReady", RpcTarget.Others, newPlayerName, readyPlayerNames.ToArray());
    }

    [PunRPC]
    void setReady(string localPlayerNickName, string[] playerNames)
    {
        if (localPlayerNickName != PhotonNetwork.LocalPlayer.NickName)
        {
            return;
        }
        else
        {
            for (int i = 0; i < playerNames.Length; i++)
            {
                for (int j = 0; j < PhotonNetwork.CurrentRoom.MaxPlayers; j++)
                {
                    if (playerListImg[j].GetComponentInChildren<TMP_Text>().text == playerNames[i])
                    {
                        playerListImg[j].transform.transform.Find("Ready Text").gameObject.SetActive(true);
                        readyCounts++;
                        break;
                    }
                }
            }
        }
    }

    void gameQuit()
    {
        //test
        //최종 버전에서는 주석 해제하기.
        /*var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string bannedTime = ((long)timeSpan.TotalSeconds).ToString();
        PlayerPrefs.SetInt("isBanned", 1);
        PlayerPrefs.SetString("bannedTime", bannedTime);*/
        isApplicationQuit = true;
        Application.Quit();
    }

    void no()
    {
        isApplicationQuit = false;
        MiddleQuitPanel.SetActive(false);
    }

    void InitializeApplicationQuit()
    {
        quitEvent += () =>
        {
            GameObject.Find("endCanvas").transform.GetChild(2).gameObject.SetActive(true);
        };
        Application.wantsToQuit += ApplicationQuit;
    }

    bool ApplicationQuit()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Room")
        {
            if (RoomManager.instance.isReady == false)
            {
                RoomManager.instance.isApplicationQuit = true;
            }
            if (!RoomManager.instance.isApplicationQuit && RoomManager.instance.isReady == true)
            {
                quitEvent?.Invoke();
            }
            return RoomManager.instance.isApplicationQuit;
        }
        else if (scene.name == "SampleScene")
        {
            if (!quitInTheMiddle.instance.isApplicationQuit && !GameManager.instance.isOver)
            {
                quitEvent?.Invoke();
            }
            return quitInTheMiddle.instance.isApplicationQuit;
        }
        else
        {
            return true;
        }

    }
}
