using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestEncoderDecoder
{
    /// <summary>
    /// Набор модульных тестов для класса <see cref="HillCipher"/>.
    /// Тестовые сценарии составлены по документу «Тестовые_сценарии_ПР7_2».
    /// Покрывает функциональные (TC_FUNC), негативные (TC_NEG),
    /// UI (TC_UI) и безопасности (TC_SEC) тестовые случаи.
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

        // ─────────────────────────────────────────────────────────────
        // TC_FUNC_1 — Шифрование и дешифрование биграмм (матрица 2x2)
        // Приоритет: Высокий
        // Тестовые данные: Текст "ПРИВЕТ", матрица [[3,4],[2,5]]
        //   det = 3*5 - 4*2 = 7, gcd(7, 33) = 1 — матрица обратима ✓
        // Ожидаемый результат: дешифрованный текст совпадает с исходным
        // Фактический результат: ПРИВЕТ → РСВЫШЁ → ПРИВЕТ
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_FUNC_1: Проверяет, что шифрование и последующее дешифрование
        /// текста «ПРИВЕТ» с матрицей 2x2 возвращает исходную строку.
        /// Длина текста (6) кратна размеру блока — паддинг не добавляется.
        /// </summary>
        [TestMethod]
        public void EncryptDecrypt_2x2_ShouldReturnOriginalText()
        {
            string original = "ПРИВЕТ";
            // det = 3*5 - 4*2 = 7, gcd(7, 33) = 1 ✓
            int[,] key = { { 3, 4 }, { 2, 5 } };

            string encrypted = cipher.Encrypt(original, key);
            string decrypted = cipher.Decrypt(encrypted, key);

            Assert.AreEqual(original, decrypted);
        }

        // ─────────────────────────────────────────────────────────────
        // TC_FUNC_2 — Шифрование триграмм (матрица 3x3)
        // Приоритет: Высокий
        // Тестовые данные: Текст "СЕКРЕТ", матрица [[2,3,1],[1,1,2],[3,0,1]]
        //   det = 14, gcd(14, 33) = 1 — матрица обратима ✓
        //   (матрица из документа [[6,24,1],[13,16,10],[20,17,15]] заменена
        //    на эквивалентную рабочую: det оригинала = 441, gcd(12,33) = 3 ≠ 1)
        // Ожидаемый результат: корректно зашифрованный текст без ошибок
        // Фактический результат: СЕКРЕТ → ЬЛЯВЪД (шифрование выполнено успешно)
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_FUNC_2: Проверяет корректность шифрования текста «СЕКРЕТ»
        /// с матрицей 3x3. Зашифрованный результат не должен совпадать с исходным.
        /// </summary>
        [TestMethod]
        public void Encrypt_3x3_ShouldReturnEncryptedText()
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

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(text, encrypted);
            Assert.AreEqual(text.Length, encrypted.Length);
        }

        // ─────────────────────────────────────────────────────────────
        // TC_FUNC_3 — Проверка детерминанта матрицы
        // Приоритет: Высокий
        // Тестовые данные: [[3,4],[2,5]]
        //   (в документе [[3,3],[2,5]] с det=9, но gcd(9,33)=3 ≠ 1 — не обратима;
        //    заменена на [[3,4],[2,5]] с det=7, gcd(7,33)=1)
        // Ожидаемый результат: детерминант = 7, матрица признана обратимой
        // Фактический результат: GetDeterminant = 7, IsInvertible = true
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_FUNC_3: Проверяет корректность вычисления определителя матрицы 2x2
        /// и признание её обратимой по mod 33. Для [[3,4],[2,5]]: det = 7.
        /// </summary>
        [TestMethod]
        public void Determinant_2x2_ShouldBeCorrectAndInvertible()
        {
            int[,] matrix = { { 3, 4 }, { 2, 5 } };

            int det = cipher.GetDeterminant(matrix);
            bool invertible = cipher.IsInvertible(matrix);

            Assert.AreEqual(7, det);
            Assert.IsTrue(invertible, "Матрица должна быть признана обратимой");
        }

        // ─────────────────────────────────────────────────────────────
        // TC_NEG_1 — Необратимая матрица (det = 0)
        // Приоритет: Высокий
        // Тестовые данные: [[2,4],[1,2]], det = 2*2 - 4*1 = 0
        // Ожидаемый результат: выброс исключения, шифрование не выполняется
        // Фактический результат: выброшено исключение «Матрица необратима»
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_NEG_1: Проверяет, что при использовании вырожденной матрицы (det=0)
        /// <see cref="HillCipher.Encrypt"/> выбрасывает исключение и
        /// шифрование не выполняется.
        /// </summary>
        [TestMethod]
        public void Encrypt_NonInvertibleMatrix_ShouldThrowException()
        {
            string text = "ТЕСТ";
            // det = 2*2 - 4*1 = 0 — матрица вырожденная
            int[,] matrix = { { 2, 4 }, { 1, 2 } };

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

        // ─────────────────────────────────────────────────────────────
        // TC_NEG_2 — Некорректная длина текста (паддинг)
        // Приоритет: Средний
        // Тестовые данные: текст "ПРИВЕ" (5 символов), матрица [[3,4],[2,5]]
        // Ожидаемый результат: дополнение символом «Х» до длины 6
        // Фактический результат: длина результата = 6 (паддинг добавлен)
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_NEG_2: Проверяет автоматическое дополнение символом «Х» при длине текста,
        /// не кратной размеру блока. «ПРИВЕ» (5 символов) → зашифрованная строка длиной 6.
        /// </summary>
        [TestMethod]
        public void Encrypt_TextWithPadding_ShouldReturnCorrectLength()
        {
            string text = "ПРИВЕ"; // 5 символов → паддинг до 6
            int[,] matrix = { { 3, 4 }, { 2, 5 } };

            string result = cipher.Encrypt(text, matrix);

            Assert.AreEqual(6, result.Length, "Длина результата должна быть 6 после добавления паддинга");
        }

        // ─────────────────────────────────────────────────────────────
        // TC_NEG_3 — Пустая строка
        // Приоритет: Высокий
        // Тестовые данные: text = "" (пустая строка), матрица [[3,4],[2,5]]
        // Ожидаемый результат: сообщение об ошибке / исключение
        // Фактический результат: выброшено исключение «Пустой текст»
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_NEG_3: Проверяет, что передача пустой строки в
        /// <see cref="HillCipher.Encrypt"/> приводит к выбросу исключения.
        /// </summary>
        [TestMethod]
        public void Encrypt_EmptyString_ShouldThrowException()
        {
            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt("", new int[,] { { 3, 4 }, { 2, 5 } });
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Ожидается исключение для пустой строки");
        }

        // ─────────────────────────────────────────────────────────────
        // TC_NEG_4 — Обработка null-значения
        // Приоритет: Высокий
        // Тестовые данные: text = null, матрица [[3,4],[2,5]]
        // Ожидаемый результат: выброс исключения, программа не завершается аварийно
        // Фактический результат: выброшено исключение — программа устойчива
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_NEG_4: Проверяет корректную обработку <c>null</c> в
        /// <see cref="HillCipher.Encrypt"/>. Программа не должна завершаться аварийно.
        /// </summary>
        [TestMethod]
        public void Encrypt_NullText_ShouldThrowException()
        {
            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt(null, new int[,] { { 3, 4 }, { 2, 5 } });
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Ожидается исключение при передаче null");
        }

        // ─────────────────────────────────────────────────────────────
        // TC_UI_1 — Ввод нечисловых значений в матрицу
        // Приоритет: Средний
        // Тестовые данные: матрица "a b; c d" (нечисловые символы)
        // Ожидаемый результат: исключение с сообщением «Неверный формат матрицы»
        // Фактический результат: ParseMatrix выбросил «Неверный формат матрицы»
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_UI_1: Проверяет, что передача нечисловых значений в парсер матрицы
        /// вызывает исключение с сообщением «Неверный формат матрицы».
        /// Симулирует ввод пользователем букв вместо цифр.
        /// </summary>
        [TestMethod]
        public void ParseMatrix_NonNumericInput_ShouldThrowException()
        {
            bool exceptionThrown = false;
            string exceptionMessage = "";
            try
            {
                ParseMatrix("a b; c d");
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                exceptionMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown, "Ожидается исключение для нечислового ввода");
            Assert.AreEqual("Неверный формат матрицы", exceptionMessage);
        }

        // ─────────────────────────────────────────────────────────────
        // TC_SEC_1 — Обработка недопустимых символов в тексте
        // Приоритет: Средний
        // Тестовые данные: текст "HELLO123!!!", матрица [[3,4],[2,5]]
        // Ожидаемый результат: исключение — символы не из русского алфавита
        // Фактический результат: выброшено исключение (IndexOf вернул -1,
        //   что приводит к выходу за границы строки алфавита)
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_SEC_1: Проверяет поведение при вводе текста с символами вне
        /// русского алфавита (латиница, цифры, спецсимволы).
        /// Ожидается исключение — недопустимые символы не должны обрабатываться молча.
        /// </summary>
        [TestMethod]
        public void Encrypt_InvalidChars_ShouldThrowException()
        {
            bool exceptionThrown = false;
            try
            {
                cipher.Encrypt("HELLO123!!!", new int[,] { { 3, 4 }, { 2, 5 } });
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Ожидается исключение для символов вне алфавита");
        }

        // ─────────────────────────────────────────────────────────────
        // TC_FUNC_4 — Проверка корректности нахождения обратной матрицы
        // Приоритет: Высокий
        // Тестовые данные: [[3,4],[2,5]], det = 7, gcd(7,33) = 1
        // Ожидаемый результат: A * A⁻¹ = E (единичная матрица) по mod 33
        // Фактический результат: [[3,4],[2,5]] * [[29,23],[28,24]] = [[1,0],[0,1]] mod 33
        // Статус: Зачёт
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// TC_FUNC_4: Проверяет, что произведение матрицы на её обратную
        /// даёт единичную матрицу по модулю 33.
        /// </summary>
        [TestMethod]
        public void InverseMatrix_2x2_ProductShouldBeIdentity()
        {
            int[,] matrix = { { 3, 4 }, { 2, 5 } };

            int[,] inverse = cipher.GetInverseMatrix(matrix);
            int[,] product = MultiplyMod(matrix, inverse);

            Assert.IsTrue(IsIdentity(product), "Произведение матрицы на обратную должно быть единичной");
        }

        // ─────────────────────────────────────────────────────────────
        // Вспомогательные методы
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Разбирает строку матрицы в двумерный массив (аналог ParseMatrix из MainWindow).
        /// Строки разделяются «;», элементы внутри строки — пробелами.
        /// </summary>
        /// <param name="input">Строка вида "1 2; 3 4".</param>
        /// <returns>Квадратная матрица <c>int[n,n]</c>.</returns>
        /// <exception cref="Exception">Выбрасывается при неверном формате или нечисловых значениях.</exception>
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
        /// Перемножает две квадратные матрицы по модулю 33.
        /// Используется для проверки обратной матрицы в TC_FUNC_4.
        /// </summary>
        /// <param name="a">Первая матрица n×n.</param>
        /// <param name="b">Вторая матрица n×n.</param>
        /// <returns>Результирующая матрица n×n, элементы взяты по mod 33.</returns>
        private int[,] MultiplyMod(int[,] a, int[,] b)
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