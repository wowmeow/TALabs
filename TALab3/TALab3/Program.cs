using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TALab3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Storage storage = new Storage(@"C:\Users\Acer\Desktop\Laba3\test1.txt");
                storage.ShowInfo();
                while (true)
                {
                    Console.WriteLine("Введите строку: ");
                    var str = Console.ReadLine();
                    storage.check_line(str);
                    Console.WriteLine();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class Link
    {
        public char S; // состояние
        public string inp; // оставшаяся часть входной ленты
        public string stack; // состояние магазина на данный момент
        public int index; // индекс возможного значения функции
        public bool term; // можно ли менять выбор ветви на данном ходе

        public Link(char s, string p, string h, bool t)
        {
            S = s;
            inp = p;
            stack = h;
            index = -1;
            term = t;
        }
        public Link(char s, string p, string h)
        {
            S = s;
            inp = p;
            stack = h;
            index = -1;
            term = false;
        }
    }
    public class Fargs
    {
        public string state; // состояние
        public string token; // символ со входной ленты
        public string mgtoken; // магазинный символ

        public Fargs(string s, string p, string h) 
        {
            state = s;
            token = p;
            mgtoken = h;
        }
    }
    public class Value
    {
        public string state;     // состояние
        public string chain;   // заносимая цепочка

        public Value(string s, string c)
        {
            state = s;
            chain = c;
        }
    }
    public class Command
    {
        public Fargs fargs;
        public List<Value> values;

        public Command(Fargs f, List<Value> v)
        {
            fargs = f;
            values = v;
        }
    }
    public class Storage
    {
        SortedSet<char> Term = new SortedSet<char>(); // терминальные символы
        SortedSet<char> NoneTerm = new SortedSet<char>(); // нетерминальные символы
        char s0 = '0', h0 = '|';
        List<Command> commands = new List<Command>();
        List<Link> chain = new List<Link>(); // цепочка конфигураций магазинного автомата, полученная в процессе его работы

        public Storage(string file)
        {
            Regex regex = new Regex(@"([A-Z])>(.+)");
            
            var fileText = File.ReadAllLines(file);
            foreach (var item in fileText)
            {
                if (!regex.IsMatch(item) || item[item.Length - 1] == '|' || item[2] == '|')
                    throw new Exception("Не удалось распознать содержимое файла\n");
                else
                {
                    var match = regex.Match(item).Groups;
                    NoneTerm.Add(match[1].ToString()[0]);
                    commands.Add(new Command(new Fargs(s0.ToString(), "", match[1].ToString()[0].ToString()), new List<Value>()));
                    commands.Last().values.Add(new Value(s0.ToString(), ""));
                    for (int i = 0; i < match[2].ToString().Length; i++)
                    {
                        if (match[2].ToString()[i] == '|')
                        {
                            if (match[2].ToString()[i - 1] != '|')
                                commands.Last().values.Add(new Value(s0.ToString(), ""));
                        }
                        else
                        {
                            Term.Add(match[2].ToString()[i]);
                            commands.Last().values.Last().chain += match[2].ToString()[i];
                        }
                    }
                    for (int i = 0; i < commands.Last().values.Count; i++)
                        commands.Last().values[i].chain = new string(commands.Last().values[i].chain.Reverse().ToArray());
                }
            }
            foreach (var c in NoneTerm)
                Term.Remove(c);
            var buf = new List<Value>();
            foreach (var c in Term) 
            {
                buf = new List<Value>();
                buf.Add(new Value(s0.ToString(), ""));
                commands.Add(new Command(new Fargs(s0.ToString(), c.ToString(), c.ToString()), buf));
            }
            buf = new List<Value>();
            buf.Add(new Value(s0.ToString(), ""));
            commands.Add(new Command(new Fargs(s0.ToString(), "", h0.ToString()), buf));
        }
        public void ShowInfo()
        {
            Console.Write("Входной алфавит:\nTerm = {");
            foreach(var item in Term)
		        Console.Write(item + ", ");
            Console.Write("\b\b}\n\n");
            Console.Write("Алфавит магазинных символов:\nZ = {");
            foreach(var item in NoneTerm)
			    Console.Write(item + ", ");
            foreach(var item in Term)
			    Console.Write(item + ", ");
            Console.Write("h0}\n\n");

            Console.Write("Список команд:\n");
            foreach(var item in commands)
		    {
                Console.Write("f(s" + item.fargs.state +", ");
                if (item.fargs.token == "")
                    Console.Write("lambda");
                else
                    Console.Write(item.fargs.token);
                Console.Write(", ");
                if (item.fargs.mgtoken == h0.ToString())
                    Console.Write("h0");
                else
                    Console.Write(item.fargs.mgtoken);
                Console.Write(") = {");
                foreach(Value v in item.values)
                {
                    Console.Write("(s" +v.state +", ");
                    if (v.chain == "")
                        Console.Write("lambda");
                    else
                        Console.Write(v.chain);
                    Console.Write("); ");

                }
                Console.Write("\b\b}\n");
            }
            Console.WriteLine();
        }
        public void showChain()
        {
            Console.Write("\nЦепочка конфигураций: \n");
            foreach (var link in chain)
                Console.Write("(s" + link.S + ", " + ((link.inp.Length == 0) ? "lambda" : link.inp) + ", h0" + link.stack + ") |– ");
            Console.WriteLine("(s0, lambda, lambda)");
        }
        public bool push_link()
        {
            int ch_size = chain.Count;
            int mag_size, j, i;
            for (i = 0; i < commands.Count; i++)
            {
                mag_size = chain[ch_size - 1].stack.Length;
                if (chain[chain.Count - 1].inp.Length != 0 && chain[chain.Count - 1].stack.Length != 0 && chain[ch_size - 1].S.ToString() == commands[i].fargs.state && (chain[ch_size - 1].inp[0].ToString() == commands[i].fargs.token || "" == commands[i].fargs.token) && chain[ch_size - 1].stack[mag_size - 1] == commands[i].fargs.mgtoken[0])
                {
                    for (j = 0; j < commands[i].values.Count; j++)
                    {
                        if (commands[i].fargs.token == "")
                        {
                            chain.Add(new Link(commands[i].values[j].state[0], chain[ch_size - 1].inp,chain[ch_size - 1].stack));
                        }
                        else
                        {
                            chain.Add(new Link(commands[i].values[j].state[0], chain[ch_size - 1].inp, chain[ch_size - 1].stack));
                            chain[ch_size].inp = new string(chain[ch_size].inp.Reverse().ToArray());
                            chain[ch_size].inp = chain[ch_size].inp.Remove(chain[ch_size].inp.Length-1);
                            chain[ch_size].inp = new string(chain[ch_size].inp.Reverse().ToArray());
                        }

                        chain[ch_size].stack = chain[ch_size].stack.Remove(chain[ch_size].stack.Length - 1);
                        chain[ch_size].stack += commands[i].values[j].chain;

                        if (chain[ch_size].inp.Length < chain[ch_size].stack.Length)
                        {
                            chain.RemoveAt(chain.Count - 1);
                            chain.RemoveAt(chain.Count - 1);
                            return false;
                        }
                        else
                        {
                            if (chain[chain.Count - 1].inp.Length == 0 && chain[chain.Count - 1].stack.Length == 0 || push_link())
                                return true;
                        }
                    }
                }
            }
            if (i == commands.Count)
            {
                chain.RemoveAt(chain.Count - 1);
                return false;
            }
            return false;
        }
        public bool check_line(string str)
        {
            if (commands[0].values.Count == 1)
                chain.Add(new Link(s0, str, "", false));
            else
                chain.Add(new Link(s0, str, "", true));

            chain[0].stack += commands[0].fargs.mgtoken;

            bool res = push_link();
            if (res)
            {
                Console.Write("Валидная строка\n");
                showChain();
            }
            else
            {
                Console.Write("Невалидная строка\n");
            }
            chain.Clear();
            return res;
        }
    }
}
