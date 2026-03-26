using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Encoder_Decoder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HillCipher cipher = new HillCipher();
        public MainWindow()
        {
            InitializeComponent();
        }

        private int[,] ParseMatrix(string input)
        {
            try
            {
                var rows = input.Split(';');
                int[,] matrix = new int[2, 2];

                for (int i = 0; i < 2; i++)
                {
                    var cols = rows[i].Trim().Split(' ');
                    for (int j = 0; j < 2; j++)
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
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
        }
    }
}
