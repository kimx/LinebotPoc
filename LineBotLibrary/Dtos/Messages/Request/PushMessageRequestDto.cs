using System;
using System.Text.Json.Serialization;

namespace LineBotLibrary.Dtos
{
    public class PushMessageRequestDto<T>
    {
        [JsonPropertyName("to")]
        public string UserId { get; set; }
        public List<T> Messages { get; set; }
    }
}

