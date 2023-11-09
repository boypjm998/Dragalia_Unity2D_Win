using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAssetInfoData", menuName = "ScriptableObjects/CharacterResources", order = 1)]
public class CharacterAssetInfo : ScriptableObject
{
    [Serializable]
    public class RelatedDataInfo
    {
        public int id;
        public List<string> resources_name = new();
    }

    public List<RelatedDataInfo> Infos = new();
    public Dictionary<int, List<string>> characterResources;


    public void Init()
    {
        //Infos = new();
        characterResources = new();
        foreach (var info in Infos)
        {
            SetResources(info.id, info.resources_name);
        }
        Debug.Log("Init");
    }

    

    private void SetResources(int id, List<string> resources)
    {
        if (characterResources == null)
        {
            characterResources = new Dictionary<int, List<string>>();
        }

        if (characterResources.ContainsKey(id))
        {
            characterResources[id] = resources;
        }
        else
        {
            characterResources.Add(id, resources);
        }
    }

    public List<string> GetResources(int id)
    {
        if (characterResources != null && characterResources.ContainsKey(id))
        {
            return characterResources[id];
        }

        return null;
    }
}
