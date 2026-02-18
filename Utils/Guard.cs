using System;

namespace SystemOcenianiaSimple.Utils
{
    public static class Guard
    {
        public static int ReadInt(string label)
        {
            while (true)
            {
                Console.Write(label);
                var s = Console.ReadLine();

                if (int.TryParse(s, out var v))
                    return v;

                Console.WriteLine("Błąd: wpisz liczbę całkowitą.");
            }
        }

        public static decimal ReadDecimal(string label)
        {
            while (true)
            {
                Console.Write(label);
                var s = Console.ReadLine();

                if (decimal.TryParse(s, out var v))
                    return v;

                Console.WriteLine("Błąd: wpisz liczbę (np. 2.5).");
            }
        }

        public static string ReadNonEmpty(string label)
        {
            while (true)
            {
                Console.Write(label);
                var s = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(s))
                    return s;

                Console.WriteLine("Błąd: pole nie może być puste.");
            }
        }
    }
}
