﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using learning_aspnetcore_react_backend_web_api.Data;
using learning_aspnetcore_react_backend_web_api.Models;

namespace learning_aspnetcore_react_backend_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public QuestionsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions()
        {
            var questions = _dataRepository.GetQuestions();
            return questions;
        }
    }
}
