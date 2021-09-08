using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace learning_aspnetcore_react_backend_web_api.Models
{
    public class QuestionPutRequest
    {
        [StringLength(100)]
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
