using System;
using System.Text;

/// <summary>
/// Реализация шифра Хилла для русского алфавита (33 символа, mod 33).
/// Поддерживает ключевые матрицы размером 2x2 и 3x3.
/// </summary>
public class HillCipher
{
    /// <summary>Модуль арифметики — размер русского алфавита.</summary>
    private const int Mod = 33;

    /// <summary>Русский алфавит, используемый для кодирования символов в числа и обратно.</summary>
    private string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    /// <summary>
    /// Шифрует текст методом Хилла с использованием заданной ключевой матрицы.
    /// Если длина текста не кратна размеру матрицы, добавляется паддинг символом «Х».
    /// </summary>
    /// <param name="text">Открытый текст на русском языке. Не может быть null или пустым.</param>
    /// <param name="key">Квадратная ключевая матрица (2x2 или 3x3), обратимая по mod 33.</param>
    /// <returns>Зашифрованная строка из символов русского алфавита.</returns>
    /// <exception cref="Exception">
    /// Выбрасывается, если текст пустой, матрица не квадратная или необратима по mod 33.
    /// </exception>
    public string Encrypt(string text, int[,] key)
    {
        if (string.IsNullOrEmpty(text))
            throw new Exception("Пустой текст");

        text = text.ToUpper().Replace(" ", "");
        int n = key.GetLength(0);

        if (key.GetLength(1) != n)
            throw new Exception("Матрица должна быть квадратной");

        if (!IsInvertible(key))
            throw new Exception("Матрица необратима");

        // паддинг
        while (text.Length % n != 0)
            text += "Х";

        var result = new StringBuilder();
        for (int i = 0; i < text.Length; i += n)
        {
            int[] vector = new int[n];
            for (int j = 0; j < n; j++)
                vector[j] = alphabet.IndexOf(text[i + j]);

            int[] encryptedVector = MultiplyMatrixVector(key, vector);
            foreach (var val in encryptedVector)
                result.Append(alphabet[(val % Mod + Mod) % Mod]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Дешифрует текст, зашифрованный методом Хилла.
    /// Вычисляет обратную ключевую матрицу и применяет <see cref="Encrypt"/>.
    /// </summary>
    /// <param name="text">Зашифрованный текст.</param>
    /// <param name="key">Ключевая матрица, использованная при шифровании.</param>
    /// <returns>Расшифрованная строка (возможно, с паддинг-символом «Х» в конце).</returns>
    /// <exception cref="Exception">Выбрасывается, если матрица необратима по mod 33.</exception>
    public string Decrypt(string text, int[,] key)
    {
        int[,] inverse = GetInverseMatrix(key);
        return Encrypt(text, inverse);
    }

    /// <summary>
    /// Умножает квадратную матрицу на вектор-столбец по модулю <see cref="Mod"/>.
    /// </summary>
    /// <param name="matrix">Квадратная матрица размером n×n.</param>
    /// <param name="vector">Вектор длиной n.</param>
    /// <returns>Результирующий вектор длиной n, каждый элемент взят по mod 33.</returns>
    private int[] MultiplyMatrixVector(int[,] matrix, int[] vector)
    {
        int n = vector.Length;
        int[] result = new int[n];

        for (int i = 0; i < n; i++)
        {
            int sum = 0;
            for (int j = 0; j < n; j++)
                sum += matrix[i, j] * vector[j];
            result[i] = (sum % Mod + Mod) % Mod;
        }

        return result;
    }

    /// <summary>
    /// Вычисляет определитель квадратной матрицы размером 2x2 или 3x3.
    /// </summary>
    /// <param name="matrix">Квадратная матрица 2x2 или 3x3.</param>
    /// <returns>Целочисленный определитель (без взятия по модулю).</returns>
    /// <exception cref="Exception">
    /// Выбрасывается, если матрица не квадратная или её размер отличается от 2 или 3.
    /// </exception>
    public int GetDeterminant(int[,] matrix)
    {
        int n = matrix.GetLength(0);
        if (n != matrix.GetLength(1))
            throw new Exception("Матрица должна быть квадратной");

        if (n == 2)
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        else if (n == 3)
        {
            return matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                 - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                 + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);
        }
        else
            throw new Exception("Поддерживаются только 2x2 и 3x3 матрицы");
    }

    /// <summary>
    /// Проверяет, является ли матрица обратимой по модулю <see cref="Mod"/>.
    /// Условие: определитель матрицы (mod 33) взаимно прост с 33.
    /// </summary>
    /// <param name="matrix">Квадратная матрица 2x2 или 3x3.</param>
    /// <returns><c>true</c>, если матрица обратима по mod 33; иначе <c>false</c>.</returns>
    public bool IsInvertible(int[,] matrix)
    {
        int det = ((GetDeterminant(matrix) % Mod) + Mod) % Mod;
        return GCD(det, Mod) == 1;
    }

    /// <summary>
    /// Вычисляет обратную матрицу по модулю <see cref="Mod"/> для матриц 2x2 и 3x3.
    /// Для 3x3 используется метод матрицы алгебраических дополнений (adjugate).
    /// </summary>
    /// <param name="matrix">Обратимая квадратная матрица 2x2 или 3x3.</param>
    /// <returns>Обратная матрица, все элементы которой лежат в диапазоне [0, 32].</returns>
    /// <exception cref="Exception">
    /// Выбрасывается, если матрица не квадратная или её определитель не имеет обратного по mod 33.
    /// </exception>
    public int[,] GetInverseMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);
        if (n != matrix.GetLength(1))
            throw new Exception("Матрица должна быть квадратной");

        int det = ((GetDeterminant(matrix) % Mod) + Mod) % Mod;
        int invDet = ModInverse(det, Mod);
        int[,] result = new int[n, n];

        if (n == 2)
        {
            result[0, 0] = (matrix[1, 1] * invDet % Mod + Mod) % Mod;
            result[0, 1] = (-matrix[0, 1] * invDet % Mod + Mod) % Mod;
            result[1, 0] = (-matrix[1, 0] * invDet % Mod + Mod) % Mod;
            result[1, 1] = (matrix[0, 0] * invDet % Mod + Mod) % Mod;
        }
        else if (n == 3)
        {
            // Матрица кофакторов (cofactor matrix)
            int[,] cof = new int[3, 3];
            cof[0, 0] = (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]);
            cof[0, 1] = -(matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]);
            cof[0, 2] = (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

            cof[1, 0] = -(matrix[0, 1] * matrix[2, 2] - matrix[0, 2] * matrix[2, 1]);
            cof[1, 1] = (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]);
            cof[1, 2] = -(matrix[0, 0] * matrix[2, 1] - matrix[0, 1] * matrix[2, 0]);

