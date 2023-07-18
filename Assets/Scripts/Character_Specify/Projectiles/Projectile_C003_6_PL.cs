using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_6_PL : MonoBehaviour
    {
        protected List<GameObject> stars = new();
        List<Animator> startAnimators = new();
        List<SpriteRenderer> startRenderers = new();
        private int index = 0;
        public ActorController_c003 ac;
        public TargetAimer ta;
        public StatusManager _statusManager;
        public GameObject target;
        [SerializeField] private GameObject Fill;
        private int currentFSLV = 0;
        public AudioClip forceSE;

        private void Start()
        {
            var starTransform = transform.GetChild(0).Find("Stars");
            for (int i = 0; i < 5; i++)
            {
                stars.Add(starTransform.GetChild(i).gameObject);
                startAnimators.Add(stars[i].GetComponent<Animator>());
                startRenderers.Add(stars[i].GetComponent<SpriteRenderer>());
            }
        }

        public void ActiveStarAnimation(int index,bool flag)
        {
            
            
            
            
            startAnimators[index-1].enabled = flag;
            startRenderers[index-1].color = flag?Color.yellow:new Color(0.9f,0.9f,0.9f,1);
            //SendMessageUpwards("ToForcingState",index);
            if (flag == false)
                stars[index - 1].transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void AimTarget()
        {
            var targetTransform = ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 8f,
                LayerMask.GetMask("Enemies"));
            
            var reversedTargetTransform = ta.GetNearestTargetInRangeDirection
            (-ac.facedir,20f, 8f,
                LayerMask.GetMask("Enemies"));

            if(targetTransform == null)
                targetTransform = reversedTargetTransform;
            else if(reversedTargetTransform!=null)
            {
                if(ta.HasMarking(reversedTargetTransform) && !ta.HasMarking(targetTransform))
                    targetTransform = reversedTargetTransform;
            }





            if (targetTransform == null)
            {
                targetTransform = ac.transform;
            }

            var targetCol = BasicCalculation.CheckRaycastedPlatform(targetTransform.gameObject);

            var position = new Vector3(targetTransform.position.x, targetCol.bounds.max.y, 0);
            
            transform.position = position;
            
            ac.forceAttackPosition = position;
        }

        private void OnDestroy()
        {
            if (ac.hurt)
            {
                return;
            }

            ac.GetComponent<AttackManager_C003>().TwilightMoonRelease(currentFSLV);
        }

        private void Update()
        {
            if (ac.forceLevel < 0)
            {
                Destroy(gameObject);
                Destroy(GetComponentInParent<AttackContainer>().gameObject);
                return;
            }

            
            
            
            AimTarget();
          
            
            
            if (ac.forceLevel < ac.maxForceLevel)
            {
                Fill.transform.localScale = (ac.forcingTime / ac.forcingRequireTime) * Vector3.one;
            }
            else
            {
                Fill.transform.localScale = Vector3.one;
            }

            if (ac.forceLevel == 0)
            {
                ActiveStarAnimation(5,false);
                ActiveStarAnimation(4,false);
                ActiveStarAnimation(3,false);
                ActiveStarAnimation(2,false);
                ActiveStarAnimation(1,false);
            }
            if (ac.forceLevel >= 1)
            {
                ActiveStarAnimation(5,false);
                ActiveStarAnimation(4,false);
                ActiveStarAnimation(3,false);
                ActiveStarAnimation(2,false);
                ActiveStarAnimation(1,true);
            }
            if (ac.forceLevel >= 2)
            {
                ActiveStarAnimation(5,false);
                ActiveStarAnimation(4,false);
                ActiveStarAnimation(3,false);
                ActiveStarAnimation(2,true);
            }
            if (ac.forceLevel >= 3)
            {
                ActiveStarAnimation(5,false);
                ActiveStarAnimation(4,false);
                ActiveStarAnimation(3,true);
            }
            if (ac.forceLevel >= 4)
            {
                ActiveStarAnimation(5,false);
                ActiveStarAnimation(4,true);
            }
            if (ac.forceLevel >= 5)
            {
                ActiveStarAnimation(5,true);
            }
            
            if (currentFSLV < ac.forceLevel)
            {
                GetComponent<MuzzleSESender>().SendVoiceToPlay(forceSE);
            }
            currentFSLV = ac.forceLevel;
            
            
            

        }
    }
}

