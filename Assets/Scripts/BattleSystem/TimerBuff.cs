public class TimerBuff : BattleCondition
    {
        public float lastTime { private set; get; }

        protected float duration; //duration is -1 means no time limit.

        #region Constructors

        public TimerBuff(int buffID, float effect)
        {
            this.buffID = buffID;
            this.duration = -1;
            this.effect = effect;
            this.DisplayType = buffEffectDisplayType.Value;
            this.dispellable = true;
        }
        
        public TimerBuff(int buffID, float effect, bool dispellable)
        {
            this.buffID = buffID;
            this.duration = -1;
            this.effect = effect;
            this.DisplayType = buffEffectDisplayType.Value;
            this.dispellable = dispellable;
        }

        public TimerBuff(int buffID, float effect, float duration)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = buffEffectDisplayType.Value;
            this.dispellable = true;
        }

        public TimerBuff(int buffID, float effect, float duration, buffEffectDisplayType type)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = type;
        }
        
        public TimerBuff(int buffID, float effect, float duration, buffEffectDisplayType type, int maxStack)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = type;
            this.maxStackNum = maxStack;
            
        }
        
        public TimerBuff(int buffID, float effect, float duration, int maxStack)
        {
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = buffEffectDisplayType.StackNumber;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
        }
        
        public TimerBuff(int buffID, float effect, float duration, int maxStack, buffEffectDisplayType type)
        {
            //Any buff should not be stacked over limit.
            this.buffID = buffID;
            this.duration = duration;
            this.effect = effect;
            this.DisplayType = type;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
        }

        public TimerBuff(int buffID, float duration, int maxStack, buffEffectDisplayType type)
        {
            //heal
            this.buffID = buffID;
            this.duration = duration;
            
            this.DisplayType = buffEffectDisplayType.StackNumber;
            this.effect = -1;
            this.maxStackNum = maxStack > 100 ? 100 : maxStack;
        }

        #endregion
        
        


        protected override void BuffStart()
        {
            throw new System.NotImplementedException();
        }

        protected override void BuffExpired()
        {
            throw new System.NotImplementedException();
        }

        protected override void BuffDispell()
        {
            throw new System.NotImplementedException();
        }
}
