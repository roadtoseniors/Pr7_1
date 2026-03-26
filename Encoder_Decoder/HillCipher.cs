using System;

public class HillCipher
{
    private const int Mod = 33; // русский алфавит

    private string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    public string Encrypt(string text, int[,] key)
    {
        if (string.IsNullOrEmpty(text))
            throw new Exception("Пустой текст");

        if (!IsInvertible(key))
            throw new Exception("Матрица необратима");

        text = text.ToUpper().Replace(" ", "");

        int n = key.GetLength(0);

        while (text.Length % n != 0)
            text += "Х";

        string result = "";

        for (int i = 0; i < text.Length; i += n)
        {
            int[] vector = new int[n];

            for (int j = 0; j < n; j++)
                vector[j] = alphabet.IndexOf(text[i + j]);

            for (int row = 0; row < n; row++)
            {
                int sum = 0;
                for (int col = 0; col < n; col++)
                    sum += key[row, col] * vector[col];

                result += alphabet[(sum % Mod + Mod) % Mod];
            }
        }

        return result;
    }

    public string Decrypt(string text, int[,] key)
    {
        int[,] inverse = GetInverseMatrix(key);
        return Encrypt(text, inverse);
    }

    public int GetDeterminant(int[,] m)
    {
        if (m.GetLength(0) == 2)
            return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

        throw new Exception("Только 2x2 поддерживается");
    }

    public bool IsInvertible(int[,] m)
    {
        int det = GetDeterminant(m);
        return det % Mod != 0;
    }

    public int[,] GetInverseMatrix(int[,] m)
    {
        int det = GetDeterminant(m);
        int invDet = ModInverse(det, Mod);

        int[,] result = new int[2, 2];

        result[0, 0] = m[1, 1];
        result[1, 1] = m[0, 0];
        result[0, 1] = -m[0, 1];
        result[1, 0] = -m[1, 0];

        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                result[i, j] = (result[i, j] * invDet % Mod + Mod) % Mod;

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
}
