using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum items { hint, erase, pass, cardSteal, timeSteal, bind };

    public Player player;

    public bool gameOver;
    public int newDiceSide;
    public Text diceTimer;
    public Dice dice;
    public bool timerOn;
    

    public static int playerStartPoint = 0;

    public Canvas problemCanvas;

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
        //newDiseSide
        if(player.curIndex > playerStartPoint + newDiceSide)
        {
            player.movingAllowed = false;
            playerStartPoint = player.curIndex - 1;
            if (playerStartPoint == 6 || playerStartPoint == 14)
                player.transporting = true;
        }

    }

    public void RoundStart()
    {
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

    public int getPlayerNextPosition()
    {
        return player.curIndex+newDiceSide;
    }

    public void useItemCard(items itemName)
    {
        player.itemCards.Remove(itemName);
    }
}
