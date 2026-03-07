using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class SupportSkillNPC : MonoBehaviour
{
    [SerializeField] private List<SupportSkillPrefabInfo> supportSkillPrefabInfos;
    private Animator anim;
    private StandardCharacterController controller;
    [SerializeField] private string animName;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private float waitTime = 1;
    private int animAppended = 0;

    public SupportSkillPrefabInfo GetSkillPrefab(int index)
    {
        return supportSkillPrefabInfos[index];
    }

    [Serializable]
    public class SupportSkillPrefabInfo
    {
        public GameObject prefab;
        public float triggerTime;
        public bool includeAttack;
        public bool useFaceDir = true;
        public PositionInfo positionType = PositionInfo.CurrentPosition;
        private Func<GameObject, List<GameObject>> getTargetCustomFunc;
        public Action<AttackBase> onAttackCreated = null;
        public Action<GameObject> onObjectCreated = null;
        public bool useCustomFunc
        {
            get { return getTargetCustomFunc != null; }
        }

        public Vector3 positionArg;
        
        /// <summary>
        /// arg0: player's GameObject, return: List of Target GameObjects
        /// </summary>
        /// <param name="func"></param>
        public void SetCustomFunc(Func<GameObject, List<GameObject>> func)
        {
            getTargetCustomFunc = func;
        }
        
        // public void SetOnAttackCreated(Action<AttackBase> action)
        // {
        //     onAttackCreated = action;
        // }
        //
        // public void SetOnObjectCreated(Action<GameObject> action)
        // {
        //     onObjectCreated = action;
        // }
        
        public List<GameObject> GetTargetCustom(GameObject player)
        {
            return getTargetCustomFunc(player);
        }
        public enum PositionInfo
        {
            CurrentPosition,
            RaycastedPosition,
            AbsolutePosition
        }
        
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<StandardCharacterController>();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < supportSkillPrefabInfos.Count; i++)
        {
            Invoke("SpawnProjectile", supportSkillPrefabInfos[i].triggerTime);
        }
        anim.Play(animName);
    }

    private void SpawnProjectile()
    {
        var info = supportSkillPrefabInfos[0];
        var prefab = info.prefab;

        List<GameObject> targetList = new();

        if (info.useCustomFunc)
        {
            targetList = info.GetTargetCustom(gameObject);
        }
        else
        {
            targetList.Add(gameObject);
        }

        GameObject container = null;
        if (info.includeAttack)
        {
            container = BattleStageManager.Instance.GetNewRangedContainer(false);
        }
        
        foreach (var target in targetList)
        {
            Vector3 position = transform.position;
            if (info.positionType == SupportSkillPrefabInfo.PositionInfo.CurrentPosition)
            {
                position = target.transform.position + 
                           new Vector3(info.positionArg.x * controller.facedir,info.positionArg.y,info.positionArg.z);
            }else if (info.positionType == SupportSkillPrefabInfo.PositionInfo.RaycastedPosition)
            {
                position = (Vector3)target.RaycastedPosition() + 
                           new Vector3(info.positionArg.x * controller.facedir,info.positionArg.y,info.positionArg.z);
            }else if (info.positionType == SupportSkillPrefabInfo.PositionInfo.AbsolutePosition)
            {
                position = info.positionArg;
            }

            if (info.includeAttack)
            {
                var proj = this.InstantiateRangedObject(prefab, position, container, 
                    info.useFaceDir?controller.facedir:1, 1, transform);
                var atk = proj.GetComponent<AttackBase>();
                info.onAttackCreated?.Invoke(atk);

                var dotween = proj.GetComponent<DOTweenSimpleController>();
                if (dotween != null)
                {
                    dotween.moveDirection.x *= controller.facedir;
                }

            }
            else
            {
                var proj = Instantiate(prefab,position,Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);
                
                if(info.useFaceDir)
                    proj.transform.localScale = new Vector3(controller.facedir,1,1);
                
                info.onObjectCreated?.Invoke(proj);
            }
            
            
        }
        
        if(supportSkillPrefabInfos.Count > 1)
            supportSkillPrefabInfos.RemoveAt(0);
        else Destroy(gameObject,destroyTime);

    }
    
    public void AppendAnimation(float delay, string animName)
    {
        if (animAppended < 10)
        {
            DOVirtual.DelayedCall(delay + waitTime, () =>
            {
                anim.Play(animName);
            },false);
            animAppended++;
        }
    }
    
}
