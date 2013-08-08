using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using InfoHub.Evaluator.Operators;
using InfoHub.Evaluator.Tokens;

namespace InfoHub.Evaluator
{
    public class Parser
    {
        string _pstr;
        int _ptr;

        readonly Queue<Token> _tokenQueue = new Queue<Token>();
        readonly Stack<OpToken> _opStack = new Stack<OpToken>();
        OperatorCollection _operators;
        private TypeRegistry _typeRegistry;

        public TypeRegistry TypeRegistry
        {
            get { return _typeRegistry; }
            set { _typeRegistry = value; }
        }

        public string StringToParse { get { return _pstr; } set { _pstr = value; _tokenQueue.Clear(); } }

        public Parser()
        {
            Initialize();
        }

        public Parser(string str)
        {
            Initialize();
            _pstr = str;
        }

        void Initialize()
        {
            _operators = new OperatorCollection
                {
                    {".", new MethodOperator(".", 12, true, OperatorCustomExpressions.MemberAccess)},
                    {"!", new UnaryOperator("!", 11, false, Expression.Not)},
                    {"*", new BinaryOperator("*", 10, true, Expression.Multiply)},
                    {"/", new BinaryOperator("/", 10, true, Expression.Divide)},
                    {"%", new BinaryOperator("%", 10, true, Expression.Modulo)},
                    {"+", new BinaryOperator("+", 9, true, OperatorCustomExpressions.Add)},
                    {"-", new BinaryOperator("-", 9, true, Expression.Subtract)},
                    {"<<", new BinaryOperator("<<", 8, true, Expression.LeftShift)},
                    {">>", new BinaryOperator(">>", 8, true, Expression.RightShift)},
                    {"<", new BinaryOperator("<", 7, true, Expression.LessThan)},
                    {">", new BinaryOperator(">", 7, true, Expression.GreaterThan)},
                    {"<=", new BinaryOperator("<=", 7, true, Expression.LessThanOrEqual)},
                    {">=", new BinaryOperator(">=", 7, true, Expression.GreaterThanOrEqual)},
                    {"==", new BinaryOperator("==", 6, true, Expression.Equal)},
                    {"!=", new BinaryOperator("!=", 6, true, Expression.NotEqual)},
                    {"&", new BinaryOperator("&", 5, true, Expression.And)},
                    {"^", new BinaryOperator("^", 4, true, Expression.ExclusiveOr)},
                    {"|", new BinaryOperator("|", 3, true, Expression.Or)},
                    {"&&", new BinaryOperator("&&", 2, true, Expression.AndAlso)},
                    {"||", new BinaryOperator("||", 1, true, Expression.OrElse)}
                };

            //operators.Add("^", new BinaryOperator("^", 11, false, Expression.Power));

            //operators.Add("[", new IndexOperator("[", 0, true, OperatorCustomExpressions.ArrayAccess));

        }

        /// <summary>
        /// Returns a boolean specifying if the current string pointer is within the bounds of the expression string
        /// </summary>
        /// <returns></returns>
        private bool IsInBounds()
        {
            return _ptr < _pstr.Length;
        }

