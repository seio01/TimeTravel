using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class resultPanel : MonoBehaviour
{
    TMP_Text resultText;
    public GameObject problemPanel;
    // Start is called before the first frame update
    void Start()
    {
        resultText = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        StartCoroutine("setAlphaValue");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {

    }

    IEnumerator setAlphaValue()
    {
        Color backGroundColor = this.gameObject.GetComponent<Image>().color;
        Color textColor = resultText.color;
        yield return new WaitForSeconds(2.0f);
        while (backGroundColor.a >= 0)
        {
            backGroundColor.a -= 0.01f;
            textColor.a -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            resultText.color = textColor;
            this.gameObject.GetComponent<Image>().color = backGroundColor;
        }
        yield return new WaitForSeconds(0.75f);
        problemPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }
    
}
