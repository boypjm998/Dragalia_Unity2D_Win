using System;
using System.Text;
using UnityEngine;

[Serializable]
public class TimerBuff : BattleCondition
    {
        

        #region Constructors
        
        
        public TimerBuff(int buffID, float effect, float duration, buffEffectDisplayType type, int maxStack)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = type;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
            this.lastTime = duration;

            if (buffID > 100 && buffID <= 200)
                dispellable = false;
            

        }
        
        public TimerBuff(int buffID, float effect, float duration, buffEffectDisplayType type, int maxStack, int spID)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = type;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
            this.lastTime = duration;
            this.specialID = spID;
            
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
            sb.Append(id);

            var sprite = Resources.Load<Sprite>(sb.ToString());
            if (sprite != null)
                return sprite;
            else return null;

        }
    }