        /// <summary>
        /// Parses the expression and builds the token queue for compiling
        /// </summary>
        public void Parse()
        {
            try
            {
                _tokenQueue.Clear();
                _ptr = 0;

                while (IsInBounds())
                {
                    var lastptr = _ptr;

                    if (_pstr[_ptr] != ' ')
                    {
                        // Parse enclosed strings
                        switch (_pstr[_ptr])
                        {
                            case '\'':
                                {
                                    var isStringClosed = false;
                                    _ptr++;
                                    lastptr = _ptr;
                                    var tokenbuilder = new StringBuilder();

                                    // check for escaped single-quote and backslash
                                    while (IsInBounds())
                                    {
                                        if (_pstr[_ptr] == '\\')
                                        {
                                            tokenbuilder.Append(_pstr.Substring(lastptr, _ptr - lastptr));
                                            var nextchar = _pstr[_ptr + 1];
                                            switch (nextchar)
                                            {
                                                case '\'':
                                                case '\\':
                                                    tokenbuilder.Append(nextchar);
                                                    break;
                                                default:
                                                    throw new Exception("Unrecognized escape sequence");
                                            }
                                            _ptr++;
                                            _ptr++;
                                            lastptr = _ptr;
                                        }
                                        else if ((_pstr[_ptr] == '\''))
                                        {
                                            isStringClosed = true;
                                            break;
                                        }
                                        else
                                        {
                                            _ptr++;
                                        }
                                    }

                                    if (!isStringClosed) throw new Exception("Unclosed string literal at " + lastptr);

                                    tokenbuilder.Append(_pstr.Substring(lastptr, _ptr - lastptr));
                                    string token = tokenbuilder.ToString();
                                    _tokenQueue.Enqueue(new Token { Value = token, IsIdent = true, Type = typeof(string) });
                                    _ptr++;
                                }
                                break;
                            case '#':
                                {
                                    bool isDateClosed = false;

                                    _ptr++;
                                    lastptr = _ptr;

                                    while (IsInBounds())
                                    {
                                        _ptr++;
                                        if (_pstr[_ptr] == '#')
                                        {
                                            isDateClosed = true;
                                            break;
                                        }
                                    }

                                    if (!isDateClosed) throw new Exception("Unclosed date literal at " + lastptr);

                                    var token = _pstr.Substring(lastptr, _ptr - lastptr);

                                    var dt = DateTime.Parse(token);
                                    _tokenQueue.Enqueue(new Token { Value = dt, IsIdent = true, Type = typeof(DateTime) });
                                    _ptr++;

                                }
                                break;
                            case ',':
                                {
                                    var pe = false;


                                    while (_opStack.Count > 0)
                                    {
                                        if ((string)_opStack.Peek().Value == "(")
                                        {
                                            var temp = _opStack.Pop();
                                            Token lastToken = _opStack.Peek();
                                            if (lastToken.GetType() == typeof(MemberToken))
                                            {
                                                var lastmember = (MemberToken)lastToken;
                                                lastmember.ArgCount++;
                                            }
                                            _opStack.Push(temp);
                                            pe = true;
                                            break;
                                        }
                                        var popToken = _opStack.Pop();
                                        _tokenQueue.Enqueue(popToken);
                                    }


                                    if (!pe)
                                    {
                                        throw new Exception("Parenthesis mismatch");
                                    }

                                    _ptr++;
                                }
                                break;
                            case '.':
                                if (_opStack.Count > 0)
                                {
                                    OpToken sc = _opStack.Peek();
                                    // if the last operator was also a Member accessor pop it on the tokenQueue
                                    if ((string)sc.Value == ".")
                                    {
                                        var popToken = _opStack.Pop();
                                        _tokenQueue.Enqueue(popToken);
                                    }
                                }
                                _opStack.Push(new MemberToken());
                                _ptr++;
                                break;
                            default:
                                if (HelperMethods.IsHexStart(_pstr, _ptr))
                                {
                                    var isNeg = false;
                                    if (_pstr[_ptr] == '-')
                                    {
                                        isNeg = true;
                                        _ptr++;
                                        lastptr = _ptr;
                                    }
                                    //skip 0x
                                    _ptr += 2;
                                    // Number identifiers start with a number and may contain numbers and decimals
                                    while (IsInBounds() && (HelperMethods.IsHex(_pstr, _ptr) || _pstr[_ptr] == 'L'))
                                    {
                                        _ptr++;
                                    }

                                    var token = _pstr.Substring(lastptr, _ptr - lastptr);

                                    var ntype = typeof(Int32);
                                    object val = null;

                                    if (token.EndsWith("L"))
                                    {
                                        ntype = typeof(Int64);
                                        token = token.Remove(token.Length - 1, 1);
                                    }

                                    switch (ntype.Name)
                                    {
                                        case "Int32":
                                            val = isNeg ? -Convert.ToInt32(token, 16) : Convert.ToInt32(token, 16);
                                            break;
                                        case "Int64":
                                            val = isNeg ? -Convert.ToInt64(token, 16) : Convert.ToInt64(token, 16);
                                            break;
                                    }

                                    _tokenQueue.Enqueue(new Token { Value = val, IsIdent = true, Type = ntype });
                                }
                                    // Parse numbers
                                else if (HelperMethods.IsANumber(_pstr, _ptr))
                                {
                                    // Number identifiers start with a number and may contain numbers and decimals
                                    while (IsInBounds() && (HelperMethods.IsANumber(_pstr, _ptr) || _pstr[_ptr] == '.' || _pstr[_ptr] == 'd' || _pstr[_ptr] == 'f' || _pstr[_ptr] == 'L'))
                                    {
                                        _ptr++;
                                    }

                                    var token = _pstr.Substring(lastptr, _ptr - lastptr);

                                    var ntype = typeof(Int32);
                                    object val = null;

                                    if (token.Contains('.')) ntype = typeof(Double);
                                    if (token.EndsWith("d") || token.EndsWith("f") || token.EndsWith("L"))
                                    {
                                        if (token.EndsWith("d")) ntype = typeof(Double);
                                        if (token.EndsWith("f")) ntype = typeof(Single);
                                        if (token.EndsWith("L")) ntype = typeof(Int64);
                                        token = token.Remove(token.Length - 1, 1);
                                    }

                                    switch (ntype.Name)
                                    {
                                        case "Int32":
                                            val = int.Parse(token);
                                            break;
                                        case "Int64":
                                            val = long.Parse(token);
                                            break;
                                        case "Double":
                                            val = double.Parse(token);
                                            break;
                                        case "Single":
                                            val = float.Parse(token);
                                            break;
                                    }


                                    _tokenQueue.Enqueue(new Token { Value = val, IsIdent = true, Type = ntype });
                                }
                                    // Test for identifier
                                else if (HelperMethods.IsAlpha(_pstr[_ptr]) || (_pstr[_ptr] == '_'))
                                {
                                    _ptr++;

                                    while (IsInBounds() && (HelperMethods.IsAlpha(_pstr[_ptr]) || HelperMethods.IsNumeric(_pstr, _ptr)))
                                    {
                                        _ptr++;
                                    }


                                    var token = _pstr.Substring(lastptr, _ptr - lastptr);
                                    MemberToken mToken = null;

                                    if (_opStack.Count > 0)
                                    {
                                        var opToken = _opStack.Peek();
                                        if (opToken.GetType() == typeof(MemberToken))
                                            mToken = (MemberToken)opToken;
                                    }

                                    if ((mToken != null) && (mToken.Name == null))
                                    {
                                        mToken.Name = token;
                                    }
                                    else if (_typeRegistry.ContainsKey(token))
                                    {
                                        _tokenQueue.Enqueue(_typeRegistry[token].GetType().Name == "RuntimeType"
                                                                ? new Token
                                                                      {
                                                                          Value = ((Type) _typeRegistry[token]).UnderlyingSystemType,
                                                                          IsType = true
                                                                      }
                                                                : new Token
                                                                      {
                                                                          Value = _typeRegistry[token], IsType = true
                                                                      });
                                    }
                                    else
                                    {
                                        switch (token.ToLower())
                                        {
                                            case "null":
                                                _tokenQueue.Enqueue(new Token
                                                                        {
                                                                            Value = null, 
                                                                            IsIdent = true, 
                                                                            Type = typeof(object)
                                                                        });
                                                break;
                                            case "false":
                                            case "true":
                                                _tokenQueue.Enqueue(new Token
                                                                        {
                                                                            Value = Boolean.Parse(token), 
                                                                            IsIdent = true, 
                                                                            Type = typeof(Boolean)
                                                                        });
                                                break;
                                            default:
                                                throw new Exception(string.Format("Unknown type or identifier '{0}'", token));
                                        }
                                    }
                                }
                                else if (_pstr[_ptr] == '[')
                                {
                                    _opStack.Push(new OpToken { Value = "[", Ptr = _ptr + 1 });
                                    _ptr++;
                                }
                                else if (_pstr[_ptr] == ']')
                                {
                                    var pe = false;
                                    // Until the token at the top of the stack is a left bracket,
                                    // pop operators off the stack onto the output queue
                                    while (_opStack.Count > 0)
                                    {
                                        OpToken sc = _opStack.Peek();
                                        if ((string)sc.Value == "[")
                                        {
                                            OpToken temp = _opStack.Pop();
                                            if (_opStack.Count > 0)
                                            {
                                                Token lastToken = _opStack.Peek();
                                                if (lastToken.GetType() == typeof(MemberToken))
                                                {
                                                    var lastmember = (MemberToken)lastToken;
                                                    // check if there was anything significant between the opening paren and the closing paren
                                                    // If so, then we have an argument... This isn't the best approach perhaps, but it works...
                                                    if (_pstr.Substring(sc.Ptr, _ptr - sc.Ptr).Trim().Length > 0) lastmember.ArgCount++;
                                                }
                                            }
                                            _opStack.Push(temp);
                                            pe = true;
                                            break;
                                        }
                                        OpToken popToken = _opStack.Pop();
                                        _tokenQueue.Enqueue(popToken);
                                    }

                                    // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                                    if (!pe)
                                    {
                                        throw new Exception("Parenthesis mismatch");
                                    }

                                    // Pop the left parenthesis from the stack, but not onto the output queue.
                                    _opStack.Pop();
                                    //tokenQueue.Enqueue(lopToken);


                                    _ptr++;
                                }
                                else if (_pstr[_ptr] == '(')
                                {
                                    int curptr = _ptr;
                                    while (_pstr[curptr] != ')')
                                    {
                                        curptr++;
                                    }
                                    string typeName = _pstr.Substring(lastptr + 1, curptr - lastptr - 1).Trim();
                                    Type t;
                                    if (_typeRegistry.ContainsKey(typeName))
                                    {
                                        _tokenQueue.Enqueue(new Token { Value = "(" + typeName + ")", IsCast = true, Type = (Type)_typeRegistry[typeName] });
                                        _ptr = curptr + 1;
                                    }
                                    else if ((t = Type.GetType(typeName)) != null)
                                    {
                                        _tokenQueue.Enqueue(new Token { Value = "(" + t.Name + ")", IsCast = true, Type = t });
                                        _ptr = curptr + 1;
                                    }
                                    else
                                    {
                                        _opStack.Push(new OpToken { Value = "(", Ptr = _ptr + 1 });
                                        _ptr++;
                                    }

                                }
                                else if (_pstr[_ptr] == ')')
                                {
                                    bool pe = false;
                                    //int poppedtokens = 0;
                                    // Until the token at the top of the stack is a left parenthesis,
                                    // pop operators off the stack onto the output queue
                                    while (_opStack.Count > 0)
                                    {
                                        OpToken sc = _opStack.Peek();
                                        if ((string)sc.Value == "(")
                                        {
                                            OpToken temp = _opStack.Pop();
                                            if (_opStack.Count > 0)
                                            {
                                                Token lastToken = _opStack.Peek();
                                                if (lastToken.GetType() == typeof(MemberToken))
                                                {
                                                    var lastmember = (MemberToken)lastToken;
                                                    // check if there was anything significant between the opening paren and the closing paren
                                                    // If so, then we have an argument... This isn't the best approach perhaps, but it works...
                                                    if (_pstr.Substring(sc.Ptr, _ptr - sc.Ptr).Trim().Length > 0) lastmember.ArgCount++;
                                                }
                                            }
                                            _opStack.Push(temp);
                                            pe = true;
                                            break;
                                        }
                                        OpToken popToken = _opStack.Pop();
                                        _tokenQueue.Enqueue(popToken);
                                        // poppedtokens++;
                                    }

                                    // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                                    if (!pe)
                                    {
                                        throw new Exception("Parenthesis mismatch");
                                    }

                                    // Pop the left parenthesis from the stack, but not onto the output queue.
                                    _opStack.Pop();

                                    //If the token at the top of the stack is a function token, pop it onto the output queue.
                                    if (_opStack.Count > 0)
                                    {
                                        OpToken popToken = _opStack.Peek();
                                        if ((string)popToken.Value == ".")
                                        {
                                            popToken = _opStack.Pop();
                                            _tokenQueue.Enqueue(popToken);
                                        }
                                    }
                                    _ptr++;
                                }
                                else
                                {
                                    string op;
                                    if ((op = _operators.IsOperator(_pstr, ref _ptr)) != null)
                                    {
                                        while (_opStack.Count > 0)
                                        {
                                            var sc = _opStack.Peek();

                                            if (!_operators.IsOperator((string) sc.Value) ||
                                                ((!_operators[op].LeftAssoc ||
                                                  (_operators[op].Precedence > _operators[(string) sc.Value].Precedence)) &&
                                                 (_operators[op].Precedence >= _operators[(string) sc.Value].Precedence)))
                                            {
                                                break;
                                            }
                                            var popToken = _opStack.Pop();
                                            _tokenQueue.Enqueue(popToken);
                                        }

                                        _opStack.Push(new OpToken { Value = op });
                                        _ptr++;
                                    }
                                    else
                                    {
                                        throw new Exception("Unexpected token '" + _pstr[_ptr].ToString(CultureInfo.InvariantCulture) + "'");
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        _ptr++;
                    }
                }

                while (_opStack.Count > 0)
                {
                    OpToken sc = _opStack.Peek();
                    if ((string)sc.Value == "(" || (string)sc.Value == ")")
                    {
                        throw new Exception("Paren mismatch");
                    }

                    sc = _opStack.Pop();
                    _tokenQueue.Enqueue(sc);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Parser error at position {0}: {1}", _ptr, ex.Message), ex);
            }
        }

        /// <summary>
        /// Builds the expression tree from the token queue
        /// </summary>
        /// <returns></returns>
        public Expression BuildTree()
        {
            if (_tokenQueue.Count == 0) Parse();

            // make a copy of the queue, so that we don't empty the original queue
            var tempQueue = new Queue<Token>(_tokenQueue);
            var exprStack = new Stack<Expression>();
            var args = new List<Expression>();

#if DEBUG
            var q = tempQueue.Select(x => x.Value.ToString() + (x.GetType() == typeof(MemberToken) ? ":" + ((MemberToken)x).Name : ""));
            System.Diagnostics.Debug.WriteLine(string.Join("][", q.ToArray()));
#endif
            var isCastPending = -1;
            Type typeCast = null;

            while (tempQueue.Count > 0)
            {
                var t = tempQueue.Dequeue();

                if (isCastPending > -1) isCastPending--;
                if (isCastPending == 0)
                {
                    exprStack.Push(Expression.Convert(exprStack.Pop(), typeCast));
                    isCastPending = -1;
                }

                if (t.IsIdent)
                {
                    // handle numeric literals
                    exprStack.Push(Expression.Constant(t.Value, t.Type));
                }
                else if (t.IsType)
                {
                    exprStack.Push(Expression.Constant(t.Value));
                }
                else if (t.IsOperator)
                {
                    // handle operators
                    var op = _operators[(string)t.Value];
                    var opfunc = OpFuncServiceLocator.Resolve(op.GetType());
                    for (var i = 0; i < t.ArgCount; i++)
                    {
                        args.Add(exprStack.Pop());
                    }
                    // Arguments are in reverse order
                    args.Reverse();
                    var result = opfunc(new OpFuncArgs
                                            {
                                                TempQueue = tempQueue, 
                                                ExprStack = exprStack, 
                                                T = t, 
                                                Op = op, 
                                                Args = args
                                            });
                    args.Clear();
                    exprStack.Push(result);
                }
                else if (t.IsCast)
                {
                    isCastPending = 2;
                    typeCast = t.Type;
                }
            }

            // we should only have one complete expression on the stack, otherwise, something went wrong
            if (exprStack.Count == 1)
            {
                var pop = exprStack.Pop();
#if DEBUG
                System.Diagnostics.Debug.WriteLine(pop.ToString());
#endif
                return pop;
            }
            throw new Exception("Invalid expression");
        }

    }
}
