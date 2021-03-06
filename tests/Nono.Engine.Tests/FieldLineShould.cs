using System.Linq;
using FluentAssertions;
using Nono.Engine.Tests.Extensions;
using Xunit;

namespace Nono.Engine.Tests
{
    public class FieldLineShould
    {
        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(1, new int[] { 0 })]
        [InlineData(4, new int[] { 1, 2, 0, 3 })]
        [InlineData(5, new int[] { 2, 1, 3, 0, 4 })]
        public void GenerateIndexFromCenter(int length, int[] expectedIndexes)
        {
            var indexes = FieldLineExtensions.IndexFromCenter(length).ToArray();

            indexes.Should().Equal(expectedIndexes);
        }

        [Theory]
        [InlineData("  00   ", 2, 4)]
        [InlineData("0000   ", 0, 4)]
        [InlineData("   0000", 3, 7)]
        [InlineData("0000000", 0, 7)]
        [InlineData("111111", -1, -1)]
        public void FindCenterBlock(string line, int expectedStart, int expectedLength)
        {
            var (start, length) = line.AsSpan().FindCenterBlock(Box.Crossed);

            start.Should().Be(expectedStart);
            length.Should().Be(expectedLength);
        }


        [Theory]
        [InlineData(" 00 ", " 00 ", "    ")]
        [InlineData(" 00 ", "100 ", "1   ")]
        [InlineData(" 00 ", "1000", "1  0")]
        [InlineData("    ", "1000", "1000")]
        public void DiffLines(string field, string collapsed, string diff)
        {
            var index = new LineIndex(Orientation.Row, 7);
            var fieldLine = new FieldLine(field.AsBoxEnumerable(), index);

            var diffLine = fieldLine.Diff(collapsed.AsBoxEnumerable());

            diffLine.Should().Equal(diff.AsBoxEnumerable());
            diffLine.Index.Should().Be(index);
        }
    }
}