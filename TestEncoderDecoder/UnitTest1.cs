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
        public void EncryptDecrypt_2x2_ShouldReturnOriginalTextWithPadding()
        {
            string original = "ПРИВЕТ";
            // det = 3*8 - 2*5 = 14, gcd(14, 33) = 1 ✓
            int[,] key = { { 3, 2 }, { 5, 8 } };

            string encrypted = cipher.Encrypt(original, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            // Паддинг: текст "ПРИВЕТ" длина 6, кратна 2 → добавления нет
            Assert.AreEqual(original, decrypted);
        }

        // TC_FUNC_2 — шифрование/дешифрование 3x3
        [TestMethod]
        public void EncryptDecrypt_3x3_ShouldReturnOriginalTextWithPadding()
        {
            string text = "СЕКРЕТ";
            // det = 14, gcd(14, 33) = 1 ✓
            int[,] key =
            {
                { 2, 3, 1 },
                { 1, 1, 2 },
                { 3, 0, 1 }
            };

            string encrypted = cipher.Encrypt(text, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            // Паддинг: длина текста 6, кратна 3 → добавления нет
            Assert.AreEqual(text, decrypted);
        }

        // TC_FUNC_2b — проверка с паддингом для 3x3
        [TestMethod]
        public void EncryptDecrypt_3x3_WithPadding_ShouldReturnOriginalPlusX()
        {
            string text = "СЕКРЕ"; // длина 5, 3x3 -> паддинг +1 символ
            int[,] key =
            {
                { 2, 3, 1 },
                { 1, 1, 2 },
                { 3, 0, 1 }
            };

            string encrypted = cipher.Encrypt(text, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            Assert.AreEqual("СЕКРЕХ", decrypted); // паддинг добавлен
        }

        // TC_FUNC_3 — детерминант
        [TestMethod]
        public void Determinant_ShouldBeCorrect()
        {
            // det = 3*8 - 2*5 = 14
            int[,] matrix = { { 3, 2 }, { 5, 8 } };
            int det = cipher.GetDeterminant(matrix);
            Assert.AreEqual(14, det);
        }

        // TC_NEG_1 — вырожденная матрица
        [TestMethod]
        public void Encrypt_WithNonInvertibleMatrix_ShouldHandleGracefully()
        {
            string text = "ТЕСТ";
            int[,] matrix = { { 2, 4 }, { 1, 2 } }; // det=0, не обратима

            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt(text, matrix);
            }
            catch
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Ожидается исключение для необратимой матрицы");
        }

        // TC_NEG_2 — длина текста не кратна размеру матрицы
        [TestMethod]
        public void Encrypt_InvalidLength_ShouldAddPadding()
        {
            string text = "ПРИВЕ"; // длина 5, 2x2 -> паддинг
            int[,] matrix = { { 3, 2 }, { 5, 8 } };

            string result = cipher.Encrypt(text, matrix);
            Assert.AreEqual(6, result.Length); // добавлен 1 символ паддинга
        }

        // TC_NEG_3 — пустая строка
        [TestMethod]
        public void Encrypt_EmptyString_ShouldThrow()
        {
            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt("", new int[,] { { 3, 2 }, { 5, 8 } });
            }
            catch
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        // TC_NEG_4 — null
        [TestMethod]
        public void Encrypt_Null_ShouldThrow()
        {
            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt(null, new int[,] { { 3, 2 }, { 5, 8 } });
            }
            catch
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        // TC_FUNC_4 — обратная матрица 2x2
        [TestMethod]
        public void InverseMatrix_2x2_ShouldReturnIdentity()
        {
            int[,] matrix = { { 3, 2 }, { 5, 8 } };
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