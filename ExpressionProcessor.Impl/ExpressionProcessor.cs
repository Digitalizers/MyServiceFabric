using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yajat.Digitalizers.ExpressionProcessor.Contracts;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using Irony.Parsing;
using XLParser;
using System.Linq;

namespace Yajat.Digitalizers.ExpressionProcessor.Impl
{
    public class ExpressionProcessor : IExpressionProcessor
    {
        private readonly ILogger<IExpressionProcessor> _Logger;
        public ExpressionProcessor(ILogger<IExpressionProcessor> logger)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> EvaluateExpressionAsync(string expression)
        {
            _Logger.LogInformation($@"{nameof(EvaluateExpressionAsync)} invoked");
            //return Task.FromResult(false);
            return await Task.Run(() =>
            {
                return XLWorkbook.EvaluateExpr(expression).ToString().Replace(",", ".");
            });
        }

        public async Task<List<string>> ParseExpressionAsync(string expression)
        {
            _Logger.LogInformation($@"{nameof(ParseExpressionAsync)} invoked");
            //return Task.FromResult(false);
            //return Task.FromResult($"\r\n        UserAccess.CreateUserAsync -> {email} -> {DateTime.UtcNow}");
            var parsedFormula = ParseExpression(expression);
            return await Task.Run(() => GetListOfVariablesInExpression(parsedFormula).ToList());
        }

        public ParseTreeNode ParseExpression(string expression)
        {
            return ExcelFormulaParser.Parse(expression);
        }

        public IEnumerable<string> GetListOfVariablesInExpression(ParseTreeNode parseTreeNode)
        {
            var tokens = parseTreeNode.AllNodes().Where(x => IsVariable(x.Term.Name)).Select(x => x.Token.Text);
            return tokens.Distinct();
        }

        private bool IsVariable(string tokenType)
        {
            switch (tokenType)
            {
                case "NameToken":
                    return true;
                case "CellToken":
                    return true;
                case "NamedRangeCombinationToken":
                    return true;
                default:
                    return false;
            }
        }
    }
}
