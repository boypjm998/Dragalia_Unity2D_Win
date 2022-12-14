using System.Text;
using UnityEngine;


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
            sb.Append("UI/General/BuffIcons/Icons/Icon_Buff_");
            string id = this.buffID.ToString();
            sb.Append(id);

            var sprite = Resources.Load<Sprite>(sb.ToString());
            if (sprite != null)
                return sprite;
            else return null;

        }
    }
