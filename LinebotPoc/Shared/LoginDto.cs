using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinebotPoc.Shared
{
    public class LoginDto
    {
        [Required]
        public string UserEmail { get; set; }
    }
}
