using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB02 : EnemyMoveManager
{

    [SerializeField] private Vector2 upperFirePointOffset;
    [SerializeField] private GameObject redSoulPrefab;

    private GameObject _groundMagmaInstance = null;
    private GameObject _shieldInstance = null;
    
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlyingHigh>();

    }

    public IEnumerator DB02_Action01()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.25f);
        
        anim.Play("combo1");

        yield return new WaitForSeconds(0.45f);

        ClawAttack();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator DB02_Action02()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB02_Action02");

        anim.Play("blast_1");

        Instantiate(GetProjectileOfFormatName("action02"), transform.position + new Vector3(0, 2),
            Quaternion.identity, RangedAttackFXLayer.transform);
        yield return new WaitForSeconds(2f);

        anim.Play("blast_3");

        yield return new WaitForSeconds(0.5f);
        
        Meltdown();
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("blast_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator DB02_Action03()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner.PrintSkillName("DB02_Action03");
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.5f);

        yield return new WaitForSeconds(0.3f);
        
        anim.Play("rage");

        yield return new WaitForSeconds(0.75f);
        
        Instantiate(GetProjectileOfFormatName("action03_1"),
            transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(0.1f);

        var lockedPosition = _behavior.targetPlayer.RaycastedPosition();
        
        yield return new WaitForSeconds(0.1f);
        
        InfernoMove(lockedPosition);

        yield return new WaitForSeconds(0.5f);

        InfernoAttack();
        
        yield return null;
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator DB02_Action03M()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        ac.SetGravityScale(0);
        
        bossBanner.PrintSkillName("DB02_Action03");
        //BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.5f);
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,7),
            true);

        yield return new WaitForSeconds(1f);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("dash");
        Instantiate(GetProjectileOfFormatName("action03_1"),
            transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.24f);
        
        DashAttack(0.46f);
        
        yield return null;
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator DB02_Action04()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        //ac.SetGravityScale(0);
        yield return new WaitForSeconds(0.1f);
        anim.Play("charge_1");

        yield return new WaitForSeconds(1f);
        
        anim.Play("charge_3a");
        
        yield return new WaitForSeconds(0.67f);
        
        FireballMuzzle();
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2.5f);
        var container = InitContainer(false, 8);
        Fireball(1.00f,0.8f,container);
        Fireball(1.33f,0.8f,container);
        Fireball(1.67f,0.8f,container);
        Fireball(2.00f,0.8f,container);
        
        yield return null;
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    public IEnumerator DB02_Action05()
    {
        yield return _canActionOnFlyingGround;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        _voiceController?.BroadCastMyVoice(0);
        bossBanner.PrintSkillName("DB02_Action05");
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,7),
            true);

        anim.Play("breathe_long_1");
        
        yield return new WaitForSeconds(1.2f);
        
        anim.Play("breathe_long_3");

        yield return new WaitForSeconds(.3f);
        
        //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        var currentGroundPosition = gameObject.RaycastedPosition();
        var mouthPosition = new Vector2(transform.position.x + ac.facedir * 6, transform.position.y + 5.5f);
        var container = BreatheMuzzle(mouthPosition);
        
        yield return new WaitForSeconds(0.25f);
        
        BreathGroundFlame(container, currentGroundPosition + new Vector2(ac.facedir * 8,0));

        yield return null;
        
        yield return new WaitForSeconds(1.75f);
        
        anim.Play("breathe_long_5");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);

        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// whirl wind
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB02_Action06()
    {
        yield return _canAction;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(100);
        bossBanner.PrintSkillName("DB02_Action06");
        _voiceController?.BroadCastMyVoice(0);
        
        yield return new WaitForSeconds(0.1f);
        
        anim.Play("sideroar_1");

        var hintTuple = GenerateWhirlwindHint(1.5f);
        var posL = hintTuple.hintL.transform.position;
        var posR = hintTuple.hintR.transform.position;

        yield return new WaitForSeconds(1f);
        
        anim.Play("sideroar_3");
        
        yield return new WaitForSeconds(0.35f);

        GenerateWhirlwind(posL,posR);
        
        ac.SetCounter(false);
        ac.currentKBRes = 999;
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("sideroar_5");

        yield return null;
        
        ac.ResetGravityScale();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// chaser
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB02_Action07()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB02_Action07");

        anim.Play("blast_1");
        
        yield return new WaitForSeconds(0.5f);
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,2);
        var pos0 = CapturePlayerPositionX();
        
        yield return new WaitForSeconds(0.5f);
        var pos1 = CapturePlayerPositionX();
        
        yield return new WaitForSeconds(0.5f);
        var pos2 = CapturePlayerPositionX();

        var posArray = CalculateChasePositionsAndSpawnHint(pos0, pos1, pos2, 0.5f,1.9f);

        yield return new WaitForSeconds(0.5f);

        anim.Play("blast_3");

        yield return new WaitForSeconds(1.5f);
        
        ChaserPillar(posArray);
        _voiceController?.BroadCastMyVoice(0);
        
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("blast_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Ground
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB02_Action08()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB02_Action08");

        StageCameraController.SwitchOverallCamera();
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("sideroar_1");
        
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            new Vector3(0,BattleStageManager.Instance.mapBorderB - 0.5f),RangedAttackFXLayer.transform,
            new Vector2(5,BattleStageManager.Instance.mapBorderR-BattleStageManager.Instance.mapBorderL),
            Vector2.zero, false,0,3,90,0.5f,true,false,
            true);
        
        Invoke("GroundMagma",3.0f);
        
        yield return new WaitForSeconds(2.5f);
        
        StageCameraController.SwitchMainCamera();
        anim.Play("sideroar_3");

        yield return new WaitForSeconds(1.5f);
        
        //_voiceController?.BroadCastMyVoice(0);

        anim.Play("sideroar_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    
    public IEnumerator DB02_Action10()
    {
        yield return _canAction;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB02_Action10");

        anim.Play("blast_1");
        
        yield return new WaitForSeconds(0.7f);
        
        _voiceController.BroadCastMyVoice(4);
        MemoryMaribelle();
        
        anim.Play("blast_3");

        yield return new WaitForSeconds(1f);

        anim.Play("blast_5");

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    

    private void ClawAttack()
    {
        var proj = InstantiateMeele(GetProjectileOfFormatName("action01"),
            transform.position + new Vector3(ac.facedir * 3, 3), InitContainer(true));
        proj.GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.2f);
    }

    private void Meltdown()
    {
        var groundPos = BattleStageManager.Instance.mapBorderB;
        var container = InitContainer(false, 2);
        var projL = InstantiateRanged(GetProjectileOfFormatName("action02_L"),
            new Vector3(transform.position.x-1, groundPos), container, 1);
        var projR = InstantiateRanged(GetProjectileOfFormatName("action02_R"),
            new Vector3(transform.position.x+1, groundPos), container, 1);
        var burnAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72, 12, 1);
        projL.GetComponent<AttackFromEnemy>().AddWithConditionAll(burnAffliction,100);
        projR.GetComponent<AttackFromEnemy>().AddWithConditionAll(burnAffliction,100);
        projL.GetComponent<AttackFromEnemy>().AddMeeleTimeStopEffect(0.18f);
        projR.GetComponent<AttackFromEnemy>().AddMeeleTimeStopEffect(0.18f);
    }

    private void InfernoMove(Vector2 lockedPosition)
    {
        var targetPosition = lockedPosition + new Vector2(-6 * ac.facedir, 2);
        if (targetPosition.x < transform.position.x && ac.facedir > 0)
        {
            targetPosition.x = transform.position.x;
        }
        else if (targetPosition.x > transform.position.x && ac.facedir < 0)
        {
            targetPosition.x = transform.position.x;
        }

        _tweener = transform.DOMove(targetPosition, 0.3f).SetEase(Ease.InOutSine);

        if (_behavior.difficulty <= 2)
        {
            _tweener.OnComplete(() =>
            {
                var atkPos = transform.position + new Vector3(ac.facedir * 5, -0.85f);
                EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, atkPos, RangedAttackFXLayer.transform,
                    7, Vector2.zero, false, true, 0.25f, 0.1f, 0.3f);
            });
        }
        
    }
    
    private Vector2 InfernoAttack()
    {
        var atkPos = transform.position + new Vector3(ac.facedir * 5, -0.85f);
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
            atkPos, InitContainer(false),ac.facedir);
        proj.GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.15f);

        return atkPos;
    }

    private void DashAttack(float dashTime)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_3"),
            transform.position + new Vector3(ac.facedir*3.5f, 3.5f), InitContainer(false), ac.facedir);

        var endPos = transform.position + new Vector3(ac.facedir * 16, 0);
        endPos = BattleStageManager.Instance.OutOfRangeCheck(endPos);
        var distance = Mathf.Abs(endPos.x-transform.position.x);
        dashTime = (distance / 16f) * dashTime;
        dashTime = Mathf.Clamp(dashTime,0.05f, 1f);

        _tweener = transform.DOMoveX(endPos.x, dashTime).SetEase(Ease.InOutSine);
        var rigid = proj.GetComponent<Rigidbody2D>();
        rigid.DOMoveX(endPos.x, dashTime).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            rigid.GetComponent<Collider2D>().enabled = false;
        });
        proj.GetComponent<AttackBase>().AddMeeleTimeStopEffect(0.25f);

    }

    private void FireballMuzzle()
    {
        Instantiate(GetProjectileOfFormatName("action04_3"),
            transform.position +
            new Vector3(upperFirePointOffset.x * ac.facedir, upperFirePointOffset.y), Quaternion.identity,
            MeeleAttackFXLayer.transform);
    }
    
    private void Fireball(float totalLockDelay, float fireDelayAfterLock, GameObject container)
    {
        // 1. 先等待，直到要锁定的那一刻
        DOVirtual.DelayedCall(totalLockDelay, () =>
        {
            // 2. 锁定位置
            float lockedPosX = _behavior.targetPlayer.transform.position.x;
            var prefab = GetProjectileOfFormatName("action04_1");

            // 3. 再等待，然后发射
            DOVirtual.DelayedCall(fireDelayAfterLock, () =>
            {
                SpawnFireball(container, prefab, lockedPosX);
            }, false);
        }, false);
    }

    /*private GameObject CallFireball(float delay, GameObject container=null)
    {
        var prefab = GetProjectileOfFormatName("action04_1");
        if(container == null) 
            container = InitContainer(false, 10);

        var position = _behavior.targetPlayer.transform.position.x;
        
        DOVirtual.DelayedCall(delay, () =>
        {
            Fireball(container,prefab,position);
        },false);

        return container;
    }*/
    
    
    

    private void SpawnFireball(GameObject container, GameObject prefab, float posX)
    {
        InstantiateRanged(prefab, new Vector3(posX,
            BattleStageManager.Instance.mapBorderB + 24), container, 1);
        print("posX:" + posX);
    }
    
    /// <returns>Container generated</returns>
    private GameObject BreatheMuzzle(Vector2 pos)
    {
        var container = InitContainer(false, 2);
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_1"), 
            pos, container, ac.facedir);
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Burn,72,12,1),
            100);
        return container;
    }

    private void BreathGroundFlame(GameObject container, Vector2 pos)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_2"), 
            pos, container, ac.facedir);
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Burn,72,12,1),
            100);
    }

    private (GameObject hintL,GameObject hintR) GenerateWhirlwindHint(float hintTime)
    {
        var posY = BattleStageManager.Instance.mapBorderB;
        var posX1 = transform.position.x + 3;
        var posX2 = transform.position.x - 3;

        var hintL = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, new Vector2(posX1, posY),
            RangedAttackFXLayer.transform, new Vector2(9, 4), Vector2.zero, true, 1,
            hintTime, 90);
        var hintR = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, new Vector2(posX2, posY),
            RangedAttackFXLayer.transform, new Vector2(9, 4), Vector2.zero, true, 1,
            hintTime, 90);

        return (hintL, hintR);
    }

    private void GenerateWhirlwind(Vector2 posL,Vector2 posR)
    {
        var prefab = GetProjectileOfFormatName("action06_1", true);

        var container = InitContainer(false, 2);
        var proj1 = InstantiateRanged(prefab, posL, container, 1);
        var proj2 = InstantiateRanged(prefab, posR, container, 1);

        var wanderingComp1 = proj1.GetComponent<WandingProjectile>();
        var wanderingComp2 = proj2.GetComponent<WandingProjectile>();

        DOVirtual.DelayedCall(0.8f, () =>
        {
            wanderingComp1.SetWandingEdges(BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);
            wanderingComp1.SetFiredir(1);
            wanderingComp1.SetVelocity(new Vector2(4.5f, 0));

            wanderingComp2.SetWandingEdges(BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);
            wanderingComp2.SetFiredir(-1);
            wanderingComp2.SetVelocity(new Vector2(4.5f, 0));
        }, false);



    }

    private float CapturePlayerPositionX()
    {
        return _behavior.targetPlayer.transform.position.x;
    }

    private Vector2[] CalculateChasePositionsAndSpawnHint(float pos0, float pos1, float pos2, float interval,
        float hintTime = 2)
    {
        float v1 = (pos1 - pos0) / interval;
        float v2 = (pos2 - pos1) / interval;

        float borderOffset = 1.5f;
        float minSpacing = 4;
        float maxSpacing = 10;

        // 异常速度裁剪（位移技能保护）
        ActorController playerAc = _behavior.targetPlayer.GetComponent<ActorController>();
        float maxSpeed = playerAc.movespeed;
        v1 = Mathf.Clamp(v1, -maxSpeed, maxSpeed);
        v2 = Mathf.Clamp(v2, -maxSpeed, maxSpeed);

        // 计算加速度（判断是加速还是减速）
        float accel = v2 - v1;

        // 取最终移动方向
        float moveDir = Mathf.Sign(v2);
        if (Mathf.Abs(moveDir) < 0.1f) 
            moveDir = Mathf.Sign(ac.facedir);

        // 根据【速度+加速度】共同计算间距
        // 速度越大、加速越强 → 间距越大；减速则间距收缩
        float speedFactor = Mathf.Abs(v2) / maxSpeed;
        float accelFactor = Mathf.Clamp(accel / maxSpeed, -interval, interval);
        float finalFactor = Mathf.Clamp01(speedFactor + accelFactor) * 0.85f;
        float spacing = Mathf.Lerp(minSpacing, maxSpacing, finalFactor) ;

        // 第一根：完全不预判，直接锁在最后快照位置
        float currentX = pos2;
        float currentDir = moveDir;

        Vector2[] positions = new Vector2[4];
        
        float y = BattleStageManager.Instance.mapBorderB;
        
        for (int i = 0; i < 4; i++)
        {
            // 记录位置
            positions[i] = new Vector2(currentX, y);

            // 计算下一个位置（最后一根不用算）
            if (i >= 3) break;

            float nextX = currentX + currentDir * spacing;
            bool needReverse = false;

            // --- 修改开始：边界先贴边，下一根再反向 ---
            if (nextX < BattleStageManager.Instance.mapBorderL + borderOffset)
            {
                nextX = BattleStageManager.Instance.mapBorderL + borderOffset;
                needReverse = true;
            }
            else if (nextX > BattleStageManager.Instance.mapBorderR - borderOffset)
            {
                nextX = BattleStageManager.Instance.mapBorderR - borderOffset;
                needReverse = true;
            }

            currentX = nextX;

            if (needReverse)
            {
                currentDir = -currentDir;
            }
        }

        //生成提示条
        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, 
            positions[0] + (moveDir > 0 ? new Vector2(-3,10): new Vector2(3,10)),
            RangedAttackFXLayer.transform,
                new Vector2(6, 20), Vector2.zero, false, 0, hintTime, 
                moveDir > 0 ? 0 : 180, 0.4f,
                true, true);

        return positions;
    }

    private void ChaserPillar(Vector2[] positions, float interval = 2)
    {
        var container = InitContainer(false, 4);
        
        var prefab = GetProjectileOfFormatName("action07_1");

        var proj = InstantiateRanged(prefab, positions[0], container, 1);

        DOVirtual.DelayedCall(1 * interval, () =>
        {
            InstantiateRanged(prefab, positions[1], container, 1);
        }, false);
        
        DOVirtual.DelayedCall(2 * interval, () =>
        {
            InstantiateRanged(prefab, positions[2], container, 1);
        }, false);
        
        DOVirtual.DelayedCall(3 * interval, () =>
        {
            InstantiateRanged(prefab, positions[3], container, 1);
        }, false);

    }

    private void GroundMagma()
    {
        if (_groundMagmaInstance)
        {
            Destroy(_groundMagmaInstance);
            _groundMagmaInstance = null;
        }

        var prefab1 = GetProjectileOfFormatName("action08_1",true);//fire
        var prefab2 = GetProjectileOfFormatName("action08_2",true);//magma

        var container = InitContainer(false,2);

        InstantiateRanged(prefab1, new Vector2(0, BattleStageManager.Instance.mapBorderB - 0.5f),
            container, 1);
        
        _groundMagmaInstance = 
            InstantiateRanged(prefab2, new Vector2(0, BattleStageManager.Instance.mapBorderB),
            container, 1);

    }

    private void AddShield()
    {
        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.Invincible))
        {
            //无敌情况下加buff
        }
        else
        {
            if (_shieldInstance == null)
            {
                _shieldInstance = Instantiate(GetProjectileOfFormatName("action09_1",true),
                    transform.position, Quaternion.identity,BuffFXLayer.transform);
            }
            else
            {
                _shieldInstance.SetActive(true);
            }

            var buff = new TimerBuff((int)BasicCalculation.BattleCondition.Invincible, 1, -1, 1);
            buff.dispellable = false;

            _statusManager.ObtainTimerBuff(buff);

        }
    }

    private GameObject SpawnFireballMinion(int hp)
    {
        
        
        
    }

    private void MemoryMaribelle()
    {
        
        float[] xPositions = new float[] { -18f, -9f, 0f, 9f, 18f };
        
        int[][] yPatterns = new int[][] {
            new int[] { 3, 10, 3, 10, 3 },
            new int[] { 10, 3, 10, 3, 10 }
        };
        
        int[][] prefabSequences = new int[][] {
            new int[] { 2, 1, 3, 1, 2 }, // 21312
            new int[] { 3, 1, 2, 1, 3 }, // 31213
            new int[] { 1, 2, 1, 3, 1 }, // 12131
            new int[] { 1, 3, 1, 2, 1 }  // 13121
        };
        
        Dictionary<int, (GameObject prefab, float radius)> prefabMap = new Dictionary<int, (GameObject, float)> {
            { 1, (GetProjectileOfFormatName("action10_1", true), 7f) },
            { 2, (GetProjectileOfFormatName("action10_2", true), 6f) },
            { 3, (GetProjectileOfFormatName("action10_3", true), 6f) }
        };
        
        int randomSeqIndex = Random.Range(0, prefabSequences.Length);
        int randomYIndex = Random.Range(0, yPatterns.Length);
        int[] selectedSequence = prefabSequences[randomSeqIndex];
        int[] selectedY = yPatterns[randomYIndex];

        float[] delays = new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

        delays = delays.Shuffle().ToArray();
        
        var fillTime = 1.2f;
        
        GameObject container = InitContainer(false, 5);
        var defDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff, 5, 15, 100);
        
        for (int i = 0; i < 5; i++)
        {
            // 获取当前预制体ID和配置
            int prefabId = selectedSequence[i];
            var (prefab, radius) = prefabMap[prefabId];
    
            // 计算坐标
            Vector3 spawnPos = new Vector3(xPositions[i], selectedY[i], 0f);

            // --- 生成攻击提示圈 ---
            EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(
                ac, spawnPos, RangedAttackFXLayer.transform,
                radius, Vector2.zero, false,
                doscale: true,
                fillTime, 0.1f, 0.5f, true,false);
            

            DOVirtual.DelayedCall(fillTime + delays[i], () =>
            {
                var proj = InstantiateRanged(
                    prefab: prefab,
                    position: spawnPos,
                    container: container,
                    facedir: 1,
                    rotateMode: 1
                );
                
                proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(defDebuff,100);
                
            }, false);
            
        }
        
        
        
    }


}
