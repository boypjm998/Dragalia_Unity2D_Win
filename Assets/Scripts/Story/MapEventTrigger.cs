using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventTrigger : MonoBehaviour
{
    [SerializeField] private int eventID;
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(triggered)
            return;
        
        if(other.CompareTag("Player") == false)
            return;


        BattleStageManager.Instance.OnMapEventTriggered?.Invoke(eventID);
        triggered = true;
    }
}
