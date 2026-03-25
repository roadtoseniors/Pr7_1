using System;

namespace GetStartedDebugging
{
    /// <summary>
    /// Демонстрационное приложение для обучения работе с отладчиком Visual Studio.
    /// Составляет имя из массива символов и отправляет сообщение на каждом шаге.
    /// </summary>
    class ArrayExample
    {
        /// <summary>
        /// Точка входа в программу. Последовательно составляет строку из символов
        /// и вызывает <see cref="SendMessage"/> на каждой итерации.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            char[] letters = { 'f', 'r', 'e', 'd', ' ', 's', 'm', 'i', 't', 'h' };
            string name = "";
            int[] a = new int[10];

            for (int i = 0; i < letters.Length; i++)
            {
                name += letters[i];
                a[i] = i + 1;
                SendMessage(name, a[i]);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Выводит приветственное сообщение с текущим именем и счётчиком.
        /// </summary>
        /// <param name="name">Строка имени, накопленная к текущей итерации.</param>
        /// <param name="msg">Числовой счётчик (номер итерации + 1).</param>
        static void SendMessage(string name, int msg)
        {
            Console.WriteLine("Hello, " + name + "! Count to " + msg);
        }
    }
}
