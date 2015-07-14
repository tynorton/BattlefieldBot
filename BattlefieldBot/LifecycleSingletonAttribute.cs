namespace BattlefieldBot
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class LifecycleSingletonAttribute : System.Attribute
    {
        public double Version;

        public LifecycleSingletonAttribute()
        {
            Version = 1.0;
        }
    }
}
