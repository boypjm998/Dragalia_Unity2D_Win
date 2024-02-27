using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ManaCircleMenu : MonoBehaviour
{
    [SerializeField] private GameObject crownInfo;
    [SerializeField] private GameObject iconParents;
    [SerializeField] private GameObject upgradeRequirementGameObject;
    [SerializeField] private GameObject upgradedCompletedGameObject;
    [SerializeField] private Transform selectEffect;
    [SerializeField] private Transform unlockEffect;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button upgradeOrDegradationButton;
    [SerializeField] private TextAsset skillTreeConfig;
    [SerializeField] private AudioSource seSource;
    
    /// 节点信息
    [SerializeField] private Image nodeIconImage;
    
    /// 所需Crown
    [SerializeField] private TextMeshProUGUI crownNeedDispalyTMP;
    
    /// 能力名
    [SerializeField] private TextMeshProUGUI nodeNameTMP;
    
    /// 能力详情
    [SerializeField] private TextMeshProUGUI nodeDetailTMP;
    
    /// 当前剩余Crown
    [SerializeField] private TextMeshProUGUI crownCountTMP;



    private int currentSelectedNodeID = 0;
    private List<Button> skillTreeButtons = new();
    public static int MaxSkillTreeNodes = 5;
    private int currentCrownCount;
    private GameOptions _gameOptions;
    /// <summary>
    /// 玩家的记录
    /// </summary>
    private List<int> _skillTreeNodes;
    private SkillTreeInfo _skillTreeInfo;
    private GlobalController.Language gameLanguage;

    private bool animating = false;

    private void OnEnable()
    {
        _gameOptions = GlobalController.Instance.gameOptions;
        _skillTreeNodes = _gameOptions.skillTreeInfo;
        InitializeSkillTreeNodes();
        CheckConflicts();
    }

    private void InitializeSkillTreeNodes()
    {
        gameLanguage = GlobalController.Instance.GameLanguage;
        
        _skillTreeInfo = JsonConvert.DeserializeObject<SkillTreeInfo>(skillTreeConfig.text);
        
        MaxSkillTreeNodes = _skillTreeInfo.skillTreeNodes.Count;
        
        while (_gameOptions.skillTreeInfo.Count < MaxSkillTreeNodes)
        {
            if (_gameOptions.skillTreeInfo.Count == 0)
            {
                _gameOptions.skillTreeInfo.Add(1);
            }
            else
            {
                _gameOptions.skillTreeInfo.Add(0);
            }
        }
        skillTreeButtons.AddRange(iconParents.GetComponentsInChildren<Button>());
        
        DisplayViewport();
        DisplayNodeInfo(0);
        selectEffect.gameObject.SetActive(true);
    }

    public static void InitializeSkillTreeNodesIfNull()
    {
        
    }

    private void ResetSkillTreeNodes()
    {
        for(int i = 0; i < _skillTreeNodes.Count; i++)
        {
            if(i == 0)
                _skillTreeNodes[i] = 1;
            else _skillTreeNodes[i] = 0;
        }
        CheckConflicts();
    }

    private void CheckConflicts()
    {
        for (int i = 0; i < _skillTreeNodes.Count; i++)
        {
            if (_skillTreeNodes[i] == 1 && !CheckUnlock(_skillTreeInfo.skillTreeNodes[i]))
            {
                LockNode(i);
            }
        }
    }

    private void DisplayViewport()
    {
        foreach (var iconBtn in skillTreeButtons)
        {
            var index = iconBtn.transform.GetSiblingIndex();
            
            iconBtn.onClick.RemoveAllListeners();
            iconBtn.onClick.AddListener(()=>DisplayNodeInfo(index));

            if (CheckUnlock(_skillTreeInfo.skillTreeNodes[index]))
            {
                iconBtn.interactable = true;
                if (_skillTreeNodes[index] == 0)
                {
                    iconBtn.image.color = new Color(0.4f, 0.4f, 0.4f, 1);
                }
                else
                {
                    iconBtn.image.color = Color.white;
                }
            }
            else
            {
                iconBtn.interactable = false;
                //iconBtn.GetComponent<Image>().color = new Color(0, 0, 0,1);
            }

        }
        
        currentCrownCount = GlobalController.Instance.GetTotalCrownCount() - GetTotalCrownCost();
        if (currentCrownCount < 0)
        {
            ResetSkillTreeNodes();
            currentCrownCount = GlobalController.Instance.GetTotalCrownCount() - GetTotalCrownCost();
        }
        
        if (currentCrownCount > 999)
        {
            currentCrownCount = 999;
        }
        
        crownCountTMP.text = currentCrownCount.ToString();
        
    }
    
    private void DisplayNodeInfo(int id)
    {
        var node = _skillTreeInfo.GetNode(id);
        
        selectEffect.SetParent(skillTreeButtons[id].transform);
        selectEffect.position = skillTreeButtons[id].transform.position;
        

        nodeIconImage.sprite = skillTreeButtons[id].GetComponent<Image>().sprite;
        nodeNameTMP.text = node.name[(int)gameLanguage];
        nodeDetailTMP.text = node.description[(int)gameLanguage];
        if (gameLanguage == GlobalController.Language.EN)
        {
            crownNeedDispalyTMP.text = $"Needed {node.crownCost}/{currentCrownCount}";
        }
        else
        {
            crownNeedDispalyTMP.text = $"所需{node.crownCost}/{currentCrownCount}";
        }

        if (_skillTreeNodes[id] == 1)
        {
            crownNeedDispalyTMP.color = Color.black;
            upgradedCompletedGameObject.SetActive(true);
            upgradeRequirementGameObject.SetActive(false);
            upgradeOrDegradationButton.gameObject.SetActive(false);
            
        }
        else if(currentCrownCount >= node.crownCost)
        {
            crownNeedDispalyTMP.color = Color.black;
            upgradeOrDegradationButton.gameObject.SetActive(true);
            upgradeOrDegradationButton.animator.Play("Normal");
            upgradeOrDegradationButton.image.color = Color.white;
            upgradedCompletedGameObject.SetActive(false);
            upgradeRequirementGameObject.SetActive(true);
            upgradeOrDegradationButton.onClick.RemoveAllListeners();
            upgradeOrDegradationButton.onClick.AddListener(()=>UnlockNewNode(id));
        }
        else
        {
            crownNeedDispalyTMP.color = Color.red;
            upgradeOrDegradationButton.gameObject.SetActive(false);
            upgradedCompletedGameObject.SetActive(false);
            upgradeRequirementGameObject.SetActive(true);
            //upgradeOrDegradationButton.interactable = false;
        }


        currentSelectedNodeID = id;

    }

    private void LockNode(int id)
    {
        _gameOptions.skillTreeInfo[id] = 0;
        _skillTreeNodes = _gameOptions.skillTreeInfo;
        currentCrownCount = GlobalController.Instance.GetTotalCrownCount() - GetTotalCrownCost();
        crownCountTMP.text = currentCrownCount.ToString();
    }

    private void UnlockNewNode(int id)
    {
        _gameOptions.skillTreeInfo[id] = 1;
        _skillTreeNodes = _gameOptions.skillTreeInfo;
        unlockEffect.SetParent(skillTreeButtons[id].transform);
        unlockEffect.transform.position = skillTreeButtons[id].transform.position;
        unlockEffect.gameObject.SetActive(true);
        seSource.Play();
        
        DisplayViewport();
        DisplayNodeInfo(id);
        
        currentCrownCount = GlobalController.Instance.GetTotalCrownCount() - GetTotalCrownCost();
        crownCountTMP.text = currentCrownCount.ToString();
    }



    private bool CheckUnlock(SkillTreeNode node)
    {
        var conditionList = node.preNodes;
        var allPre = node.unlockedRequireAllPreNodes;

        if (conditionList.Count == 0)
            return true;

        if (conditionList[0] < 0)
            return false;

        if (allPre)
        {
            foreach (var cond in conditionList)
            {
                if (_skillTreeNodes[cond] == 0)
                    return false;
            }

            return true;
        }
        else
        {
            foreach (var cond in conditionList)
            {
                if (_skillTreeNodes[cond] == 1)
                    return true;
            }

            return false;
        }

    }

    private int GetTotalCrownCost()
    {
        int count = 0;
        for (int i = 0; i < MaxSkillTreeNodes; i++)
        {
            if (_skillTreeNodes[i] > 0)
            {
                count += _skillTreeInfo.skillTreeNodes[i].crownCost;
            }
        }

        return count;
    }





    [Serializable]
    public class SkillTreeInfo
    {
        public List<SkillTreeNode> skillTreeNodes = new();

        public SkillTreeNode GetNode(int id)
        {
            return skillTreeNodes.Find(x => x.nodeID == id);
        }

    }

    [Serializable]
    public class SkillTreeNode
    {
        public int nodeID;
        public int crownCost;
        public List<int> preNodes = new();
        public bool unlockedRequireAllPreNodes;
        public string[] name;
        public string[] description;
    }
    
}
