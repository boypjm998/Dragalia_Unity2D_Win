using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class BattleEnvironmentManager : MonoBehaviour
{
    private static BattleEnvironmentManager _instance;
    
    public static BattleEnvironmentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BattleEnvironmentManager>();
            }

            return _instance;
        }
    }

    private GameObject globalSceneVolumeGO;
    private Light2D globalLight;
    [SerializeField] private List<Renderer> environmentRenderers = new List<Renderer>();
    private Dictionary<string, Renderer> environmentRendererDict = new Dictionary<string, Renderer>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        InitElements();

    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void InitElements()
    {
        var gos = SceneManager.GetActiveScene().GetRootGameObjects();
        
        //找到gos中名字为SceneVolume的GameObject，使用性能更好的Find方法
        globalSceneVolumeGO = Array.Find(gos, go => go.name == "SceneVolume");
        
        globalLight = globalSceneVolumeGO.GetComponentInChildren<Light2D>();
        
        //把environmentRenderers中的元素放到environmentRendererDict中
        foreach (var renderer in environmentRenderers)
        {
            environmentRendererDict.Add(renderer.gameObject.name, renderer);
        }


    }
    
    public Renderer GetEnvironmentSpriteRenderer(string name)
    {
        if (environmentRendererDict.ContainsKey(name))
        {
            return environmentRendererDict[name];
        }
        else
        {
            return null;
        }
    }

    public List<Renderer> GetAllEnvironmentRenderer()
    {
        return environmentRendererDict.Values.ToList();
    }


    public void SetGlobalLight(float intensity, Color color)
    {
        globalLight.intensity = intensity;
        globalLight.color = color;
    }
    
    public void SetGlobalLight(float intensity)
    {
        globalLight.intensity = intensity;
    }
    
    public void SetGlobalLight(Color color)
    {
        globalLight.color = color;
    }
    
    
    
    


}
