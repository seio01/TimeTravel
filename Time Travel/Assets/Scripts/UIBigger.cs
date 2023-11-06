
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBigger : MonoBehaviour
{
    public static UIBigger instance;
    float time;
    public bool bigger;
    public bool smaller;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (bigger)
        {
            Bigger();
        }
        else if(smaller)
        {
            Smaller();
        }
        
    }

    public void Bigger()
    {
        transform.localScale = Vector3.one * (1 + time);
        time += Time.deltaTime;
        if (time > 0.3f)
        {
            bigger = false;
        }
    }

    public void Smaller()
    {
        transform.localScale = Vector3.one * (1.3f - time);
        time += Time.deltaTime;
        if (time > 0.3f)
        {
            smaller = false;
        }
    }

    public void resetTime()
    {
        time = 0;
    }
}
