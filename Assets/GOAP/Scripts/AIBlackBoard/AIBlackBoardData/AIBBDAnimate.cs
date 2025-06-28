

namespace MrThaw
{
    public class AIBBDAnimate : AIBlackBoardData
    {
        public AIAnimationDict.AnimationDict Animation { get; private set; }

        public override EnumType.AIBlackBoardKey Key => EnumType.AIBlackBoardKey.Animate;

        public AIBBDAnimate(AIAnimationDict.AnimationDict _animationName)
        {
            Animation = _animationName;
        }
    }
}