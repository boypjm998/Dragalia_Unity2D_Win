using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] private int eventID;
    bool isTriggered = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(isTriggered)
            return;
        if (col.GetComponentInParent<PlayerInput>() != null)
        {
            isTriggered = true;
            TutorialLevelManager.Instance.StartCutScene(eventID);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
