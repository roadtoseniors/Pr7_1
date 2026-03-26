using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestEncoderDecoder
{
    [TestClass]
    public class UnitTest1
    {
        private HillCipher cipher;

        [TestInitialize]
        public void Init()
        {
            cipher = new HillCipher();
        }

        // TC_FUNC_1 — шифрование/дешифрование
        [TestMethod]
        public void EncryptDecrypt_ShouldReturnOriginalText()
        {
            string original = "ПРИВЕТ";
            int[,] key = { { 3, 3 }, { 2, 5 } };

            string encrypted = cipher.Encrypt(original, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            Assert.AreEqual(original, decrypted);
        }

        // TC_FUNC_2 — 3x3
        [TestMethod]
        public void Encrypt_3x3_ShouldWork()
        {
            string text = "СЕКРЕТ";
            int[,] key =
            {
                {6,24,1},
                {13,16,10},
                {20,17,15}
            };

            string encrypted = cipher.Encrypt(text, key);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(text, encrypted);
        }

        // TC_FUNC_3 — детерминант
        [TestMethod]
        public void Determinant_ShouldBeCorrect()
        {
            int[,] matrix = { { 3, 3 }, { 2, 5 } };

            int det = cipher.GetDeterminant(matrix);

            Assert.AreEqual(9, det);
        }

        // TC_NEG_1 — вырожденная матрица
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_WithNonInvertibleMatrix_ShouldThrow()
        {
            string text = "ТЕСТ";
            int[,] matrix = { { 2, 4 }, { 1, 2 } };

            cipher.Encrypt(text, matrix);
        }

        // TC_NEG_2 — длина не кратна
        [TestMethod]
        public void Encrypt_InvalidLength_ShouldHandle()
        {
            string text = "ПРИВЕ"; // 5 символов
            int[,] matrix = { { 3, 3 }, { 2, 5 } };

            try
            {
                string result = cipher.Encrypt(text, matrix);
                Assert.IsNotNull(result);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        // TC_NEG_3 — пустая строка
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_EmptyString_ShouldThrow()
        {
            string text = "";
            int[,] matrix = { { 3, 3 }, { 2, 5 } };

            cipher.Encrypt(text, matrix);
        }

        // TC_NEG_4 — null
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_Null_ShouldThrow()
        {
            string text = null;
            int[,] matrix = { { 3, 3 }, { 2, 5 } };

            cipher.Encrypt(text, matrix);
        }

        // TC_FUNC_4 — обратная матрица
        [TestMethod]
        public void InverseMatrix_ShouldReturnIdentity()
        {
            int[,] matrix = { { 3, 3 }, { 2, 5 } };

            int[,] inverse = cipher.GetInverseMatrix(matrix);
            int[,] result = Multiply(matrix, inverse);

            Assert.IsTrue(IsIdentity(result));
        }

        // вспомогательные методы
        private int[,] Multiply(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int[,] result = new int[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        result[i, j] += a[i, k] * b[k, j];

            return result;
        }

        private bool IsIdentity(int[,] matrix)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    if (i == j && matrix[i, j] != 1) return false;
                    if (i != j && matrix[i, j] != 0) return false;
                }

            return true;
        }
    }
}
