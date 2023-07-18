using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public GameManager manager;
    public Image curSide;
    public Sprite[] diceSides;

    public void RollDice()
    {
        StartCoroutine(RollDiceRoutine());
    }

    IEnumerator RollDiceRoutine()
    {
        int ranSide = 0;
        for(int i = 0; i <=10; i++)
        {
            ranSide = Random.Range(0, 6);
            curSide.sprite = diceSides[ranSide];
            yield return new WaitForSeconds(0.05f);
        }
        
        //ranSide + 1;
        manager.newDiceSide = ranSide + 1;
        
        yield return new WaitForSeconds(1f);
        manager.diceImg.SetActive(false);
        manager.CheckCurPoint(GameManager.playerStartPoint + ranSide + 1);
        
        //manager.MovePlayer();
    }

}
