using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OmiyaGames;

public class TestScrollVerticalTo : MonoBehaviour
{
    [SerializeField]
    ScrollRect scroller;
    [SerializeField]
    bool scrollHere;

    bool lastScrollValue;
    RectTransform rect;

    // Use this for initialization
    void Start()
    {
        lastScrollValue = scrollHere;
        rect = transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastScrollValue != scrollHere)
        {
            Utility.ScrollVerticallyTo(scroller, rect);


            lastScrollValue = scrollHere;
        }
    }
}
