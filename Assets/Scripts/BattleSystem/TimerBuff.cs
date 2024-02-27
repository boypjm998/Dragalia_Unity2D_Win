using System;
using System.Linq;
using System.Text;
using GameMechanics;
using UnityEngine;

[Serializable]
public class TimerBuff : BattleCondition
{
    public int extra_iconID = -1;

    

        #region Constructors
        
        
        public TimerBuff(int buffID, float effect, float duration, int maxStack = 100)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = GetDisplayType(buffID);
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
            this.lastTime = duration;
            specialID = -1;

            if (buffID > 100 && buffID <= 200)
                dispellable = false;
            

        }
        
        public TimerBuff(int buffID, float effect, float duration,  int maxStack = 100, int spID = -1)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            //this.DisplayType = type;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
            this.lastTime = duration;
            this.specialID = spID;
            this.DisplayType = GetDisplayType(buffID);
            
            if (buffID > 100 && buffID <= 200)
                dispellable = false;
        }

        /// Dispell
        public TimerBuff(int buffID)
        {
            if (buffID == 999)
                this.buffID = 999;
        }

        public TimerBuff(TimerBuff origin)
        {
            //deep copy
            this.buffID = origin.buffID;
            this.duration = origin.duration;
            this.effect = origin.effect;
            this.DisplayType = origin.DisplayType;
            this.maxStackNum = origin.maxStackNum;
            this.lastTime = origin.duration;
            this.specialID = origin.specialID;
            this.dispellable = origin.dispellable;
            this.extra_iconID = origin.extra_iconID;
            
            
        }

        public TimerBuff()
        {
            this.buffID = 999;
        }

        public TimerBuff(TimerBuff origin, int extraIconID)
        {
            this.buffID = origin.buffID;
            this.duration = origin.duration;
            this.effect = origin.effect;
            this.DisplayType = origin.DisplayType;
            this.maxStackNum = origin.maxStackNum;
            this.lastTime = origin.duration;
            this.specialID = origin.specialID;
            this.dispellable = origin.dispellable;
            this.extra_iconID = extraIconID;
        }




        #endregion

       protected buffEffectDisplayType GetDisplayType(int buffID)
        {
            if (BasicCalculation.conditionsDisplayedByStacknum.Contains(buffID))
            {
                return buffEffectDisplayType.StackNumber;
            }
            else if (BasicCalculation.conditionsDisplayedByLevel.Contains(buffID))
            {
                return buffEffectDisplayType.Level;
            }
            else if(BasicCalculation.conditionDisplayedByExactValue.Contains(buffID))
            {
                return buffEffectDisplayType.ExactValue;
            }else if (buffID == (int)(BasicCalculation.BattleCondition.Energy) ||
                      buffID == (int)(BasicCalculation.BattleCondition.Inspiration))
            {
                return buffEffectDisplayType.EnergyOrInspiration;
            }
            else
                return buffEffectDisplayType.Value;

            
        }

       
       


       

        public override void BuffDispell()
        {
            throw new System.NotImplementedException();
        }

        public override Sprite GetIcon()
        {
            if (buffIcon != null)
                return buffIcon;

            StringBuilder sb = new StringBuilder();
            sb.Append("UI/InBattle/BuffIcons/Icons/Icon_Buff_");
            string id = this.buffID.ToString();
            
            if(extra_iconID > 0)
                id = extra_iconID.ToString();
            
            sb.Append(id);

            var sprite = Resources.Load<Sprite>(sb.ToString());

            buffIcon = sprite;

            if (sprite != null)
                return sprite;
            
            else return null;

        }
    }

[Serializable]
public class AdvancedTimerBuff : TimerBuff
{
    public float effect2 { get; private set; }
    public float effect3 { get; private set; }

    public float effectArg { get; private set; }

    public void SetEffect(int id, float val)
    {
        if(id == 1)
            effect = val;
        else if(id == 2)
            effect2 = val;
        else if(id == 3)
            effect3 = val;
    }

    

    public AdvancedTimerBuff(int buffID, float effect, float effect2, float effect3, float duration,
        int maxStack = 100, int spID = -1, float effectArg = 1)
    {
        this.buffID = buffID;
        this.duration = duration;
        this.effect = effect;
        this.effect2 = effect2;
        this.effectArg = effectArg;
        //this.DisplayType = type;
        this.maxStackNum = maxStack > 100 ? 100 : maxStack;
        this.lastTime = duration;
        this.specialID = spID;
        this.DisplayType = GetDisplayType(buffID);
            
        if (buffID > 100 && buffID <= 200)
            dispellable = false;
    }
}
