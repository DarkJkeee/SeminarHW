using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        // Объявление делегата MathOperation, который хранит ссылку на метод к соответствующей операции.
        delegate double MathOperation(double a, double b);
        // Объявление словаря для определения операции.
        static readonly Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation>();

        static void Main()
        {
            // Счетчик для Error.
            int k = 0;
            // Заполнение словаря методами.
            operations.Add("+", (a, b) => a + b);
            operations.Add("-", (a, b) => a - b);
            operations.Add("*", (a, b) => a * b);
            operations.Add("/", (a, b) => a / b);
            operations.Add("^", (a, b) => Math.Pow(a, b));

            string[] res = Calculation();
            
            File.WriteAllLines("results.txt", CalculationChecker(res, ref k));

            File.AppendAllText("results.txt", k.ToString());
        }

        /// <summary>
        /// Метод, который разбивает выражение и вычисляет его.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        static double Calculate(string expr)
        {
            double num1 = 0, num2 = 0;
            string number1 = "", number2 = "";
            string operation = "";
            int size = expr.Length;

            for (int i = 0; i < size; ++i)
            {
                if (expr[i] != ' ')
                {
                    number1 += expr[i];
                }
                else
                {
                    size = i;
                    operation += expr[i + 1];

                    for (int j = i + 2; j < expr.Length; ++j)
                    {
                        number2 += expr[j];
                    }
                }
            }
            try
            {
                num1 = double.Parse(number1);
                num2 = double.Parse(number2);
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат строки!");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Пустая строка!");
            }


            return operations[operation](num1, num2);
        }

        /// <summary>
        /// Метод, который считывает выражения из файла и записывает результат в новый файл.
        /// </summary>
        /// <returns></returns>
        static string[] Calculation()
        {
            string[] str = File.ReadAllLines("expressions.txt");
            string[] res = new string[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                double num = Calculate(str[i]);
                // Форматируем num.
                res[i] = String.Format("{0:f3}", num);

            }
            File.WriteAllLines("answers.txt", res);
            return res;
        }

        /// <summary>
        /// Метод, который сравнивает ответы с результатами из файла.
        /// </summary>
        /// <param name="res"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        static string[] CalculationChecker(string[] res, ref int k)
        {
            string[] str = File.ReadAllLines("expressions_checker.txt");
            string[] checker = new string[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                if(str[i] == res[i])
                {
                    checker[i] = "OK"; 
                }
                else
                {
                    checker[i] = "Error";
                    ++k;
                }
            }
            return checker;
        }
    }
}
