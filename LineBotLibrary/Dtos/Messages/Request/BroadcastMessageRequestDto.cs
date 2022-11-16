using System;

namespace LineBotLibrary.Dtos
{
    public class BroadcastMessageRequestDto<T>
    {
        public List<T> Messages { get; set; }
        public bool? NotificationDisabled { get; set; }
    }
}

