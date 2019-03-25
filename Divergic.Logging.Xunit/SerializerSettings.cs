namespace Divergic.Logging.Xunit
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The <see cref="SerializerSettings" />
    ///     class provides access to settings for JSON serialization.
    /// </summary>
    public static class SerializerSettings
    {
        private static JsonSerializerSettings BuildSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return settings;
        }

        /// <summary>
        ///     Gets the default serializer settings.
        /// </summary>
        public static JsonSerializerSettings Default { get; } = BuildSerializerSettings();
    }
}