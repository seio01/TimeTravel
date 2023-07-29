using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BsetItemUsePanel : MonoBehaviour
{
    int time;
    public TMP_Text TimeText;
    public TMP_Text useOrNotText;
    public Button yesButton;
    public Button noButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        //모두 B세트 카드가 없을 시, 바로 문제 풀이로 넘어가는 코드 추가할 것.
        //카드 빼앗아서 B세트 2개 이상 가지고 있을 경우의 코드 추가할 것.
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            useOrNotText.text = "문제를 푸는 플레이어는 B세트 카드를 쓸 수 없습니다.\n 다른 사람의 선택을 기다립니다...";
        }
        else
        {
            List<GameManager.items> playerCards = GameManager.instance.player.itemCards;
            if (playerCards.Contains(GameManager.items.cardSteal))
            {
                useOrNotText.text = "카드 빼앗기 카드를 사용하시겠습니까?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else if (playerCards.Contains(GameManager.items.timeSteal))
            {
                useOrNotText.text = "시간 빼앗기 카드를 사용하시겠습니까?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else if (playerCards.Contains(GameManager.items.bind))
            {
                useOrNotText.text = "공동운명체 카드를 사용하시겠습니까?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else
            {
                useOrNotText.text = "쓸 수 있는 아이템이 없습니다. \n 다른 사람의 선택을 기다립니다...";
            }
        }
        time = 5;
        StartCoroutine("setTimer");
    }

    void OnDisable()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    IEnumerator setTimer()
    {
        while (time >= 0)
        {
            TimeText.text = "선택 종료까지: " + time.ToString() + "초 남았습니다.";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
        }
        this.gameObject.SetActive(false);
    }
}
