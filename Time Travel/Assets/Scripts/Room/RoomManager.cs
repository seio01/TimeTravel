using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Image[] playerList;
    public Image[] orderList;
    public GameObject readyText;
    public GameObject infoText;
    public GameObject timeText;
    public GameObject setOrderPanel;

    public Button pickCardBtn;

    public Sprite[] itemImg;
    public Image[] items;

    public List<int> itemList = new List<int>();
    public List<int> playerOrderList = new List<int>();

    public int readyCounts;
    public int localPlayerIndex;

    public bool setTimer;

    public PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        //readyCounts = 1;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName);
        }
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerList[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("playerList" + i + PhotonNetwork.PlayerList[i].NickName);
            playerList[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[i].NickName;
        }

        //timer기능
        Timer();
        
    }

    public void Timer()
    {
        setTimer = true;
        timeText.SetActive(true);
        infoText.SetActive(true);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
                StartGame();
        }
        Debug.Log(newPlayer.NickName);

        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(playerList[i].GetComponentInChildren<TMP_Text>().text == "")
            {
                playerList[i].GetComponentInChildren<TMP_Text>().text = newPlayer.NickName;
                break;
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (playerList[i].GetComponentInChildren<TMP_Text>().text == otherPlayer.NickName)
            {
                playerList[i].GetComponentInChildren<TMP_Text>().text = "";
                break;
            }
        }
    }

    public void PickItems()
    {
        setTimer = false;
        pickCardBtn.interactable = false;
        int ran;
        //아이템 뽑기
        for(int i = 0; i < 3; i++)
        {
            ran = Random.Range(0, 3);
            items[i].sprite = itemImg[ran];
            itemList.Add(ran); //item 인덱스 저장 -->각자 개인의 list로 저장해야하는데...
        }
        ran = Random.Range(3, 6);
        items[3].sprite = itemImg[ran];
        itemList.Add(ran);

        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(true);
            Debug.Log(itemList[i]);
        }

        Invoke("ChangeToReady", 1f);
        
    }

    public void ChangeToReady()
    {
        //아이템 뽑기 완료하면 자동 레디되게
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                localPlayerIndex = i;
                break;
            }
        }
        playerList[localPlayerIndex].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        playerList[localPlayerIndex].GetComponent<playerPanel>().setReadyToOther(localPlayerIndex);
        //leaveButton.interactable = false;
    }

    public List<int> ShuffleOrder(List<int> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            int ran = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[ran];
            list[ran] = temp;
        }

        return list;
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;


        PhotonNetwork.CurrentRoom.IsOpen = false;
        //순서 정하기
        setOrderPanel.SetActive(true);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerOrderList.Add(i);
        }

        playerOrderList = ShuffleOrder(playerOrderList);
        setOrderPanel.GetComponent<PlayerOrderPanel>().UpdateListToOthers(playerOrderList);
        
        


        //PhotonNetwork.LoadLevel("Loading");
    }

    public void LeaveRoom()
    {
        pickCardBtn.interactable = true;
        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Main");
    }

}
