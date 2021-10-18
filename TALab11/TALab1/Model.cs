using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TALab1
{
    delegate double Operation(double first, double second);
    class Model
    {
        private List<string> standart_operators =
            new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^", "log", ";" });
        private List<string> str_mod;
        private Stack<string> buf;
        private List<string> postfix_notation;
        private bool isError;

        public string Rezult(string str)
        {
            isError = false;
            buf = new Stack<string>();
            postfix_notation = new List<string>();
            str_mod = new List<string>();
            string newstr;
            ErrorHandler error = new ErrorHandler();
            newstr = error.errorHandler(ref str);
            if (newstr != null)
                return newstr;
            if (str != null)
                Parsing(str);
            newstr = OPN();
            if (isError == false)
                return MathCalc();
            return newstr;
        }

        private string Parsing(string str)
        {
            string newstr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetterOrDigit(str[i]) || str[i] == '.' || str[i] == ',')
                    newstr += str[i];
                else if (standart_operators.Contains(str[i].ToString()))
                {
                    if (newstr.Length != 0)
                        str_mod.Add(newstr);
                    newstr = "";
                    str_mod.Add(str[i].ToString());
                }
            }
            if (newstr.Length != 0)
                str_mod.Add(newstr);
            return ToString(str_mod);
        }

        private string OPN()
        {
            try
            {
                foreach (var elem in str_mod)
                {
                    if (standart_operators.Contains(elem))
                    {

                        if (buf.Count == 0)
                            buf.Push(elem);

                        else if (elem == ")" || elem == ";")
                        {
                            while (buf.Peek() != "(")
                            {
                                postfix_notation.Add(buf.Pop());
                                if (buf.Count == 0)
                                    throw new Exception("Отсутствует открывающая скобка");
                            }
                            if(elem == ")")
                                buf.Pop();
                        }

                        else if (elem == "(")
                            buf.Push(elem);

                        else if (Priority(buf.Peek()) >= Priority(elem))
                        {
                            while (Priority(buf.Peek()) >= Priority(elem))
                            {
                                postfix_notation.Add(buf.Pop());
                                if (buf.Count == 0)
                                    break;
                            }
                            buf.Push(elem);
                        }
                        else if (Priority(buf.Peek()) < Priority(elem))
                        {
                            buf.Push(elem);
                        }
                    }
                    else
                    {
                        postfix_notation.Add(elem);
                    }
                }
                int k = buf.Count;
                for (int i = 0; i < k; i++)
                {
                    if(buf.Peek() == "(")
                        throw new Exception("Отсутствует закрывающая скобка");
                    postfix_notation.Add(buf.Pop());
                }

                return ToString(postfix_notation);
            }
            catch (Exception e)
            {
                isError = true;
                return e.Message;
            }
        }
        private string MathCalc()
        {
            try
            {
                Stack<string> rezult = new Stack<string>();
                double first;
                double second;
                string rezultstr = "";
                int counter = 0;
                foreach (var elem in postfix_notation)
                {
                    switch (elem)
                    {
                        case "+":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                Operation operation = (x, y) => x + y;
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + first + " + " + second + " = " + rezult.Peek() + "\n";
                            }
                            break;
                        case "-":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                Operation operation = (x, y) => x - y;
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + first + " - " + second + " = " + rezult.Peek() + "\n";
                            }
                            break;
                        case "*":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                Operation operation = (x, y) => x * y;
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + first + " * " + second + " = " + rezult.Peek() + "\n";
                            }
                            break;
                        case "/":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                if (second == 0)
                                    throw new Exception("Попытка деления на 0");
                                Operation operation = (x, y) => x / y;
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + first + " / " + second + " = " + rezult.Peek() + "\n";
                            }
                            break;
                        case "^":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                Operation operation = (x, y) => Math.Pow(x, y);
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + first + " ^ " + second + " = " + rezult.Peek() + "\n";
                            }
                            break;
                        case "log":
                            if (rezult.Count > 1)
                            {
                                second = Convert.ToDouble(rezult.Pop());
                                first = Convert.ToDouble(rezult.Pop());
                                if (second == 1)
                                    throw new Exception("Основание логарифма не может быть равным 1");
                                Operation operation = (x, y) => Math.Log(x,y);
                                rezult.Push(Convert.ToString(operation(first, second)));
                                rezultstr += ++counter + ": " + " log(" + first + ";" + second + ") = " + rezult.Peek() + "\n";
                            }
                            break;
                        default:
                            rezult.Push(elem);
                            break;

                    }
                }
                if (rezult.Count == 0)
                    return null;
                if (rezult.Peek() == ")")
                    throw new Exception("Отсутствует открывающая скобка");
                return rezultstr + "Ответ: " + rezult.Peek();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private byte Priority(string elem)
        {
            switch (elem)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                case "log":
                    return 4;
                default:
                    return 5;
            }
        }

        private string ToString(List<string> lst)
        {
            string ret = "";
            foreach(string elem in lst)
            {
                ret += elem + "\n";
            }
            return ret;
        }
    }
}
