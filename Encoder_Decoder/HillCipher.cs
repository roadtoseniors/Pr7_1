using System;
using System.Text;

public class HillCipher
{
    private const int Mod = 33; // русский алфавит
    private string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

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

    public string Decrypt(string text, int[,] key)
    {
        int[,] inverse = GetInverseMatrix(key);
        return Encrypt(text, inverse);
    }

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

    public bool IsInvertible(int[,] matrix)
    {
        int det = ((GetDeterminant(matrix) % Mod) + Mod) % Mod;
        return GCD(det, Mod) == 1;
    }

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

    private int ModInverse(int a, int mod)
    {
        a = (a % mod + mod) % mod;
        for (int x = 1; x < mod; x++)
            if ((a * x) % mod == 1)
                return x;
        throw new Exception("Нет обратного элемента");
    }

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