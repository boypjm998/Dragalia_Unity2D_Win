using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCharacterAnimation_C033 : MonoBehaviour
{
    [SerializeField] private GameObject[] fxs;




    private void AttackAction(int eventID)
    {
        switch (eventID)
        {
            case 1:
            {
                Instantiate(fxs[0], transform.position, Quaternion.identity, transform.parent);
                break;
            }
        }
    }
    
    
    
    
    
}
