using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneController_100003 : CutSceneController
{
    [SerializeField] private GameObject deadSoliderGameObject;
    [SerializeField] private GameObject activeSoliderGameObject;

    [SerializeField] private GameObject playerActorGameObject;
    
    [SerializeField] private GameObject[] fxInstances;
    [SerializeField] private Image blackImage;
    
    private Animator deadSoliderAnimator;
    private Animator activeSoliderAnimator;
    private Animator playerActorAnimator;
    
    private Coroutine cutsceneCoroutine;
    private IEnumerator nextSignal;
    private bool next = false;
    
    public bool start = false;

    public void NextAction()
    {
        next = true;
    }

    private void Start()
    {
        deadSoliderAnimator = deadSoliderGameObject.GetComponentInChildren<Animator>();
        activeSoliderAnimator = activeSoliderGameObject.GetComponentInChildren<Animator>();
        playerActorAnimator = playerActorGameObject.GetComponentInChildren<Animator>();
        
        
        deadSoliderAnimator.Play("die");
        nextSignal = new WaitUntil(()=>next);
        
        //cutsceneCoroutine = StartCoroutine(CutsceneAnimation());
    }

    private void Update()
    {
        if (start)
        {
            start = false;
            if (cutsceneCoroutine == null)
            {
                _director.Play();
                cutsceneCoroutine = StartCoroutine(CutsceneAnimation());
            }
        }
    }


    private IEnumerator CutsceneAnimation()
    {
        yield return nextSignal;

        next = false;
        activeSoliderAnimator.SetTrigger("next");

        yield return null;
        yield return nextSignal;
        
        next = false;
        playerActorAnimator.SetTrigger("next");

        yield return new WaitForSeconds(0.3f);
        
        fxInstances[0].SetActive(true);
        
        yield return null;
        yield return nextSignal;
        next = false;
        
        activeSoliderAnimator.SetTrigger("next");
        
        yield return null;
        yield return nextSignal;
        next = false;
        
        fxInstances[1].SetActive(true);

        yield return null;
        yield return nextSignal;
        next = false;
        
        activeSoliderAnimator.SetTrigger("next");

        yield return new WaitForSeconds(1.5f);

        //blackImage.DOColor(Color.black, 1f);

    }







}
