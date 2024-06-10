namespace Commons.ServiceDefaults.Options
{
    public class AuditLogStoreOptions
    {
        public const string Option = "AuditLogStore";
        public string ConnectionString { get; set; }
        public bool Enabled { get; set; }
        public string TableName { get; set; }
    }
}
