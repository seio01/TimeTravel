using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class problem : MonoBehaviour
{
    public Button selection1;
    public Button selection2;
    public Button selection3;
    public Button selection4;
    public Button hintButton;
    public TMP_Text TimeText;
    public TMP_Text dynastyText;
    public GameObject problemImage;
    public GameObject resultPanel;
    public GameObject hintPanel;
    public GameObject problemPassButton;
    public GameObject selectionEraseButton;
    List<Dictionary<string, object>> problemData;
    List<Dictionary<string, object>> answerData;

    int problemID;
    int prevDynasty;
    string dynasty;
    string problemType;
    string isHaveHint;
    string hintString;
    TMP_Text resultText;

    problemGraph problemScript;

    int playerPosition;
    bool isPlayerCorrect;

    bool isOtherPlayerUseTimeSteal;

    public PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        problemData = CSVReader.Read("문제");
        answerData = CSVReader.Read("답");
        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        problemScript = this.gameObject.GetComponent<problemGraph>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        getPlayerNextPosition();
        Debug.Log(playerPosition);
        setProblemID();
        Debug.Log("문제 번호: " + problemID);
        getInfoFromCSV();
        setImage();
        controlButtons();
        if (problemType == "ox")
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 8);
            }
            else
            {
                StartCoroutine("setTimer", 15);
            }
        }
        else
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 15);
            }
            else
            {
                StartCoroutine("setTimer", 30);
            }
        }
    }

    void OnDisable()
    {
        hintPanel.SetActive(false);
        GameManager.instance.isThisTurnTimeSteal = false;
        if (isPlayerCorrect == true)
        {
            GameManager.instance.MovePlayer();
            //추가
            GameManager.instance.correctCount++;
            GameManager.instance.UpdateGaugeImg();

        }
        else
            GameManager.instance.finishRound = true;
    }

    void setProblemID()
    {
        Debug.Log(playerPosition);
            if (playerPosition >= 1 && playerPosition <= 8)
            {
                problemID = Random.Range(1, 31);
                prevDynasty = 0;
            }
            else if (playerPosition >= 9 && playerPosition <= 20)
            {
                problemID = Random.Range(1, 66) + 30;
                prevDynasty = 30;
            }
            else if (playerPosition >= 21 && playerPosition <= 40)
            {
                problemID = Random.Range(1, 81) + 95;
                prevDynasty = 95;
            }
            else if (playerPosition >= 41 && playerPosition <= 70)
            {
                problemID = Random.Range(1, 110) + 175;
                prevDynasty = 175;
            }
            else
            {
                //problemID = Random.Range(1, 근현대 문제 수) + 95+고려시대문제 수+조선시대 문제 수;
                //prevDynasty = 95+고려시대 문제 수+조선시대 문제 수;
            }
    }

    IEnumerator setTimer(int time)
    {
        while (time >= 0)
        {
            TimeText.text = "남은 시간: " + time.ToString() + "초";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
        }
        resultText.text = "틀렸습니다...";
        isPlayerCorrect = false;
        resultPanel.SetActive(true);
    }

    void getInfoFromCSV()
    {
        dynasty = problemData[problemID - 1]["시대"].ToString();
        problemType = problemData[problemID - 1]["유형"].ToString();
        isHaveHint = problemData[problemID - 1]["힌트 여부"].ToString();
        hintString = problemData[problemID - 1]["힌트"].ToString();
        dynastyText.text = dynasty;
    }

    void setImage()
    {
        string graphName = dynasty + problemID;
        Sprite[] dynastyImageGraph = null;
        switch (dynasty)
        {
            case "고조선":
                dynastyImageGraph = problemScript.dynasty1;
                break;
            case "삼국시대":
                dynastyImageGraph = problemScript.dynasty2;
                break;
            case "고려":
                dynastyImageGraph = problemScript.dynasty3;
                break;
            case "조선시대":
                dynastyImageGraph = problemScript.dynasty4;
                break;
            case "근대이후":
                dynastyImageGraph = problemScript.dynasty5;
                break;
            default: break;
        }
        Sprite sprite = dynastyImageGraph[problemID - 1 - prevDynasty];
        problemImage.GetComponent<Image>().sprite = sprite;
        problemImage.GetComponent<Image>().SetNativeSize();
    }

    void controlButtons()
    {
        selection1.gameObject.SetActive(true);
        selection2.gameObject.SetActive(true);
        if (problemType == "ox")
        {
            selection3.gameObject.SetActive(false);
            selection4.gameObject.SetActive(false);
            TMP_Text selectionText = selection1.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "o";
            selectionText = selection2.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "x";
        }
        else
        {
            selection3.gameObject.SetActive(true);
            selection4.gameObject.SetActive(true);
            setSelectionText(selection1, "선택지1");
            setSelectionText(selection2, "선택지2");
            setSelectionText(selection3, "선택지3");
            setSelectionText(selection4, "선택지4");
        }
        if (isHaveHint == "o")
        {
            hintButton.gameObject.SetActive(true);
        }
        else
        {
            hintButton.gameObject.SetActive(false);
        }
        showPassButton();
        showSelectionEraseButton();
    }

    void setSelectionText(Button button, string selectionName)
    {
        TMP_Text selectionText = button.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = answerData[problemID - 1][selectionName].ToString();
    }

    public void selectAnswer(int selectionNum)
    {
        StopCoroutine("setTimer");
        resultPanel.SetActive(true);
        int correctAnswer = int.Parse(answerData[problemID - 1]["답"].ToString());
        if (selectionNum == correctAnswer)
        {
            resultText.text = "정답입니다!";
            isPlayerCorrect = true;
        }
        else
        {
            resultText.text = "틀렸습니다...";
            isPlayerCorrect = false;
        }
    }

    public void showPassButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.pass))
        {
            problemPassButton.SetActive(true);
        }
        else
        {
            problemPassButton.gameObject.SetActive(false);
        }
    }

    public void showSelectionEraseButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.erase) && problemType == "4지선다")
        {
            selectionEraseButton.SetActive(true);
        }
        else
        {
            selectionEraseButton.gameObject.SetActive(false);
        }
    }

    public void showHint()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.hint))
        {
            TMP_Text hintText = hintPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            hintText.text = hintString;
            hintPanel.SetActive(true);
            RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.hint);
        }
    }

    public void eraseWrongSelection()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.erase);
        int correctAnswer = int.Parse(answerData[problemID - 1]["답"].ToString());
        int eraseSelection;
        while (true)
        {
            eraseSelection = Random.Range(1, 5);
            if (eraseSelection != correctAnswer)
            {
                break;
            }
        }
        if (eraseSelection == 1)
        {
            selection1.gameObject.SetActive(false);
        }
        else if (eraseSelection == 2)
        {
            selection2.gameObject.SetActive(false);
        }
        else if (eraseSelection == 3)
        {
            selection3.gameObject.SetActive(false);
        }
        else
        {
            selection4.gameObject.SetActive(false);
        }
    }

    public void passProblem()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.pass);
        StopCoroutine("setTimer");
        resultPanel.SetActive(true);
        resultText.text = "문제를 패스했습니다. \n";
        string correctAnswer = answerData[problemID - 1]["답"].ToString();
        if (problemType == "ox")
        {
            if (correctAnswer == "1")
            {
                resultText.text += "정답은 o 였습니다.";
            }
            else
            {
                resultText.text += "정답은 x 였습니다.";
            }
        }
        else
        {
            resultText.text += "정답은 " + correctAnswer + "번이었습니다.";
        }
        isPlayerCorrect = true;
    }

    public void getPlayerNextPosition()
    {
        playerPosition = GameManager.instance.getPlayerNextPosition();
    }
}