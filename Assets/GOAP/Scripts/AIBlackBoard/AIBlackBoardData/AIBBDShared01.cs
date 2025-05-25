namespace MrThaw
{
    public class AIBBDPatrolRoute : AIBlackBoardData
    {
        public PatrolRoute PatrolRoute { get; private set; }

        public AIBBDPatrolRoute(PatrolRoute _patrolRoute)
        {
            PatrolRoute = _patrolRoute;
        }
    }

    public class AIBBDOverallDamageToBodyConfidence : AIBlackBoardData
    {
        public float damageMeter { get; set; }
    }
}