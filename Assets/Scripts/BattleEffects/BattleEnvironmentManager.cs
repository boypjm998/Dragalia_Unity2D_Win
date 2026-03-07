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
    [SerializeField] private List<ParticleSystem> environmentParticleSystems = new List<ParticleSystem>();

    private Dictionary<string, Renderer> environmentRendererDict = new Dictionary<string, Renderer>();
    private Dictionary<string, ParticleSystem> environmentParticleSystemDict = new Dictionary<string, ParticleSystem>();

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
        
        //把environmentParticleSystems中的元素放到environmentParticleSystemDict中
        foreach (var particleSystem in environmentParticleSystems)
        {
            environmentParticleSystemDict.Add(particleSystem.gameObject.name, particleSystem);
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
    
    public void AddEnvironmentRenderer(Renderer renderer)
    {
        if (!environmentRenderers.Contains(renderer))
        {
            environmentRenderers.Add(renderer);
            environmentRendererDict.Add(renderer.gameObject.name, renderer);
        }
    }
    
    private ParticleSystem GetParticleSystem(string name)
    {
        if (environmentParticleSystemDict.ContainsKey(name))
        {
            return environmentParticleSystemDict[name];
        }
        else
        {
            return null;
        }
    }
    
    private void AddParticleSystem(ParticleSystem particleSystem)
    {
        if (!environmentParticleSystems.Contains(particleSystem))
        {
            environmentParticleSystems.Add(particleSystem);
            environmentParticleSystemDict.Add(particleSystem.gameObject.name, particleSystem);
        }
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
