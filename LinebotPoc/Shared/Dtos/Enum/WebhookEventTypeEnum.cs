namespace LinebotPoc.Shared.Enum
{
    public static class WebhookEventTypeEnum
    {
        public const string Message = "message";
        public const string Unsend = "unsend";
        public const string Follow = "follow";
        public const string Unfollow = "unfollow";
        public const string Join = "join" ;
        public const string Leave = "leave";
        public const string MemberJoined = "memberJoined";
        public const string MemberLeft = "memberLeft";
        public const string Postback = "postback";
        public const string VideoPlayComplete = "videoPlayComplete";

        //Link 相關為Kim擴充
        public const string AccountLink = "accountLink";
    }
}