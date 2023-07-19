using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum items { hint, erase, pass, cardSteal, timeSteal, bind };
    public static int playerStartPoint = 0;

    public Player player;
    public int newDiceSide;
    public bool timerOn;
    public bool isLadder;
    public bool isTransport;
    public bool finishRound;
    public string spaceCategory;

    [Header("UI")]
    public Canvas problemCanvas;
    public Dice dice;
    public Space spaceAction;
    public Text diceTimer;
    public Text spaceText;
    public GameObject space;
    public GameObject diceImg;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        RoundStart();
    }

    // Update is called once per frame
    void Update()
    {
        //when to stop moving
        if(player.curIndex > playerStartPoint + newDiceSide)
        {
            player.movingAllowed = false;
            playerStartPoint = player.curIndex - 1;
            if (isLadder && !player.movingAllowed)
                player.moveLadder = true;
            else if (isTransport && !player.movingAllowed)
                player.Transport();
            else
                finishRound = true;
        }

        if (finishRound)
            Invoke("RoundStart", 1);
    }

    public void RoundStart()
    {
        player.moveLadder = false;
        finishRound = false;
        diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
        timerOn = true;
    }
    public void MovePlayer()
    {
        player.movingAllowed = true;
    }
    public void showProblem()
    {
        problemCanvas.gameObject.SetActive(true);
    }

    //추가
    public int getPlayerNextPosition()
    {
        return player.curIndex + newDiceSide;
    }

    public void useItemCard(items itemName)
    {
        player.itemCards.Remove(itemName);
    }

    public void CheckCurPoint(int diceNum)
    {
        switch (diceNum)
        {
            //test
            case 5:
            case 10:
            
            //test
            case 3:
            case 8:
            case 11:
            case 12:
            case 19:
            case 24:
            case 28:
            case 34:
            case 42:
            case 43:
            case 49:
            case 50:
            case 55:
            case 58:
            case 70:
            case 74:
            case 78:
            case 82:
            case 91:
                spaceCategory = "Nothing";
                spaceText.text = "현재 칸은 빈 칸입니다.\n 바로 이동가능합니다.";
                break;
            case 7:
            case 22:
            case 53:
            case 64:
            case 76:
                spaceCategory = "Ladder";
                spaceText.text = "현재 칸은 사다리 칸입니다.\n 사다리를 타고 이동합니다.";
                break;
            case 15:
            case 30:
            case 26:
            case 38:
            case 32:
            case 80:
            case 46:
            case 67:
            case 62:
            case 90:
                spaceCategory = "Portal";
                spaceText.text = "현재 칸은 포털 칸입니다.\n 같은 색깔의 포털로 이동합니다.";
                break;
            default:
                spaceCategory = "Problem";
                spaceText.text = "현재 칸은 문제 칸입니다.\n 문제를 맞추면 이동가능합니다.";
                break;
        }
        space.SetActive(true);

        StartCoroutine(DoActionRoutine());
        
    }

    IEnumerator DoActionRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        spaceAction.DoAction(spaceCategory);
    }

}
