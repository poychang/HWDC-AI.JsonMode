using System.Text.Encodings.Web;
using System.Text.Json;

namespace Library
{
    public static class JsonExtension
    {
        /// <summary>
        /// 將 JSON 字串轉成物件
        /// </summary>
        /// <typeparam name="T">目標物件類型</typeparam>
        /// <param name="str">JSON 字串</param>
        /// <returns></returns>
        public static T ToModel<T>(this string str)
            => JsonSerializer.Deserialize<T>(str) ?? default!;

        /// <summary>
        /// 將物件轉成 JSON 字串
        /// </summary>
        /// <typeparam name="T">物件類型</typeparam>
        /// <param name="model">物件</param>
        /// <returns>JSON 字串</returns>
        public static string ToJson<T>(this T model)
            => JsonSerializer.Serialize(model);

        private static readonly JsonSerializerOptions optionsWriteIndented = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };

        private static readonly JsonSerializerOptions optionsWriteMinify = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
        };

        private static string ToJsonFormat(this string str, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.Parse(str);
            var formattedJson = JsonSerializer.Serialize(doc.RootElement, options);
            return formattedJson;
        }

        public static string ToIndented(this string str) => str.ToJsonFormat(optionsWriteIndented);

        public static string ToMinify(this string str) => str.ToJsonFormat(optionsWriteMinify);

        public static string ToJsonSchema(this Type type, string? description = null)
            => ResponseFormatBuilder.GetJsonSchema(type, description);
    }
}
