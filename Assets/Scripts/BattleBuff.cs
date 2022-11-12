using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleBuff : MonoBehaviour
{
    protected int buffID;
    protected Sprite buffIcon;
    protected int maxStackNum;
    protected bool dispellable;
    protected buffEffectDisplayType DisplayType;
    protected bool addToTotal; //����buff������50�ӹ��ĵ���ģʽʹ�ú󣬲�������ʾ50�ӹ���


    protected enum buffEffectDisplayType
    {
        Value = 0,
        StackNumber = 1,
        None = 2
    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GetIcon()
    {
        
    }

    protected abstract void OnBuffEnable();

    protected abstract void OnBuffDisable();

}
