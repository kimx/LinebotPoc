using System;
using System.Text.Json.Serialization;

namespace LineBotLibrary.Dtos
{
    public class MulticastMessageRequestDto<T>
    {
        [JsonPropertyName("to")]
        public List<string> UserIds { get; set; }
        public List<T> Messages { get; set; }
    }
}

