using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestEncoderDecoder
{
    /// <summary>
    /// Набор модульных тестов для класса <see cref="HillCipher"/>.
    /// Покрывает функциональные сценарии (шифрование, дешифрование, детерминант, обратная матрица)
    /// и негативные сценарии (пустой ввод, null, необратимая матрица, паддинг).
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>Экземпляр шифра, используемый во всех тестах.</summary>
        private HillCipher cipher;

        /// <summary>
        /// Инициализирует экземпляр <see cref="HillCipher"/> перед каждым тестом.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            cipher = new HillCipher();
        }

        // TC_FUNC_1 — шифрование/дешифрование 2x2
        /// <summary>
        /// TC_FUNC_1: Проверяет, что шифрование и последующее дешифрование
        /// текста с матрицей 2x2 возвращает исходную строку.
        /// Текст «ПРИВЕТ» (длина 6) кратен размеру матрицы — паддинг не добавляется.
        /// Ключ: [[3,2],[5,8]], det=14, gcd(14,33)=1.
        /// </summary>
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
        /// <summary>
        /// TC_FUNC_2: Проверяет, что шифрование и последующее дешифрование
        /// текста с матрицей 3x3 возвращает исходную строку.
        /// Текст «СЕКРЕТ» (длина 6) кратен размеру матрицы — паддинг не добавляется.
        /// Ключ: [[2,3,1],[1,1,2],[3,0,1]], det=14, gcd(14,33)=1.
        /// </summary>
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
        /// <summary>
        /// TC_FUNC_2b: Проверяет корректность паддинга при шифровании матрицей 3x3.
        /// Текст «СЕКРЕ» (длина 5) дополняется до 6 символов буквой «Х»,
        /// поэтому дешифрованный результат должен быть «СЕКРЕХ».
        /// </summary>
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
        /// <summary>
        /// TC_FUNC_3: Проверяет корректность вычисления определителя матрицы 2x2.
        /// Для [[3,2],[5,8]]: det = 3*8 - 2*5 = 14.
        /// </summary>
        [TestMethod]
        public void Determinant_ShouldBeCorrect()
        {
            // det = 3*8 - 2*5 = 14
            int[,] matrix = { { 3, 2 }, { 5, 8 } };
            int det = cipher.GetDeterminant(matrix);
            Assert.AreEqual(14, det);
        }

        // TC_NEG_1 — вырожденная матрица
        /// <summary>
        /// TC_NEG_1: Проверяет, что при передаче необратимой матрицы (det=0)
        /// метод <see cref="HillCipher.Encrypt"/> выбрасывает исключение.
        /// Матрица [[2,4],[1,2]]: det = 2*2 - 4*1 = 0.
        /// </summary>
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
        /// <summary>
        /// TC_NEG_2: Проверяет, что при длине текста, не кратной размеру матрицы,
        /// автоматически добавляется паддинг. «ПРИВЕ» (5 символов) + 1 «Х» = 6 символов.
        /// </summary>
        [TestMethod]
        public void Encrypt_InvalidLength_ShouldAddPadding()
        {
            string text = "ПРИВЕ"; // длина 5, 2x2 -> паддинг
            int[,] matrix = { { 3, 2 }, { 5, 8 } };

            string result = cipher.Encrypt(text, matrix);
            Assert.AreEqual(6, result.Length); // добавлен 1 символ паддинга
        }

        // TC_NEG_3 — пустая строка
        /// <summary>
        /// TC_NEG_3: Проверяет, что передача пустой строки в <see cref="HillCipher.Encrypt"/>
        /// приводит к выбросу исключения.
        /// </summary>
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
        /// <summary>
        /// TC_NEG_4: Проверяет, что передача <c>null</c> в <see cref="HillCipher.Encrypt"/>
        /// приводит к выбросу исключения.
        /// </summary>
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
        /// <summary>
        /// TC_FUNC_4: Проверяет корректность вычисления обратной матрицы для 2x2.
        /// Произведение матрицы на её обратную должно давать единичную матрицу по mod 33.
        /// </summary>
        [TestMethod]
        public void InverseMatrix_2x2_ShouldReturnIdentity()
        {
            int[,] matrix = { { 3, 2 }, { 5, 8 } };
            int[,] inverse = cipher.GetInverseMatrix(matrix);
            int[,] result = Multiply(matrix, inverse);
            Assert.IsTrue(IsIdentity(result));
        }

        // TC_FUNC_5 — обратная матрица 3x3
        /// <summary>
        /// TC_FUNC_5: Проверяет корректность вычисления обратной матрицы для 3x3.
        /// Произведение матрицы на её обратную должно давать единичную матрицу по mod 33.
        /// </summary>
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
        /// <summary>
        /// Перемножает две квадратные матрицы по модулю 33.
        /// Используется в тестах TC_FUNC_4 и TC_FUNC_5 для проверки обратной матрицы.
        /// </summary>
        /// <param name="a">Первая матрица n×n.</param>
        /// <param name="b">Вторая матрица n×n.</param>
        /// <returns>Результирующая матрица n×n, элементы взяты по mod 33.</returns>
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

        /// <summary>
        /// Проверяет, является ли матрица единичной по mod 33.
        /// Диагональные элементы должны равняться 1, остальные — 0.
        /// </summary>
        /// <param name="matrix">Квадратная матрица n×n.</param>
        /// <returns><c>true</c>, если матрица единичная; иначе <c>false</c>.</returns>
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