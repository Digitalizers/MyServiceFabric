using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yajat.Digitalizers.ExpressionProcessor.Contracts
{
    public interface IExpressionProcessor: IService
    {
        Task<string> EvaluateExpressionAsync(string expression);
        Task<List<string>> ParseExpressionAsync(string expression);
    }
}
