using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class DiceTimer : MonoBehaviour
{
    public GameManager manager;
    public Text diceTimeText;
    public Dice dice;

    public float diceTime = 10f; //나중에 수정해야
    int currentSecond = 0;
    bool soundPlayed = false;

    void Start()
    {
        diceTimeText.text = diceTime.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if (!manager.timerOn)
            return;

        if (diceTime >= 0)
        {
            diceTime -= Time.deltaTime;
            int newSecond = Mathf.RoundToInt(diceTime);

            if (newSecond != currentSecond)
            {
                currentSecond = newSecond;
                soundPlayed = false;
            }

            if (currentSecond >= 0 && !soundPlayed)
            {
                SoundManager.instance.SoundPlayer("DiceTimer");
                soundPlayed = true;
            }
        }

        else if (diceTime < 0)
        {
            diceTimeText.gameObject.SetActive(false);
            if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
            {
                RpcManager.instance.rollDice();
            }
            diceTime = 3;
            manager.timerOn = false;
        }
        

        diceTimeText.text = Mathf.Round(diceTime).ToString();
        /*if(diceTime == 3 || diceTime == 2 || diceTime == 1 || diceTime == 0)
        {
            SoundManager.instance.SoundPlayer("DiceTimer");
            Debug.Log("dicetimer" + diceTimeText.text);
        }*/
            
    }

}
