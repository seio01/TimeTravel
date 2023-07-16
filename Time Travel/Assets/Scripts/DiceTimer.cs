using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTimer : MonoBehaviour
{
    public GameManager manager;
    public float diceTime = 10f;
    public Text diceTimeText;
    public Dice dice;

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
            dice.RollDice();
            diceTime = 3;
            manager.timerOn = false;
        }

        diceTimeText.text = Mathf.Round(diceTime).ToString();
    }

}
