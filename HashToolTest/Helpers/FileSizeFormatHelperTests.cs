using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashTool.Helpers.Tests
{
    [TestClass()]
    public class FileSizeFormatHelperTests
    {
        [TestMethod()]
        [Timeout(500)]
        [DataRow(0, "0")]
        [DataRow(123, "123.00 B")]
        [DataRow(1024 * 65, "65.00 KiB")]
        [DataRow(1024 * 65 + 123, "65.12 KiB")]
        [DataRow(1L << 62, "4.00 EiB")]
        [DataRow((1L << 62) + 123, "4.00 EiB")]
        public void Format_WithValidSize_GetFormattedFileSize(long size, string expected)
        {
            var result = FileSizeFormatHelper.Format(size);

            Assert.AreEqual(expected, result, false, $"{size} is not formatted correctly.");
        }

        [TestMethod()]
        [Timeout(200)]
        [DataRow(-10)]
        public void Format_WithSizeIsLessThanZero_ThrowArgumentOutOfRangeException(long size)
        {
            var action = () => FileSizeFormatHelper.Format(size);
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }
    }
}
