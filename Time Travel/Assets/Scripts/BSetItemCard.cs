using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler
{
    bool isUsed;
    bool canNotUse;
    // Start is called before the first frame update
    void Start()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        if (spriteName == "Ä«µå»©¾Ñ±â" && canStealCard() == false)
        {
            changeColorBlack();
            canNotUse = true;
        }
        else
        {
            isUsed = false;
            canNotUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentTurnBSetItem > 0)
        {
            canNotUse = true;
            changeColorBlack();
        }
    }

    void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == true || canNotUse==true || GameManager.instance.currentTurnBSetItem > 0)
        {
            return;
        }
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        changeColorBlack();
        isUsed = true;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "¿î¸í°øµ¿Ã¼")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
            RpcManager.instance.makeIsUsedBindTrue(GameManager.instance.localPlayerIndexWithOrder);
        }
        else if (spriteName == "Ä«µå»©¾Ñ±â")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal);
        }
        else
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
            RpcManager.instance.setIsThisTurnTimeStealTrue();
        }
        GameManager.instance.currentTurnBSetItem++;
    }

    bool canStealCard()
    {
        if (DontDestroyObjects.instance.playerItems[GameManager.instance.localPlayerIndexWithOrder].Count == 4 || DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void changeColorBlack()
    {
        Color usedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
    }
}
