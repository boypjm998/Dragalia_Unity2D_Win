using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AllIn1SpriteShader;

public class TotalDamageAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject number;
    GameObject totalTxt;
    private Material matNum; 
    private Material matTxt;
    public TextMeshPro tmp;

    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private float lastTime;

    private float fadeSpeed;
    private float fadeTime2;
    //public AllIn1Shader allIn1Shader;


    private void Awake()
    {
        
        number = transform.Find("Number").gameObject;
        Debug.Log(number);
        totalTxt = transform.Find("Total").gameObject;
        matTxt = totalTxt.GetComponent<SpriteRenderer>().material;

        tmp = number.GetComponent<TextMeshPro>();

        matNum = tmp.spriteAsset.material;
        matNum.SetFloat("_Alpha", 0f);
        //matTxt.color = new Color(matTxt.color.r, matTxt.color.g, matTxt.color.b, 0);
        matTxt.SetFloat("_Alpha", 0f);


        fadeSpeed = 1 / fadeTime;
        fadeTime2 = fadeTime;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTime > 0)
        {
            fadeTime -= Time.deltaTime;
            var newAlpha = matNum.GetFloat("_Alpha") + fadeSpeed * Time.deltaTime > 1 ? 1 : matNum.GetFloat("_Alpha") + fadeSpeed * Time.deltaTime;
            matNum.SetFloat("_Alpha", newAlpha);
            matTxt.SetFloat("_Alpha", newAlpha);
            //matTxt.color = new Color(matTxt.color.r, matTxt.color.g, matTxt.color.b, newAlpha);
        }
        else if (lastTime > 0)
        {
            lastTime -= Time.deltaTime;

        }
        else if (fadeTime2 > 0)
        {
            fadeTime2 -= Time.deltaTime;
            var newAlpha = matNum.GetFloat("_Alpha") + fadeSpeed * Time.deltaTime < 0 ? 0 : matNum.GetFloat("_Alpha") - fadeSpeed * Time.deltaTime;
            matNum.SetFloat("_Alpha", newAlpha);
            matTxt.SetFloat("_Alpha", newAlpha);
            //matTxt.color = new Color(matTxt.color.r, matTxt.color.g, matTxt.color.b, newAlpha);
        }
        else {
            Destroy(gameObject);
        }

    }
}
