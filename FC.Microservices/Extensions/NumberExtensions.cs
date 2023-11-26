namespace FCMicroservices.Extensions;

public static class NumberExtensions
{
    public static string RandomCitizenId(int seed = 0)
    {
        var str = seed.ToString("00000000#");

        if (seed == 0)
        {
            str = DateTime.Now.Ticks.ToString().Substring(9, 9);
        }

        var ticks = int.Parse(str);

        int[] no = new int[11];
        for (int i = 8; i >= 0; i--)
        {
            no[i] = (int)(ticks % 10);
            ticks /= 10;
        }

        no[9] = ((no[0] + no[2] + no[4] + no[6] + no[8]) * 7 - (no[1] + no[3] + no[5] + no[7])) %
                  10;

        no[10] = (no[0] + no[1] + no[2] + no[3] + no[4]
                    + no[5] + no[6] + no[7] + no[8] + no[9]) % 10;

        var result = (string.Join("", no));
        return result;
    }

    public static long Transform(long originalNumber)
    {
        originalNumber ^= MASK;
        long[] digits = new long[DIGITS];

        for (int i = 0; i < DIGITS; i++)
        {
            digits[i] = originalNumber % 10;
            originalNumber /= 10;
        }

        for (int i = 1; i < DIGITS; i++)
        {
            digits[i] = (digits[i] + digits[i - 1]) % MOD;
        }

        digits[0] = (digits[0] + digits[DIGITS - 1]) % MOD;


        long transformed = 0;
        int multiplier = 1;
        for (int i = 0; i < DIGITS; i++)
        {
            transformed += digits[i] * multiplier;
            multiplier *= 10;
        }

        return transformed;
    }

    public static long Revert(long transformedNumber)
    {
        long[] digits = new long[DIGITS];

        for (int i = 0; i < DIGITS; i++)
        {
            digits[i] = transformedNumber % 10;
            transformedNumber /= 10;
        }

        digits[0] = (digits[0] - digits[DIGITS - 1] + 10) % MOD;
        for (int i = DIGITS - 1; i > 0; i--)
        {
            digits[i] = (digits[i] - digits[i - 1] + 10) % MOD;
        }

        long original = 0;
        int multiplier = 1;
        for (int i = 0; i < DIGITS; i++)
        {
            original += digits[i] * multiplier;
            multiplier *= 10;
        }

        return original ^ MASK;
    }

    const int DIGITS = 10;
    const int MOD = 10;
    const long MASK = 92647_35810;

}