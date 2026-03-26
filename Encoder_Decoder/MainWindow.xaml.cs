using System;
using System.Windows;
using System.Windows.Controls;

namespace Encoder_Decoder
{
    public partial class MainWindow : Window
    {
        private HillCipher cipher = new HillCipher();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Парсинг квадратной матрицы из строки вида "1 2; 3 4" или "2 3 1; 1 1 2; 3 0 1"
        /// </summary>
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