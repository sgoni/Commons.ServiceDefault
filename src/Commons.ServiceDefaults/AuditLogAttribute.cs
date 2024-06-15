namespace Commons.ServiceDefaults
{
    /// <summary>
    /// Attribute to determine which IRequest should be audited
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AuditLogAttribute : Attribute { }
}
