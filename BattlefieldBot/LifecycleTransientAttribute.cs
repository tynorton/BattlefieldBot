namespace BattlefieldBot
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class LifecycleTransientAttribute : System.Attribute
    {
        public double Version;

        public LifecycleTransientAttribute()
        {
            Version = 1.0;
        }
    }
}
