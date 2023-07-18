using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCNavigateAnchorSensor : MonoBehaviour, IGroundSensable
{
    [SerializeField] protected GameObject fixedAttachingGround;
    protected Collider2D fixedAttachingGroundCol;

    public string GetPlatformName()
    {
        return fixedAttachingGround.name;
    }

    private void Start()
    {
        if(fixedAttachingGround != null)
            fixedAttachingGroundCol = fixedAttachingGround.GetComponent<Collider2D>();
        else
        {
            Debug.LogWarning("No attaching collider");
        }
    }


    public IEnumerator DisableCollision()
    {
        yield return null;
    }

    public Collider2D GetCurrentAttachedGroundCol()
    {
        return fixedAttachingGroundCol;
    }

    public GameObject GetLastAttachedGroundInfo()
    {
        return fixedAttachingGround;
    }

    public GameObject GetCurrentAttachedGroundInfo()
    {
        return fixedAttachingGround;
    }
}
