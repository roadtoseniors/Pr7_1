using System;
using System.Windows;
using System.Windows.Controls;

namespace Encoder_Decoder
{
    /// <summary>
    /// Главное окно приложения шифратора/дешифратора на основе шифра Хилла.
    /// Предоставляет пользовательский интерфейс для ввода текста, задания ключевой матрицы,
    /// а также запуска операций шифрования и дешифрования.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Экземпляр шифра Хилла, используемый для выполнения криптографических операций.
        /// </summary>
        private HillCipher cipher = new HillCipher();

        /// <summary>
        /// Инициализирует главное окно и все его визуальные компоненты.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Разбирает строковое представление квадратной матрицы в двумерный массив целых чисел.
        /// Строки матрицы разделяются символом «;», элементы внутри строки — пробелами.
        /// </summary>
        /// <example>
        /// Форматы ввода:
        /// <code>
        /// "1 2; 3 4"       — матрица 2x2
        /// "2 3 1; 1 1 2; 3 0 1"  — матрица 3x3
        /// </code>
        /// </example>
        /// <param name="input">Строка с представлением матрицы.</param>
        /// <returns>Квадратная матрица n×n в виде двумерного массива <c>int[n, n]</c>.</returns>
        /// <exception cref="Exception">
        /// Выбрасывается при неверном формате строки или если матрица не является квадратной.
        /// </exception>
        private int[,] ParseMatrix(string input)
        {
            try
            {
                var rows = input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                int n = rows.Length;
                int[,] matrix = new int[n, n];

                for (int i = 0; i < n; i++)
                {
                    var cols = rows[i].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (cols.Length != n)
                        throw new Exception("Матрица должна быть квадратной");

                    for (int j = 0; j < n; j++)
                        matrix[i, j] = int.Parse(cols[j]);
                }

                return matrix;
            }
            catch
            {
                throw new Exception("Неверный формат матрицы");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Зашифровать».
        /// Считывает текст из <c>InputTextBox</c> и матрицу из <c>MatrixBox</c>,
        /// выполняет шифрование и выводит результат в <c>ResultTextBox</c>.
        /// В случае ошибки отображает диалоговое окно с описанием проблемы.
        /// </summary>
        /// <param name="sender">Источник события — кнопка «Зашифровать».</param>
        /// <param name="e">Аргументы события нажатия кнопки.</param>
        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var matrix = ParseMatrix(MatrixBox.Text);
                ResultTextBox.Text = cipher.Encrypt(InputTextBox.Text, matrix);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка шифрования", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Расшифровать».
        /// Считывает текст из <c>InputTextBox</c> и матрицу из <c>MatrixBox</c>,
        /// выполняет дешифрование и выводит результат в <c>ResultTextBox</c>.
        /// В случае ошибки отображает диалоговое окно с описанием проблемы.
        /// </summary>
        /// <param name="sender">Источник события — кнопка «Расшифровать».</param>
        /// <param name="e">Аргументы события нажатия кнопки.</param>
        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var matrix = ParseMatrix(MatrixBox.Text);
                ResultTextBox.Text = cipher.Decrypt(InputTextBox.Text, matrix);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка дешифрования", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}