using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C003_8_Boss : MonoBehaviour
{
    private List<ParticleSystem> ps = new();
    private GameObject Wall1;
    private GameObject Wall2;

    private void Start()
    {
        var dusts = transform.Find("Dust");
        Wall1 = transform.GetChild(2).gameObject;
        Wall2 = transform.GetChild(3).gameObject;
        ps.AddRange(dusts.GetComponentsInChildren<ParticleSystem>());
        Invoke("PlayDust",14.4f);
        Invoke("HideWalls", 14.6f);
        Invoke("DestroyParent", 15f);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void PlayDust()
    {
        foreach (var p in ps)
        {
            p.Play();
        }
    }

    public void HideWalls()
    {
        Wall1.SetActive(false);
        Wall2.SetActive(false);
    }

    public void DestroyParent()
    {
        if(transform.parent.name!="mountain")
            return;
        Destroy(transform.parent.gameObject);
    }
}
