using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Library
{
    /*
     * 主要移植的程式碼檔案：
     * - OpenAIChatResponseFormatBuilder
     *   https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Connectors/Connectors.OpenAI/Helpers/OpenAIChatResponseFormatBuilder.cs
     * - KernelJsonSchemaBuilder
     *   https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/InternalUtilities/src/Schema/KernelJsonSchemaBuilder.cs
     */
    public static class ResponseFormatBuilder
    {
        public static object? CreateResponseFormat(string name, string schema)
        {
            return OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(name, BinaryData.FromString(schema), jsonSchemaIsStrict: true);
        }
        
        public static string GetJsonSchema(Type formatObjectType, string? description = null)
        {
            var type = Nullable.GetUnderlyingType(formatObjectType) ?? formatObjectType;
            var schema = AIJsonUtilities.CreateJsonSchema(
                type,
                description: string.IsNullOrEmpty(description) ? null : description,
                serializerOptions: GetDefaultJsonSerializerOptions(),
                inferenceOptions: GetJsonSchemaCreateOptions()
            );
            return schema.GetRawText();
        }

        #region private methods

        /// <summary>
        /// Returns a type name concatenated with generic argument type names if they exist.
        /// </summary>
        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType) return type.Name;

            // 處理泛型的型別。特別是像匿名型別的名稱中會包含 ` 字元。
            var baseName = type.Name[..type.Name.IndexOf('`')];
            // 取得泛型參數的所有屬性型別，並串聯它們的名稱
            var typeArguments = type.GetGenericArguments();
            var argumentNames = string.Concat(Array.ConvertAll(typeArguments, GetTypeName));
            // 回傳組合後的名稱，藉此呈現完整且幾乎唯一的泛型型別名稱
            return $"{baseName}{argumentNames}";
        }

        private static JsonSerializerOptions? s_options;

        /// <summary>
        /// Returns a default instance of <see cref="JsonSerializerOptions"/> configured with a <see cref="DefaultJsonTypeInfoResolver"/> and a <see cref="JsonStringEnumConverter"/>.
        /// </summary>
        /// <returns>A read-only <see cref="JsonSerializerOptions"/> instance configured with default settings.</returns>
        [RequiresUnreferencedCode("Uses JsonStringEnumConverter and DefaultJsonTypeInfoResolver classes, making it incompatible with AOT scenarios.")]
        [RequiresDynamicCode("Uses JsonStringEnumConverter and DefaultJsonTypeInfoResolver classes, making it incompatible with AOT scenarios.")]
        private static JsonSerializerOptions GetDefaultJsonSerializerOptions()
        {
            if (s_options is not null) return s_options;

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Converters = { new JsonStringEnumConverter() },
            };
            options.MakeReadOnly();
            s_options = options;

            return s_options;
        }

        private static AIJsonSchemaCreateOptions? s_schemaOptions;

        /// <summary>
        /// Returns a <see cref="AIJsonSchemaCreateOptions"/> for JSON schema format for structured outputs.
        /// </summary>
        private static AIJsonSchemaCreateOptions GetJsonSchemaCreateOptions()
        {
            if (s_schemaOptions is not null) return s_schemaOptions;

            var schemaOptions = new AIJsonSchemaCreateOptions
            {
                TransformOptions = new()
                {
                    DisallowAdditionalProperties = true,
                    RequireAllProperties = true,
                    MoveDefaultKeywordToDescription = true,
                }
            };
            s_schemaOptions = schemaOptions;

            return s_schemaOptions;
        }

        #endregion
    }
}
