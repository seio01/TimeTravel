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
    public Sprite[] itemCardImages;

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
        if (checkIfAllDoesntHaveBsetCard() == true)
        {
            useOrNotText.text = "모든 플레이어가 쓸 수 있는 카드가 없습니다.. \n";
            GameManager.instance.AllDoesntHaveBsetCard = true;
            TimeText.text = "";
            Invoke("setActiveFalse", 1.5f);
        }
        else
        {
            if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
            {
                useOrNotText.text = "문제를 푸는 플레이어는 B세트 카드를 쓸 수 없습니다.\n 다른 사람의 선택을 기다립니다...";
            }
            else
            {
                List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.localPlayerIndexWithOrder];

                if (playerCards.Contains(DontDestroyObjects.items.cardSteal) || playerCards.Contains(DontDestroyObjects.items.timeSteal) || playerCards.Contains(DontDestroyObjects.items.bind))
                {
                    GameObject itemCardPrefab = Resources.Load<GameObject>("Prefabs/itemImage");
                    transform.GetChild(2).gameObject.SetActive(true);
                    if (playerCards.Contains(DontDestroyObjects.items.cardSteal))
                    {
                        itemCardPrefab = Resources.Load<GameObject>("Prefabs/itemImage");
                        GameObject obj = Instantiate(itemCardPrefab);
                        obj.GetComponent<Image>().sprite = itemCardImages[0];
                        obj.transform.parent = transform.GetChild(2);
                    }
                    if (playerCards.Contains(DontDestroyObjects.items.timeSteal))
                    {
                        itemCardPrefab = Resources.Load<GameObject>("Prefabs/itemImage");
                        GameObject obj = Instantiate(itemCardPrefab);
                        obj.GetComponent<Image>().sprite = itemCardImages[1];
                        obj.transform.parent = transform.GetChild(2);
                    }
                    if (playerCards.Contains(DontDestroyObjects.items.bind))
                    {
                        itemCardPrefab = Resources.Load<GameObject>("Prefabs/itemImage");
                        GameObject obj = Instantiate(itemCardPrefab);
                        obj.GetComponent<Image>().sprite = itemCardImages[2];
                        obj.transform.parent = transform.GetChild(2);
                    }
                    useOrNotText.text = "사용할 카드를 선택해주세요. \n (시간 내 선택되지 않을 시 카드는 사용되지 않습니다.)";
                }
                else
                {
                    useOrNotText.text = "쓸 수 있는 카드가 없습니다. \n 다른 사람의 선택을 기다립니다...";
                }
            }
            time = 5;
            StartCoroutine("setTimer");
        }
    }

    void OnDisable()
    {
        transform.GetChild(2).gameObject.SetActive(false);
    }

    bool checkIfAllDoesntHaveBsetCard()
    {
        for (int i = 0; i < GameManager.instance.initialPlayerNum; i++)
        {
            if (i  == GameManager.instance.controlPlayerIndexWithOrder)
            {
                continue;
            }
            if (quitInTheMiddle.instance.outPlayerIndex.Contains(i))
            {
                continue;
            }
            if (DontDestroyObjects.instance.playerItems[i].Contains(DontDestroyObjects.items.timeSteal) == true)
            {
                return false;
            }
            else if (DontDestroyObjects.instance.playerItems[i].Contains(DontDestroyObjects.items.bind) == true)
            {
                return false;
            }
            else if (DontDestroyObjects.instance.playerItems[i].Contains(DontDestroyObjects.items.cardSteal) == true)
            {
                if (DontDestroyObjects.instance.playerItems[i].Count == 4 || DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count == 0)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
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

    void setActiveFalse()
    {
        this.gameObject.SetActive(false);
    }
}
