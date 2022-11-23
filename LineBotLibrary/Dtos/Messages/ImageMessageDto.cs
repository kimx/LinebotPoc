using LineBotLibrary.Enum;
using LineBotMessage.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineBotLibrary.Dtos.Messages
{
    public class ImageMessageDto : BaseMessageDto
    {
        public ImageMessageDto()
        {
            Type = MessageTypeEnum.Image;
        }

        public string OriginalContentUrl { get; set; }
        public string PreviewImageUrl { get; set; }
    }
}
