using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class Projectile_C003_6 : MonoBehaviour
{
    protected List<GameObject> stars = new();
    private Animator anim;
    private int index = 0;
    public StatusManager _statusManager;//need set

    private void Start()
    {
        anim = GetComponent<Animator>();
        var starTransform = transform.Find("Stars");
        for (int i = 0; i < 5; i++)
        {
            stars.Add(starTransform.GetChild(i).gameObject);
        }
    }

    public void ToNextForcingStage()
    {
        stars[index].GetComponent<SpriteRenderer>().color = Color.yellow;
        stars[index].GetComponent<Animator>().enabled = true;
        index++;
        SendMessageUpwards("ToForcingState",index);
    }

    private void Update()
    {
        if (_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.HolyFaith) > 0)
        {
            anim.speed = 1.5f;
        }
        else anim.speed = 1f;
    }
}
