using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using learning_aspnetcore_react_backend_web_api.Models;

namespace learning_aspnetcore_react_backend_web_api.Data
{
    public interface IQuestionCache
    {
        QuestionGetSingleResponse Get(int questionId);
        void Remove(int questionId);
        void Set(QuestionGetSingleResponse question);

    }
}
