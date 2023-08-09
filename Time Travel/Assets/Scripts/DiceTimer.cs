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
    void Start()
    {
        diceTimeText.text = diceTime.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if (!manager.timerOn)
            return;

        if (diceTime > 0)
        {
            diceTime -= Time.deltaTime;
        }

        else if (diceTime <= 0)
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
    }

}
