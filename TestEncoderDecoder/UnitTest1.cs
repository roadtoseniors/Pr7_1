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

        // TC_FUNC_1 — шифрование/дешифрование 2x2
        [TestMethod]
        public void EncryptDecrypt_2x2_ShouldReturnOriginalText()
        {
            string original = "ПРИВЕТ";
            int[,] key = { { 3, 2 }, { 5, 7 } }; // det=11, gcd(11,33)=1

            string encrypted = cipher.Encrypt(original, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            Assert.AreEqual(original + "ХХ", decrypted); // паддинг добавлен
        }

        // TC_FUNC_2 — шифрование/дешифрование 3x3
        [TestMethod]
        public void EncryptDecrypt_3x3_ShouldWork()
        {
            string text = "СЕКРЕТ";
            int[,] key =
            {
                { 2, 3, 1 },
                { 1, 1, 2 },
                { 3, 0, 1 }
            }; // det=1, gcd(1,33)=1

            string encrypted = cipher.Encrypt(text, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(text, encrypted);
            Assert.AreEqual(text + "Х", decrypted); // паддинг добавлен
        }

        // TC_FUNC_3 — детерминант
        [TestMethod]
        public void Determinant_ShouldBeCorrect()
        {
            int[,] matrix = { { 3, 2 }, { 5, 7 } };
            int det = cipher.GetDeterminant(matrix);
            Assert.AreEqual(11, det);
        }

        // TC_NEG_1 — вырожденная матрица
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_WithNonInvertibleMatrix_ShouldThrow()
        {
            string text = "ТЕСТ";
            int[,] matrix = { { 2, 4 }, { 1, 2 } }; // det=0, не обратима
            cipher.Encrypt(text, matrix);
        }

        // TC_NEG_2 — длина текста не кратна размеру матрицы
        [TestMethod]
        public void Encrypt_InvalidLength_ShouldHandle()
        {
            string text = "ПРИВЕ"; // 5 символов, 2x2 -> паддинг
            int[,] matrix = { { 3, 2 }, { 5, 7 } };

            string result = cipher.Encrypt(text, matrix);
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Length); // добавлен 1 символ паддинга
        }

        // TC_NEG_3 — пустая строка
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_EmptyString_ShouldThrow()
        {
            cipher.Encrypt("", new int[,] { { 3, 2 }, { 5, 7 } });
        }

        // TC_NEG_4 — null
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Encrypt_Null_ShouldThrow()
        {
            cipher.Encrypt(null, new int[,] { { 3, 2 }, { 5, 7 } });
        }

        // TC_FUNC_4 — обратная матрица 2x2
        [TestMethod]
        public void InverseMatrix_2x2_ShouldReturnIdentity()
        {
            int[,] matrix = { { 3, 2 }, { 5, 7 } };
            int[,] inverse = cipher.GetInverseMatrix(matrix);
            int[,] result = Multiply(matrix, inverse);
            Assert.IsTrue(IsIdentity(result));
        }

        // TC_FUNC_5 — обратная матрица 3x3
        [TestMethod]
        public void InverseMatrix_3x3_ShouldReturnIdentity()
        {
            int[,] matrix =
            {
                { 2, 3, 1 },
                { 1, 1, 2 },
                { 3, 0, 1 }
            };

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

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i, j] = (result[i, j] % 33 + 33) % 33;

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