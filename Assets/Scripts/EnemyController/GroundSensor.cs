using System.Collections;
using UnityEngine;


public interface IGroundSensable
{
    public GameObject GetCurrentAttachedGroundInfo();
    public GameObject GetLastAttachedGroundInfo();

    public Collider2D GetCurrentAttachedGroundCol();
    
    public IEnumerator DisableCollision();
}