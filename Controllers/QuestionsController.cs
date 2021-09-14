using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using learning_aspnetcore_react_backend_web_api.Data;
using learning_aspnetcore_react_backend_web_api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace learning_aspnetcore_react_backend_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IQuestionCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _auth0UserInfo;

        public QuestionsController(IDataRepository dataRepository, IQuestionCache questionCache, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _dataRepository = dataRepository;
            _cache = questionCache;
            _clientFactory = clientFactory;
            _auth0UserInfo = $"{configuration["Auth0:Authority"]}userinfo";
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search, bool includeAnswers, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeAnswers)
                {
                    return _dataRepository.GetQuestionsWithAnswers();
                }
                else
                {
                    return _dataRepository.GetQuestions();
                }
            }
            else
            {
                return

                    _dataRepository.GetQuestionsBySearchWithPaging(

                        search,

                        page,

                        pageSize

                    );
            }
        }

        [HttpGet("unanswered")]

        public async Task<IEnumerable<QuestionGetManyResponse>>

            GetUnansweredQuestions()

        {

            return await _dataRepository.

                GetUnansweredQuestionsAsync();

        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            var question = _cache.Get(questionId);

            if (question == null)

            {

                question =

                 _dataRepository.GetQuestion(questionId);

                if (question == null)

                {

                    return NotFound();

                }

                _cache.Set(question);

            }

            return question;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<QuestionGetSingleResponse>> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            var savedQuestion = _dataRepository.PostQuestion(new
                QuestionPostFullRequest
                {
                    Title = questionPostRequest.Title,
                    Content = questionPostRequest.Content,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    UserName = await GetUserName(),
                    Created = DateTime.UtcNow
                });
            return CreatedAtAction(nameof(GetQuestion), new { questionId = savedQuestion.QuestionId }, savedQuestion);
        }

        [Authorize(Policy = "MustBeQuestionAuthor")]
        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ? question.Title : questionPutRequest.Title;
            questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ? question.Content : questionPutRequest.Content;
            var savedQuestion = _dataRepository.PutQuestion(questionId, questionPutRequest);
            _cache.Remove(savedQuestion.QuestionId);
            return savedQuestion;
        }

        [Authorize(Policy = "MustBeQuestionAuthor")]
        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            _dataRepository.DeleteQuestion(questionId);
            _cache.Remove(questionId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{questionId}/answer")]
        public async Task<ActionResult<AnswerGetResponse>> PostAnswer(int questionId, AnswerPostRequest answerPostRequest)
        {
            answerPostRequest.QuestionId = questionId;
            var questionExists= _dataRepository.QuestionExists(questionId);
            if (!questionExists)
            {
                return NotFound();
            }
            var savedAnswer =_dataRepository.PostAnswer(new AnswerPostFullRequest
                {
                    QuestionId = answerPostRequest.QuestionId.Value,
                    Content = answerPostRequest.Content,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    UserName = await GetUserName(),
                    Created = DateTime.UtcNow
                });
            _cache.Remove(answerPostRequest.QuestionId.Value);
            return savedAnswer;
        }

        private async Task<string> GetUserName()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                _auth0UserInfo);
            request.Headers.Add(
                "Authorization",
                Request.Headers["Authorization"].First());
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var user =
                    JsonSerializer.Deserialize<User>(
                        jsonContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                return user.Name;
            }
            else
            {
                return "";
            }
        }
    }
}
