using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TALab1
{
    class ErrorHandler
    {
        public string errorHandler(ref string str)
        {
            try
            {
                if(str == null || str == "")
                {
                    return null;
                }
                str = str.Replace(".", ",");
                if(str.Length == 1 && str[0] == '-')
                    throw new Exception("Входная строка имела неверный формат, отсутствует левый операнд.");
                if (str[0] == '+' || str[0] == '*' || str[0] == '^' || str[0] == '/')
                    throw new Exception("Входная строка имела неверный формат, отсутствует левый операнд.");
                Regex regex = new Regex(@"[^log\d(),+\-*/^.; ]");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, неопределенные буквы или спецсимволы).");
                regex = new Regex(@"([+*/\^-]{1}[+*/\^-]+)|[,]{2,}");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, 2 подряд идущих символа операций.");
                regex = new Regex(@"[+*/\^-]{1}[)]{1}|[(]{1}[+*\^/]{1}");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, " +
                        "математический знак перед закрывающей или после открывающей скобки.");
                regex = new Regex(@"[log;]");
                if (regex.IsMatch(str))
                {
                    regex = new Regex(@"log");
                    if (regex.IsMatch(str))
                    {
                        regex = new Regex(@".*log[(][^;]+;[^;]+[)]");
                        if (!regex.IsMatch(str))
                            throw new Exception("Входная строка имела неверный формат,ошибка ввода логарифма.\nПример: log(100;100)");
                    }
                    else
                        throw new Exception("Входная строка имела неверный формат,неопределенные буквы или спецсимволы).");
                }
                regex = new Regex(@"\d \d");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, пробел между цельным числом");
                regex = new Regex(@"\(;\)");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, ';'в скобках");
                regex = new Regex(@"\d*\,\d*\,");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, в числе 2 запятые");
                regex = new Regex(@" {3,}");
                if (regex.IsMatch(str))
                    throw new Exception("Больше 2 пробелов. \nЭто все-таки калькулятор, выражение должно быть читаемым");

                regex = new Regex(@"\( * \)");
                if (str.Contains("()") || str.Contains(")(") || regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат,пустые скобки");
                regex = new Regex(@"\)\d|\d\(");
                if (regex.IsMatch(str))
                    throw new Exception("Входная строка имела неверный формат, число перед открывающей " +
                        "или после закрывающей скобки");
                if (str.Contains("(-")) 
                {
                    string newstr;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == '-' && i != 0 && str[i-1] == '(')
                        {
                            newstr = "";
                            for (int j = 0; j < i; j++)
                                newstr += str[j];
                            newstr += "0";
                            for (int j = i; j < str.Length; j++)
                                newstr += str[j];
                            str = newstr;
                        }
                    }
                }
                else if(str[0] == '-')
                {
                    string newstr = "0" + str;
                    str = newstr;
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
