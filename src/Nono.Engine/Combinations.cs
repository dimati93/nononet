using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Nono.Engine.Extensions;

using static Nono.Engine.Constraints;

namespace Nono.Engine
{
    public static class Combinations
    {
        private static readonly ConcurrentDictionary<(int, int), decimal> ZippedBlocksCountCache
            = new ConcurrentDictionary<(int, int), decimal>();

        /// <summary>
        /// Calculate count of all combinations of n blocks of size 1 fit in length with spaces of size 1
        /// </summary>
        public static decimal CountZippedBlocks(int n, int length)
        {
            decimal Count((int n, int length) arg)
            {
                if (arg.n == 1)
                    return arg.length;

                int leftN = arg.n >> 1;
                int rightN = arg.n - leftN;
                int minL = (leftN << 1) - 1;
                int minR = (rightN << 1) - 1;

                int moves = arg.length - minR - minL;
                int maxR = minR + moves - 1;

                var leftCounts = Enumerable.Range(minL, moves).Select(x => CountZippedBlocks(leftN, x));
                var rightCounts = Enumerable.Range(-maxR, moves).Select(x => CountZippedBlocks(rightN, -x));

                return Enumerable.Zip(Derivative(leftCounts), rightCounts, (dleft, right) => dleft * right).Sum();
            }

            return ZippedBlocksCountCache.GetOrAdd((n, length), Count);
        }

        private static IEnumerable<decimal> Derivative(IEnumerable<decimal> enumerable, decimal start = 0)
        {
            decimal prev = start;
            foreach (var item in enumerable)
            {
                yield return item - prev;
                prev = item;
            }
        }

        /// <summary>
        /// Calculates how many combinations of spans (thus block positions) available for a specific cues for a line length
        /// Helps to prioritize reduce operations before calculating actual spans

        /// Uses divide &amp; conquer strategy by dividing line in 2 and calculating respective counts in left and right parts.
        /// Than assembles results by multiplying.

        /// Also reduces all blocks in cues to length of 1 and space to length of 1.
        /// </summary>
        public static decimal Count(ReadOnlySpan<int> cues, int length)
        {
            var extraBlockSpace = (MIN_BLOCK_SPACE - 1) * (cues.Length - 1);
            var extraBlockLength = cues.Sum() - cues.Length;
            return CountZippedBlocks(cues.Length, (length - extraBlockSpace - extraBlockLength));
        }

        public static bool IsHot(ReadOnlySpan<int> cues, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            var maxBlock = cues.Length > 0 ? cues.Max() : 0;
            return Moves(cues, length) < maxBlock;
        }

        public static int Moves(ReadOnlySpan<int> cues, int length)
        {
            unchecked
            {
                return length - cues.Sum() - MIN_BLOCK_SPACE * (cues.Length - 1);
            }
        }
    }
}