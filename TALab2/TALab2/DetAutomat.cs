using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TALab2
{
    class DetAutomat
    {
        private string prevState;
        private List<RuleTransition> MatrixTransition = new List<RuleTransition>();
        public DetAutomat(List<string> init)
        {
            int testIndex = 0;
            foreach(var elem in init)
            {
                Regex regex = new Regex(@"^[\t\s\v\n]*\p{L}\d+,.?=\p{L}\d+[\t\s\v\n]*$");
                if (regex.IsMatch(elem))
                {
                    string buf = "";
                    var ruleTransition = new RuleTransition();
                    bool Cflag = false;
                    for(int i = 0; i < elem.Length; i++)
                    {
                        if (Cflag)
                        {
                            ruleTransition.Symbol = elem[i].ToString();
                            Cflag = false;
                        }
                        else if (char.IsLetterOrDigit(elem[i]))
                        {
                            buf += elem[i];
                        }
                        else if (elem[i] == ',' && buf != "")  
                        {
                            ruleTransition.StartCondition = buf;
                            buf = "";
                            Cflag = true;
                        }
                        
                    }
                    ruleTransition.NextCondition = buf;
                    MatrixTransition.Add(ruleTransition);
                    testIndex++;
                }
            }
            if (testIndex != init.Count)
                throw new Exception("Ошибка файла, внутреннее содержимое не соответствует условию валидации входных данных");
        }
        public string Parsing(string checkstr)
        {
            prevState = "q0";
            for(int i = 0; i < checkstr.Length; i++)
            {
                bool flag = false;
                foreach (var elem in MatrixTransition)
                {
                    if (elem.StartCondition == prevState && elem.Symbol == checkstr[i].ToString())
                    {
                        prevState = elem.NextCondition;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                        return "Автомат не может разобрать введенную строку";
            }
            if (prevState[0] == 'f')
                return "Строка разобрана";
            else
                return "Автомат не может разобрать введенную строку";
        }
        public string CheckDet()
        {
            List<string> buf = new List<string>();
            for (int k = 0; k < MatrixTransition.Count; k++)
            {
                if (MatrixTransition[k].Symbol == null)
                    return "автомат недетерменированный, лямбда переходы";
                for(int i = 0; i < MatrixTransition.Count; i++)
                {
                    if (MatrixTransition[k].StartCondition == MatrixTransition[i].StartCondition
                        && MatrixTransition[k].Symbol == MatrixTransition[i].Symbol && i != k)
                        return "автомат недетерменированный";
                }
            }
            return "автомат детерменированный";
        }
        public void NetToDet()
        {
            string strNewVer;
            bool isBreak = false;
            List<string> NewVer = new List<string>();
            List<string> OldVer = new List<string>();
            do
            {
                strNewVer = "";
                isBreak = false;
                foreach (var elemFirst in MatrixTransition)
                {
                    foreach (var elemSecond in MatrixTransition)
                    {
                        if (elemFirst.StartCondition == elemSecond.StartCondition && elemFirst.Symbol == elemSecond.Symbol
                            && elemFirst.NextCondition != elemSecond.NextCondition && elemFirst != elemSecond)
                        {
                            NewVer.Add(elemSecond.NextCondition + elemFirst.NextCondition);
                            OldVer.Add(elemSecond.NextCondition);
                            NewVer.Add(elemSecond.NextCondition + elemFirst.NextCondition);
                            OldVer.Add(elemFirst.NextCondition);

                            elemFirst.NextCondition = NewVer[0];
                            elemSecond.NextCondition = NewVer[0];
                            isBreak = true;
                            break;
                        }
                    }
                    if (isBreak)
                        break;
                }
                if (isBreak) 
                {
                    foreach (var elemFirst in MatrixTransition)
                    {
                        if (OldVer.Contains(elemFirst.StartCondition) && OldVer.Contains(elemFirst.NextCondition)
                            && elemFirst.StartCondition != elemFirst.NextCondition)
                        {
                            elemFirst.StartCondition = NewVer[0];
                            int buf = OldVer.FindIndex(x => x == elemFirst.NextCondition);
                            strNewVer = "b" + elemFirst.NextCondition;
                            NewVer[buf] = strNewVer;
                            elemFirst.NextCondition = strNewVer;
                        }
                    }
                    foreach (var elemFirst in MatrixTransition)
                    {
                        for (int i = 0; i < OldVer.Count; i++)
                        {
                            if(elemFirst.NextCondition == OldVer[i])
                            {
                                elemFirst.NextCondition = NewVer[i];
                            }
                            if (elemFirst.StartCondition == OldVer[i])
                            {
                                elemFirst.StartCondition = NewVer[i];
                            }
                        }
                    }
                    
                    if(strNewVer != "")
                        for(int i = 0; i < MatrixTransition.Count; i++)
                        {
                            if (MatrixTransition[i].StartCondition == strNewVer)
                            {
                                RuleTransition rule = new RuleTransition();
                                rule.StartCondition = NewVer[NewVer.FindIndex(x => x != strNewVer)];
                                rule.Symbol = MatrixTransition[i].Symbol;
                                if (MatrixTransition[i].StartCondition != MatrixTransition[i].NextCondition)
                                    rule.NextCondition = MatrixTransition[i].NextCondition;
                                else
                                    rule.NextCondition = NewVer[NewVer.FindIndex(x => x != strNewVer)];
                                MatrixTransition.Add(rule);
                            }
                        }
                    OldVer = new List<string>();
                    NewVer = new List<string>();
                }
            } while (isBreak);
        }
        public StackPanel GetTable()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            StackPanel stackPanel1 = new StackPanel();
            List<string> buf = new List<string>();
            TextBlock textBlocks = new TextBlock();
            textBlocks.Text = " ";
            textBlocks.Height = 20;
            stackPanel1.Children.Add(textBlocks);
            foreach (var elem in MatrixTransition)
            {
                if (!buf.Contains(elem.StartCondition))
                {
                    buf.Add(elem.StartCondition);
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = elem.StartCondition;
                    textBlock.Height = 20;
                    stackPanel1.Children.Add(textBlock);
                }
            }
            foreach (var elem in MatrixTransition)
            {
                if (!buf.Contains(elem.NextCondition))
                {
                    buf.Add(elem.NextCondition);
                }
            }
            stackPanel.Children.Add(stackPanel1);
            List<string> buf2 = new List<string>(buf.Count);
            for (int i = 0; i < buf.Count; i++)
                buf2.Add("");
            foreach (var elem in buf)
            {
                StackPanel stack = new StackPanel();
                stack.Width = 40;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = elem;
                textBlock.Height = 20;
                stack.Children.Add(textBlock);
                foreach (var elem2 in MatrixTransition)
                {
                    if(elem2.NextCondition == elem)
                    {
                        int k = buf.FindIndex(x => x == elem2.StartCondition);
                        buf2[k] +=elem2.Symbol+" ";
                    }
                }
                for (int i = 0; i < buf2.Count; i++)
                {
                    textBlock = new TextBlock();
                    textBlock.Text = buf2[i];
                    textBlock.Height = 20;
                    stack.Children.Add(textBlock);
                    buf2[i] = "";
                }
                stackPanel.Children.Add(stack);
            }
            return stackPanel;
        }
    }
}
