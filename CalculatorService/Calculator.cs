using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yajat.Digitalizers.Calculator.Service
{
    public class Calculator: Contracts.ICalculator
    {
        public Task<string> Add(int a, int b)
        {
            return Task.FromResult<string>((a + b).ToString());
        }
        public Task<string> Subtract(int a, int b)
        {
            return Task.FromResult<string>((a - b).ToString());
        }
    }
}
