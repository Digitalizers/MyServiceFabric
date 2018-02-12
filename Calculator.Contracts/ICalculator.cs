namespace Yajat.Digitalizers.Calculator.Contracts
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    public interface ICalculator : IService
    {
        Task<string> Add(int a, int b);
        Task<string> Subtract(int a, int b);
    }
}