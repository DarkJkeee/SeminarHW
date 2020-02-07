using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    // Объявление делегата MathOperation, который хранит ссылку на метод к соответствующей операции.
    delegate double MathOperation(double a, double b);
    // Объявлене делегата-типа ErrorNotificationType.
    delegate void ErrorNotificationType(string message);

    class Program
    {
        static void Main()
        {
            // Счетчик для Error.
            int k = 0;
            // Очищаем файл перед записью.
            File.WriteAllText("answers.txt", "");

            // Подписываем обработчики на событие.
            Calculator.Handler(ConsoleErrorHandler);
            Calculator.Handler(ResultErrorHandler);

            // Записываем результат сравнения в файл.
            File.WriteAllLines("results.txt", CalculationChecker(Calculation(), ref k));
            File.AppendAllText("results.txt", k.ToString());
            Console.ReadLine();
        }

        /// <summary>
        /// Метод, который считывает выражения из файла и записывает результат в новый файл.
        /// </summary>
        /// <returns></returns>
        static public string[] Calculation()
        {
            string[] str = File.ReadAllLines("expressions.txt");
            string[] res = new string[str.Length];

            for (int i = 0; i < str.Length; ++i)
            {
                try
                {
                    double num = Calculator.Calculate(str[i]);
                    res[i] = $"{Math.Round(num, 3, MidpointRounding.ToEven):f3}";
                    File.AppendAllText("answers.txt", $"{res[i]}\r\n");
                }
                catch(OverflowException)
                {
                    res[i] = double.PositiveInfinity.ToString();
                }
                catch(Exception ex)
                {
                    res[i] = ex.Message;
                }
            }
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
                if (str[i] == res[i])
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
        /// <summary>
        /// Обработчик события, который выводит на консоль сообщение об ошибке.
        /// </summary>
        /// <param name="message"></param>
        public static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine(message + " " + DateTime.Now);
        }
        /// <summary>
        /// Обработчик события, который записывает ошибку в файл.
        /// </summary>
        /// <param name="message"></param>
        public static void ResultErrorHandler(string message)
        {
            File.AppendAllText("answers.txt", message + "\r\n");
        }
    }

    class Calculator
    {
        static event ErrorNotificationType ErrorNotification;
        // Словарь для хранения математический операций.
        static Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation>()
        {
            ["+"] = (a, b) => a + b,
            ["-"] = (a, b) => a - b,
            ["*"] = (a, b) => a * b,
            ["/"] = (a, b) => a / b,
            ["^"] = (a, b) => Math.Pow(a, b),
        };
        public static void Handler(ErrorNotificationType e)
        {
            ErrorNotification += e;
        }

        /// <summary>
        /// Метод, который разбивает выражение и возвращает результат вычисления.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        static public double Calculate(string expr)
        {
            double ans = 0;
            string[] numbers = expr.Split(' ');
            try
            {
                double num1 = double.Parse(numbers[0]);
                double num2 = double.Parse(numbers[2]);
                string operation = numbers[1];
                if (num2 == 0 && operation == "/")
                {
                    throw new DivideByZeroException("bruh");
                }
                if (!operations.ContainsKey(operation))
                {
                    throw new KeyNotFoundException("неверный оператор");
                }
                ans = operations[operation](num1, num2);
                return ans;
            }
            catch(OverflowException)
            {
                ErrorNotification(double.PositiveInfinity.ToString());
                throw;
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.Message);
                throw;
            }
        }
    }
}
