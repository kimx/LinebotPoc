using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineBotLibrary.Dtos.Profile
{
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Language { get; set; }
        public string PictureUrl { get; set; }
    }
}
