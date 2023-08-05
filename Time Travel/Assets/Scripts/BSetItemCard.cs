using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler
{
    bool isUsed;
    // Start is called before the first frame update
    void Start()
    {
        isUsed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        if (isUsed == true)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == true)
        {
            return;
        }
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        Color usedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
        isUsed = true;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "운명공동체")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
            //인원 수대로 플레이어 말이 추가되면  코드 추가.
        }
        else if (spriteName == "카드빼앗기")  //클릭하자마자 빼앗은 플레이어 UI에서 사라지는데 resultPanel 뜨면 사라지도록 수정 필요. 새로 추가된 아이템 UI 크기 조정 필요.
        {
            int controlPlayerCardNum = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count;
            int stealCardIndex = Random.Range(0, controlPlayerCardNum);
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal); 
            RpcManager.instance.cardSteal(GameManager.instance.localPlayerIndexWithOrder, stealCardIndex);
        }
        else
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
            RpcManager.instance.setIsThisTurnTimeStealTrue();
        }
    }
}
