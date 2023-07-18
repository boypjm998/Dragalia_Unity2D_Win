using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MyDebug : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private Animator anim;
    private PlayableDirector director;

    private IEnumerator Start()
    {
        var rt = Resources.Load<GameObject>("Timeline/HB02_RT");
        Instantiate(rt, Vector3.zero, Quaternion.identity,transform);
        yield return null;
        var fullscreen = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;
        fullscreen.SetActive(true);
        fullscreen.GetComponent<RawImage>().texture = 
            transform.GetChild(0).Find("Camera").GetComponent<Camera>().targetTexture;
    }
}
