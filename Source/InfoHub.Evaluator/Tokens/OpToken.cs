namespace InfoHub.Evaluator.Tokens
{
    internal class OpToken : Token
    {
        public OpToken()
        {
            IsOperator = true;
            ArgCount = 0;
        }
    }
}