using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class errorMessage : MonoBehaviour
{
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
        StartCoroutine("setTimerForPanel");
    }

    IEnumerator setTimerForPanel()
    {
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
        yield return null;
    }
}
