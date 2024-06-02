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
            Persistent = 1
        }

        /// <summary>
        /// Storage type that should be used.
        /// </summary>
        public StorageTypes? StorageType { get; set; }

        /// <summary>
        /// Should be present if <see cref="StorageType"/> == <see cref="StorageTypes.Persistent"/>
        /// </summary>
        public string? SqlConnection { get; set; }
    }
}
