using System.Text.Json;

namespace JsonMode
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
            => System.Text.Json.JsonSerializer.Deserialize<T>(str) ?? default!;

        /// <summary>
        /// 將物件轉成 JSON 字串
        /// </summary>
        /// <typeparam name="T">物件類型</typeparam>
        /// <param name="model">物件</param>
        /// <returns>JSON 字串</returns>
        public static string ToJson<T>(this T model)
            => System.Text.Json.JsonSerializer.Serialize(model);

        public static string ToIndented(this string str)
        {
            // 解析 JSON 並重新序列化為縮排格式
            using JsonDocument doc = JsonDocument.Parse(str);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var formattedJson = JsonSerializer.Serialize(doc.RootElement, options);

            return formattedJson;
        }

        public static string ToJsonSchema(this Type type, string? description = null)
            => ResponseFormatBuilder.GetJsonSchema(type, description);
    }
}
