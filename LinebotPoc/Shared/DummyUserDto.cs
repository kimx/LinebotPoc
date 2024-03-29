﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LinebotPoc.Shared;
public class DummyUserDto
{
    //Cosmos id要小寫
    public string id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string UserEmail { get; set; }

    public string LineUserId { get; set; }

    //挷定時要比對Line的參數用
    public string LineNonce { get; set; }
}

