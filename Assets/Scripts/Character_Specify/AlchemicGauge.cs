using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemicGauge : MonoBehaviour
{
    [SerializeField] private int catridgeCount = 0;
    [SerializeField] private int cp = 0;
    [SerializeField] private GameObject number;
    [SerializeField] private GameObject gaugeRect;
    [SerializeField] private GameObject gaugeRing;
    [SerializeField] private GameObject catridges;

    [SerializeField] private Sprite number0Sprite;
    [SerializeField] private Sprite number1Sprite;
    [SerializeField] private Sprite number2Sprite;
    [SerializeField] private Sprite number3Sprite;
    [SerializeField] private Sprite numberLoadedSprite;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color maxFillColor;
    [SerializeField] private Color activeColor;

    [SerializeField] private bool catridgeActive;
    [SerializeField] private int activeCatridgeCount = 0;

    private bool startAnimIsFinished;
    private bool startAnimIsStarted = false;
    private bool endAnimIsStarted = false;

    private Coroutine catFillRoutine;

    private PlayerStatusManager stat;



    private void Awake()
    {
        
        normalColor = new Color(255, 204, 0, 1);
        //maxFillColor = new Color(254, 119, 53, 1);
        
        //activeColor = new Color(80, 222, 226, 1);
    }


    // Start is called before the first frame update
    void Start()
    {
        //yield return new WaitUntil(()=>GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        number = transform.Find("Number").gameObject;
        gaugeRect = transform.Find("SliderRec").gameObject;
        gaugeRing = transform.Find("SliderRing").gameObject;
        catridges = transform.Find("Catridge").gameObject;
        stat = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();

        //numberImages will be set in the prefab.
        catridgeActive = false;
        startAnimIsFinished = false;

        catFillRoutine = null;
    }

    // Update is called once per frame
    private void Update()
    {
        
        if (catridgeActive == false)
        {
            startAnimIsFinished = false;
            startAnimIsStarted = false;
            endAnimIsStarted = false;
            CPCalc();
            DisplayNumber();
            DisplayRingGauge();
            DisplayRectGauge();
        }
        else {
            if (!startAnimIsStarted)
            {
                stat.SetRequiredSP(1, 3060);
                startAnimIsStarted = true;
                StartCoroutine(ActiveAnimation());
                activeCatridgeCount = catridgeCount;
            }
            DisplayRingGauge();
            DisplayRectGauge();

            if (startAnimIsFinished && !endAnimIsStarted)
            {
                cp = 33;
            }
            
            if (catridgeCount < activeCatridgeCount)
            {
                activeCatridgeCount--; ;
                StartCoroutine(CatridgeUnloadAnimate(activeCatridgeCount + 1));
            }
            if (catridgeCount == 0 && endAnimIsStarted == false)
            {
                endAnimIsStarted = true;
                StartCoroutine(InactiveAnimation());
                activeCatridgeCount = 0;
                stat.SetRequiredSP(1, 6120);
            }
        }
        

    }

    

    void DisplayRingGauge()
    {
        Slider gaugeImage = gaugeRing.GetComponent<Slider>();
        Image image = gaugeRing.GetComponentInChildren<Image>();

        gaugeImage.value = 7 + (cp % 33);
        if (gaugeImage.value > 24)
        {
            gaugeImage.value = 25;
        }

        if (cp == 33 && (catridgeCount == 3 || catridgeActive))
        {
            gaugeImage.value = 25;
            image.color = maxFillColor;
        }
        else {
            image.color = normalColor;
        }
        if (catridgeActive)
        {
            image.color = activeColor;

        }


    }
    void DisplayRectGauge()
    {
        Slider gaugeImage = gaugeRect.GetComponent<Slider>();
        Image image = gaugeRect.GetComponentInChildren<Image>();


        gaugeImage.value = (cp % 33) - 18;
        if (gaugeImage.value < 0)
        {
            gaugeImage.value = 0;
        }

        if (cp == 33 && (catridgeCount == 3 || catridgeActive))
        {
            gaugeImage.value = 15;
            image.color = maxFillColor;
        }
        else
        {
            image.color = normalColor;
        }
        if (catridgeActive)
        {
            image.color = activeColor;

        }



    }

    void DisplayNumber()

    {
        Image image = number.GetComponent<Image>();
        switch (catridgeCount)
        {
            case 0:
                image.sprite = number0Sprite;
                break;
            case 1:
                image.sprite = number1Sprite;
                break;
            case 2:
                image.sprite = number2Sprite;
                break;
            case 3:
                image.sprite = number3Sprite;
                break;
            default:
                break;

        }




    }

    private void CPCalc()
    {
        if (cp >= 33 && catridgeCount < 2)
        {
            cp %= 33;
            catridgeCount++;

        }
        else if (cp >= 33 && catridgeCount >= 2)
        {
            cp = 33;
            catridgeCount = 3;
        }
        else if (cp < 33 && catridgeCount == 3)
        {
            catridgeCount = 2;
        }

    }


    private IEnumerator ActiveAnimation()
    {
        while (cp > 0)
        {
            cp--;
            yield return new WaitForSeconds(0.01f);
        }
        
        catFillRoutine = StartCoroutine(CatridgeFillAnimate(catridgeCount));
        number.GetComponent<Image>().sprite = numberLoadedSprite;
        
        while (cp < 33)
        {
            cp++;
            yield return new WaitForSeconds(0.01f);
        }
        
        gaugeRect.GetComponent<Slider>().value = 15;
        gaugeRing.GetComponent<Slider>().value = 25;
        startAnimIsFinished = true;
    }

    private IEnumerator InactiveAnimation()
    {
        catridgeActive = false;
        while (cp > 0)
        {
            cp--;
            yield return new WaitForSeconds(0.01f);
        }

        number.GetComponent<Image>().sprite = number0Sprite;

        
        catridgeCount = 0;
        cp = 0;
    }


    private IEnumerator CatridgeFillAnimate(int catnum)
        {
            while (true)
            {

                Slider slider1 = catridges.transform.GetChild(0).GetComponent<Slider>();
                while (slider1.value < 1)
                {
                    slider1.value += 0.1f;
                    yield return new WaitForFixedUpdate();
                }                
                if (--catnum == 0)
                {
                    break;
                }


                Slider slider2 = catridges.transform.GetChild(1).GetComponent<Slider>();
                while (slider2.value < 1)
                {
                    slider2.value += 0.1f;
                    yield return new WaitForFixedUpdate();
                }
                if (--catnum == 0)
                {
                    break;
                }


                Slider slider3 = catridges.transform.GetChild(2).GetComponent<Slider>();
                while (slider3.value < 1)
                {
                    slider3.value += 0.1f;
                    yield return new WaitForFixedUpdate();
                }
                break;

            }
            
        }

    private IEnumerator CatridgeUnloadAnimate(int catID)
    {
        Slider slider1 = catridges.transform.GetChild(0).GetComponent<Slider>();
        Slider slider2 = catridges.transform.GetChild(1).GetComponent<Slider>();
        Slider slider3 = catridges.transform.GetChild(2).GetComponent<Slider>();

        if (catFillRoutine != null)
        {
            StopCoroutine(catFillRoutine);
            catFillRoutine = null;
        }

        switch (catID)
        {
            
            
            case 1:
                slider2.value = 0;
                slider3.value = 0;
                while (slider1.value > 0)
                {
                    slider1.value -= 0.1f;
                    yield return new WaitForFixedUpdate();
                }
                break;
            
            case 2:
                slider3.value = 0;
                while (slider2.value > 0)
                {
                    slider2.value -= 0.1f;
                    yield return new WaitForFixedUpdate();
                }
                break;
            case 3:
                

                while (slider3.value > 0)
                {
                    slider3.value -= 0.1f;
                    yield return new WaitForFixedUpdate();
                }
                break;




        }

    }

    public void CPCharge(int q)
    {
        cp += q;
    }

    public bool IsCatridgeActive()
    {
        return catridgeActive;
    }

    public int GetActiveCatridgeNumber()
    {
        return activeCatridgeCount;
    }

    public int GetCatridgeNumber()
    {
        return catridgeCount;
    }

    public void SetCatridgeActive()
    {
        catridgeActive = true;
    }

    public void CatridgeConsume()
    {
        catridgeCount--;
    }

}





