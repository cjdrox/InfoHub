using System.Text.RegularExpressions;

namespace InfoHub.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Write("Infohub started\n\n>>");
            string command;

            while ( !IsQuitCommand(command = System.Console.ReadLine()) )
            {
                System.Console.WriteLine(CommandResult(command));
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
                    result = EvaluateExpression( command );
                    break;
            }

            return result;
        }

        private static string EvaluateExpression(string expression)
        {
            const string space = @"[\s]*?";
            const string sign = @"[+-]{0,1}";
            const string Operator = @"[+*/-]";
            const string literal = @"\d+[%]{0,1}";

            const string operand = space + sign + space + literal + space;
            const string simpleExpression = @"^" + operand + "(" + Operator + operand + ")*$";

            if(Regex.IsMatch(expression, simpleExpression))
            {
                var op1 = Regex.Match(expression, @"^" + operand).ToString();
                var rem = expression.Remove(expression.IndexOf(op1, System.StringComparison.Ordinal), op1.Length);
                
                //System.Console.Write("rem is: " + rem);
                var op = Regex.Match(rem, Operator).ToString();

                var rem2 = rem.Remove(rem.IndexOf(op, System.StringComparison.Ordinal), op.Length);
                var op2 = Regex.Match(rem2, operand + "$").ToString();

                return Evaluate(op1, op2, op);
            }

            return "invalid expression";
        }

        private static string Evaluate(string op1, string op2, string op)
        {
            double operand1, operand2;
            double.TryParse(op1, out operand1);
            double.TryParse(op2, out operand2);

            switch (op)
            {
                case "+":
                    return (operand1 + operand2).ToString();

                case "-":
                    return (operand1 - operand2).ToString();

                case "/":
                    return (operand1 / operand2).ToString();

                case "*":
                    return (operand1 * operand2).ToString();
            }

            return "Invalid expression";
        }

        static bool IsQuitCommand(string command)
        {
            return command != null && (command.ToLower().Equals("quit") || command.ToLower().Equals("exit"));
        }
    }
}
