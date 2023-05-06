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
            }
            else
                return buffEffectDisplayType.Value;

            
        }



        public override void BuffStart()
        {
            
        }

        public override void BuffExpired()
        {
            
            throw new System.NotImplementedException();
        }

        public override void BuffDispell()
        {
            throw new System.NotImplementedException();
        }

        public override Sprite GetIcon()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UI/InBattle/BuffIcons/Icons/Icon_Buff_");
            string id = this.buffID.ToString();
            
            if(extra_iconID > 0)
                id = extra_iconID.ToString();
            
            sb.Append(id);

            var sprite = Resources.Load<Sprite>(sb.ToString());
            if (sprite != null)
                return sprite;
            else return null;

        }
    }
