namespace LineBotLibrary.Enum
{
    public static class MessageTypeEnum
    {
        public const string Text = "text";
        public const string Sticker = "sticker";
        public const string Image = "image";
        public const string Video = "video";
        public const string Audio = "audio";
        public const string Location = "location";
        public const string Imagemap = "imagemap";
        public const string Template = "template";
        public const string Flex = "flex";
        public const string FlexBubble = "flexBubble";
        public const string FlexCarousel = "flexCarousel";
    }

    public static class ActionTypeEnum
    {
        public const string Postback = "postback";
        public const string Message = "message";
        public const string Uri = "uri";
        public const string DatetimePicker = "datetimepicker";
        public const string Camera = "camera";
        public const string CameraRoll = "cameraRoll";
        public const string Location = "location";
        public const string RichMenuSwitch = "richmenuswitch";
    }

    public static class PostbackInputOptionEnum
    {
        public const string CloseRichMenu = "closeRichMenu";
        public const string OpenRichMenu = "openRichMenu";
        public const string OpenKeyboard = "openKeyboard";
        public const string OpenVoice = "openVoice";
    }

    public static class DatetimePickerModeEnum
    {
        public const string Date = "date";
        public const string Time = "time";
        public const string Datetime = "datetime";
    }

    public static class TemplateTypeEnum
    {
        public const string Buttons = "buttons";
        public const string Confirm = "confirm";
        public const string Carousel = "carousel";
        public const string ImageCarousel = "image_carousel";
    }

    public static class TemplateImageAspectRatioEnum
    {
        public const string Rectangle = "rectangle";
        public const string Square = "square";
    }

    public static class TemplateImageSizeEnum
    {
        public const string Cover = "cover";
        public const string Contain = "contain";
    }

    public static class FlexContainerTypeEnum
    {
        public const string Bubble = "bubble";
        public const string Carousel = "carousel";
    }
}

