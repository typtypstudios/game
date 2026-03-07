using System;

public static class RangeProvider
{
    /// <summary>
    /// Fills a pre-allocated buffer with a range of numbers, each repeated N times.
    /// </summary>
    /// <param name="start">The inclusive starting number.</param>
    /// <param name="end">The inclusive ending number.</param>
    /// <param name="repetitions">How many times each number should appear.</param>
    /// <exception cref="ArgumentException">Thrown if the buffer is too small for the requested range.</exception>
    public static int[] FillRepeatedRange(int start, int end, int repetitions)
    {
        int rangeCount = end - start + 1;
        int totalRequired = rangeCount * repetitions;

        int[] buffer = new int[totalRequired];

        // Fill the buffer sequentially
        for (int r = 0; r < repetitions; r++)
        {
            int offset = r * rangeCount;
            for (int i = 0; i < rangeCount; i++)
            {
                buffer[offset + i] = start + i;
            }
        }

        return buffer;
    }
}