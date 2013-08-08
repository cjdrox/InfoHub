using System;
using System.Linq;
using InfoHub.Evaluator;

namespace InfoHub.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Write("Infohub started\n\n>>");
            string command;
            
            while (!IsQuitCommand(command = (args.Any() && !String.IsNullOrEmpty(args[0]) ? 
                args[0] : System.Console.ReadLine())))
            {
                System.Console.WriteLine("={0}", CommandResult(command));
                System.Console.Write(">>");              
            }

            System.Console.WriteLine(">>Bye! (Press any key to Quit)");
            System.Console.ReadLine();
        }

        private static string CommandResult(string command)
        {
            string result;

            switch (command.ToUpper())
            {
                case "HELP":
                    result = "Generic Help method";
                    break;
                default:
                    var parser = new CompiledExpression(command);
                    parser.Parse();
                    parser.Compile();
                    result = parser.Eval().ToString();
                    break;
            }

            return result;
        }

        #region Old Codes

        //private static string EvaluateExpression(string expression)
        //{
        //    const string space = @"[\s]*?";
        //    const string sign = @"[+-]{0,1}";
        //    const string Operator = @"[+*/-]";
        //    const string literal = @"\d+[%]{0,1}";

        //    const string operand = space + sign + space + literal + space;
        //    const string simpleExpression = @"^" + operand + "(" + Operator + operand + ")*$";

        //    if(Regex.IsMatch(expression, simpleExpression))
        //    {
        //        var op1 = Regex.Match(expression, @"^" + operand).ToString();
        //        var rem = expression.Remove(expression.IndexOf(op1, StringComparison.Ordinal), op1.Length);

        //        //System.Console.Write("rem is: " + rem);
        //        var op = Regex.Match(rem, Operator).ToString();

        //        var rem2 = rem.Remove(rem.IndexOf(op, StringComparison.Ordinal), op.Length);
        //        var op2 = Regex.Match(rem2, operand + "$").ToString();

        //        return Evaluate(op1, op2, op);
        //    }

        //    return "invalid expression";
        //}

        //private static string Evaluate(string op1, string op2, string op)
        //{
        //    double operand1, operand2;
        //    double.TryParse(op1, out operand1);
        //    double.TryParse(op2, out operand2);

        //    switch (op)
        //    {
        //        case "+":
        //            return (operand1 + operand2).ToString(CultureInfo.InvariantCulture);

        //        case "-":
        //            return (operand1 - operand2).ToString(CultureInfo.InvariantCulture);

        //        case "/":
        //            return (operand1 / operand2).ToString(CultureInfo.InvariantCulture);

        //        case "*":
        //            return (operand1 * operand2).ToString(CultureInfo.InvariantCulture);
        //    }

        //    return "Invalid expression";
        //}

        #endregion

        static bool IsQuitCommand(string command)
        {
            return command != null && (command.ToLower().Equals("quit") || command.ToLower().Equals("exit"));
        }
    }
}
