using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using learning_aspnetcore_react_backend_web_api.Models;

namespace learning_aspnetcore_react_backend_web_api.Data
{
    public interface IDataRepository

    {
        IEnumerable<QuestionGetManyResponse> GetQuestions();
        IEnumerable<QuestionGetManyResponse>
            GetQuestionsBySearch(string search);
        IEnumerable<QuestionGetManyResponse>
            GetUnansweredQuestions();
        QuestionGetSingleResponse GetQuestion(int questionId);
        IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers();
        bool QuestionExists(int questionId);
        AnswerGetResponse GetAnswer(int answerId);
        QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(AnswerPostFullRequest answer);

    }
}