            cof[2, 0] = (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]);
            cof[2, 1] = -(matrix[0, 0] * matrix[1, 2] - matrix[0, 2] * matrix[1, 0]);
            cof[2, 2] = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]);

            // Adjugate = транспонированная матрица кофакторов, умноженная на invDet
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i, j] = (cof[j, i] * invDet % Mod + Mod) % Mod;
        }

        return result;
    }

    /// <summary>
    /// Находит обратный элемент <paramref name="a"/> по модулю <paramref name="mod"/>
    /// методом перебора (подходит для малых значений модуля).
    /// </summary>
    /// <param name="a">Число, для которого ищется обратный элемент.</param>
    /// <param name="mod">Модуль (33 для русского алфавита).</param>
    /// <returns>Такое <c>x</c>, что <c>(a * x) % mod == 1</c>.</returns>
    /// <exception cref="Exception">Выбрасывается, если обратного элемента не существует.</exception>
    private int ModInverse(int a, int mod)
    {
        a = (a % mod + mod) % mod;
        for (int x = 1; x < mod; x++)
            if ((a * x) % mod == 1)
                return x;
        throw new Exception("Нет обратного элемента");
    }

    /// <summary>
    /// Вычисляет наибольший общий делитель двух чисел алгоритмом Евклида.
    /// </summary>
    /// <param name="a">Первое число.</param>
    /// <param name="b">Второе число.</param>
    /// <returns>НОД чисел <paramref name="a"/> и <paramref name="b"/>.</returns>
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int tmp = b;
            b = a % b;
            a = tmp;
        }
        return a;
    }
}