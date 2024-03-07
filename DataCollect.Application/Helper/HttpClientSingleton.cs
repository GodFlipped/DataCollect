using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace DataCollect.Application.Helper
{
    public class HttpClientSingleton
    {
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static string _url = "https://example.com/api/resource";
        // 静态的 HttpClient 实例  
        private static readonly HttpClient client = new HttpClient();

        // 私有构造函数，防止外部实例化  
        private HttpClientSingleton()
        {

        }

        public static async Task<ResponseResult> PostAsync(string url, object content)
        {
            // 将内容序列化为 JSON 字符串  
            var jsonContent = JsonConvert.SerializeObject(content);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // 创建HttpRequestMessage并设置其为POST请求  
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://wcs.kuajingmaihuo.com{url}")
                {
                    Content = httpContent
                };

                var timeStamp = DateTimeToLongMillisecondsTimeStamp(DateTime.Now).ToString();
                var randomString = Guid.NewGuid().ToString("N");

                //密钥用于sign加密 正式密钥和设备ID 
                const string devicePwd = "64072600c74774546751d76d9169c40c272f9ac0";
                request.Headers.Add("deviceId", "4272e03e-bb7d-4a71-91d7-60396acb577a");

                //测试密钥和设备ID 
                //var devicePwd = "c13e0ee753e45172b07c84cd774265304ac882f1";
                //request.AddHeader("deviceId", "09a86004-5813-42d9-a4cc-37938e120394");

                //加密用于表头
                var data = url + timeStamp + randomString;
                var headerSign = HmacSha256(devicePwd, data);
                //设置 request用于线程回调的 trackingId
                request.Headers.Add("timestamp", timeStamp);
                request.Headers.Add("random", randomString);
                request.Headers.Add("sign", headerSign);

                // 添加自定义的HTTP头  
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // 发送请求并获取响应  
                var response = await client.SendAsync(request);

                // 确保响应成功  
                response.EnsureSuccessStatusCode();

                // 读取响应内容  
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<ResponseResult>(responseBody);
                    
                return responseModel;
                // 输出响应内容（或根据你的需求处理它）  
                Console.WriteLine(responseBody);

            }
            catch (HttpRequestException e)
            {
                // 处理异常  
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public class ResponseResult
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("errorCode")]
            public int ErrorCode { get; set; }

            [JsonProperty("errorMsg")]
            public string ErrorMsg { get; set; }

            [JsonProperty("result")]
            public string Result { get; set; }
        }

        private static string HmacSha256(string secret, string message)
        {
            string hash = "";
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                hash = ByteArrayToHexString(hashBytes);
            }
            return hash;
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            var hs = new StringBuilder();
            foreach (byte b in bytes)
            {
                hs.Append(b.ToString("x2"));
            }
            return hs.ToString().ToLower();
        }

        private static long DateTimeToLongMillisecondsTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
        }
    }
}
