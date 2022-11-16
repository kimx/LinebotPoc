using LineBotLibrary.Enum;
using LineBotMessage.Dtos;

namespace LineBotLibrary.Dtos
{
    public class TemplateMessageDto<T> : BaseMessageDto
    {
        public TemplateMessageDto()
        {
            Type = MessageTypeEnum.Template;
        }

        public string AltText { get; set; }
        public T Template { get; set; }
    }

    public class ButtonsTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.Buttons;
        public string Text { get; set; }
        public List<ActionDto>? Actions { get; set; }

        public string? ThumbnailImageUrl { get; set; }
        public string? ImageAspectRatio { get; set; }
        public string? ImageSize { get; set; }
        public string? ImageBackgroundColor { get; set; }
        public string? Title { get; set; }
        public string? DefaultAction { get; set; }
    }

    public class ConfirmTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.Confirm;
        public string Text { get; set; }
        public List<ActionDto>? Actions { get; set; }
    }

    public class CarouselTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.Carousel;
        public List<CarouselColumnObjectDto> Columns { get; set; }

        public string ImageAspectRatio { get; set; }
        public string ImageSize { get; set; }
    }

    public class CarouselColumnObjectDto
    {
        public string Text { get; set; }
        public List<ActionDto> Actions { get; set; }

        public string? ThumbnailImageUrl { get; set; }
        public string? ImageBackgroundColor { get; set; }
        public string? Title { get; set; }
        public ActionDto? DefaultAction { get; set; }
    }

    public class ImageCarouselTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.ImageCarousel;
        public List<ImageCarouselColumnObjectDto> Columns { get; set; }
    }

    public class ImageCarouselColumnObjectDto
    {
        public string ImageUrl { get; set; }
        public ActionDto Action { get; set; }

    }
}

