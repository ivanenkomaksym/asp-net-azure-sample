namespace AspNetAzureSample.Configuration
{
    public class StorageOptions
    {
        public static readonly string Name = "Storage";

        public enum StorageTypes
        {
            /// <summary>
            /// Will configure the context to connect to a named in-memory database.
            /// </summary>
            InMemory = 0,

            /// <summary>
            /// Will configure the context used to connect to a MySQL database.
            /// </summary>
            MySql = 1,

            /// <summary>
            /// Will configure the context used to connect to a SQL Server database.
            /// </summary>
            SqlServer = 2
        }

        /// <summary>
        /// Storage type that should be used.
        /// </summary>
        public StorageTypes? StorageType { get; set; }

        /// <summary>
        /// Should be present if <see cref="StorageType"/> == <see cref="StorageTypes.MySql"/>
        /// </summary>
        public string? MySqlConnection { get; set; }

        /// <summary>
        /// Should be present if <see cref="StorageType"/> == <see cref="StorageTypes.SqlServer"/>
        /// </summary>
        public string? SqlServerConnection { get; set; }
    }
}
