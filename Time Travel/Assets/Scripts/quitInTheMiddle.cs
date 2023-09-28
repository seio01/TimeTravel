using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System;
using UnityEngine.UI;

public class quitInTheMiddle : MonoBehaviourPunCallbacks
{
    public static quitInTheMiddle instance;
    public TMP_Text testTMP;

    public List<int> outPlayerIndex;
    public Dice diceScript;
    public problem problemScript;
    public GameObject endGameText;
    public GameObject endPanel;

    public GameObject MiddleQuitPanel;
    public Button YesButton;
    public Button NoButton;

    public Action quitEvent;
    public bool isApplicationQuit;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        outPlayerIndex = new List<int>();
        InitializeApplicationQuit();
    }
    void Start()
    {
        YesButton.onClick.AddListener(gameQuit);
        NoButton.onClick.AddListener(no);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationQuit();
        }*/
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //게임 승리했을때 한사람 나가는 경우는 제외
            if (GameManager.instance.isOver)
                return;
            stopCoroutinesAndSetActiveFalse();
            GameManager.instance.isOver = true;
            for (int i = 0; i < DontDestroyObjects.instance.playerListWithOrder.Count; i++)
            {
                if (otherPlayer.NickName == DontDestroyObjects.instance.playerListWithOrder[i].NickName)
                {
                    GameManager.instance.playerInformationUIs[i].SetActive(false);
                    Color c = new Color(100 / 255f, 100 / 255f, 100 / 255f);
                    GameManager.instance.player[i].gameObject.GetComponent<SpriteRenderer>().color = c;
                    break;
                }
            }
            //남은 플레이어 닉네임
            endPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[0].NickName + " 님\n승리를 축하합니다!";
            StartCoroutine(endGame());
            return;
        }
        if (otherPlayer == GameManager.instance.controlPlayer)
        {
            int index = GameManager.instance.controlPlayerIndexWithOrder;
            GameManager.instance.playerInformationUIs[index].SetActive(false);
            Color c= new Color(100 / 255f, 100 / 255f, 100 / 255f);
            GameManager.instance.player[index].gameObject.GetComponent<SpriteRenderer>().color = c;
            if (GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].movingAllowed == true)
            {
                outPlayerIndex.Add(index);
            }
            else
            {
                StartCoroutine(UISmallerRoutine(index));
                outPlayerIndex.Add(index);
                changeControlPlayerIndex();
                stopCoroutinesAndSetActiveFalse();
                GameManager.instance.Invoke("RoundStart", 1);
            }

        }
        else
        {
            for (int i = 0; i < DontDestroyObjects.instance.playerListWithOrder.Count; i++)
            {
                if (otherPlayer.NickName == DontDestroyObjects.instance.playerListWithOrder[i].NickName)
                {
                    outPlayerIndex.Add(i);
                    GameManager.instance.playerInformationUIs[i].SetActive(false);
                    Color c = new Color(100 / 255f, 100 / 255f, 100 / 255f);
                    GameManager.instance.player[i].gameObject.GetComponent<SpriteRenderer>().color = c;
                    break;
                }
            }

        }
    }

    IEnumerator UISmallerRoutine(int index)
    {
        float time = 0f;
        bool smaller = true;
        while (smaller)
        {
            GameManager.instance.playerInformationUIs[index].transform.localScale = Vector3.one * (1.3f - time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                smaller = false;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator endGame()
    {
        SoundManager.instance.SoundPlayer("ShowPanel1");
        endGameText.transform.GetChild(0).GetComponent<TMP_Text>().text = "모든 플레이어가 게임을 중단하여\n자동으로 종료됩니다.\n";
        endGameText.SetActive(true);

        yield return new WaitForSeconds(2f);

        endGameText.SetActive(false);
        endPanel.SetActive(true);
        SoundManager.instance.SoundPlayer("Finish");
    }

    public void changeControlPlayerIndex()
    {
        GameManager.instance.controlPlayerIndexWithOrder++;
        if (GameManager.instance.controlPlayerIndexWithOrder == GameManager.instance.initialPlayerNum)
        {
            GameManager.instance.controlPlayerIndexWithOrder = 0;
        }
        GameManager.instance.controlPlayer = DontDestroyObjects.instance.playerListWithOrder[GameManager.instance.controlPlayerIndexWithOrder];
    }

    void stopCoroutinesAndSetActiveFalse()
    {
        diceScript.StopAllCoroutines();
        problemScript.StopAllCoroutines();
        GameManager.instance.problemCanvas.gameObject.SetActive(false);
        GameManager.instance.StopAllCoroutines();
        GameManager.instance.startRoundPanel.SetActive(false);
        GameManager.instance.itemUsePanel.SetActive(false);
        GameManager.instance.itemUseResultPanel.SetActive(false);
        GameManager.instance.space.SetActive(false);
        RpcManager.instance.diceImg.SetActive(false);
        RpcManager.instance.diceTimer.gameObject.SetActive(false);
    }

    void gameQuit()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string bannedTime = ((long)timeSpan.TotalSeconds).ToString();
        PlayerPrefs.SetInt("isBanned", 1);
        PlayerPrefs.SetString("banType", "game");
        PlayerPrefs.SetString("bannedTime", bannedTime);
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
            MiddleQuitPanel.SetActive(true);
        };

        Application.wantsToQuit += ApplicationQuit;
    }

    //프로그램 종료
    bool ApplicationQuit()
    {
        if (!isApplicationQuit && !GameManager.instance.isOver)
        {
            quitEvent?.Invoke();
        }

        return isApplicationQuit;
    }
}
