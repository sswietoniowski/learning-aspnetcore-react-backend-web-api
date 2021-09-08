﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace learning_aspnetcore_react_backend_web_api.Models
{
    public class AnswerPostFullRequest
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
