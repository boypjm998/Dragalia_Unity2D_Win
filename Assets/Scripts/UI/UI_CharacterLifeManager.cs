using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterLifeManager : MonoBehaviour
{
    //private GameObject lifeHeart;
    [SerializeField] private GameObject lifeHeartPrefab;
    private int lifeRemain;

    private PlayerStatusManager _statusManager;
    // Start is called before the first frame update
    void Start()
    {
        _statusManager = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        lifeRemain = _statusManager.remainReviveTimes;
        while (transform.childCount < lifeRemain)
        {
            LifeAdd();
        }
    }
    
    private void LifeAdd()
    {
        Instantiate(lifeHeartPrefab, transform);
        //lifeRemain = _statusManager.remainReviveTimes;
    }

    private void LifeBreak()
    {
        var breakHeart = transform.GetChild(transform.childCount - 1);
        breakHeart.GetComponent<Image>().enabled = false;
        breakHeart.GetComponent<ParticleSystem>().Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeRemain > _statusManager.remainReviveTimes)
        {
            lifeRemain = _statusManager.remainReviveTimes;
            LifeBreak();
        }
        else if (lifeRemain < _statusManager.remainReviveTimes)
        {
            lifeRemain = _statusManager.remainReviveTimes;
            LifeAdd();
        }
    }
}
