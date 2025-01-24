using System.Text;
using Fuyu.Common.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fuyu.Tests.Common.Units;

[TestClass]
public class ZlibTest
{
    [TestMethod]
    public void TestSmallZip()
    {
        // Initializing
        var testString = "Testing Zlib with some string _ AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA SSSSSSSSSSSSSSSSSS FFFFFFFFFFFF VVVVVVVVVVVVVVVVVVVVVVVV 234534545";
        var bytes = Encoding.UTF8.GetBytes(testString);
        Assert.AreEqual(Encoding.UTF8.GetString(bytes), testString);

        // compressing
        var compressedBytes = MemoryZlib.Compress(bytes, System.IO.Compression.CompressionLevel.SmallestSize);
        Assert.IsNotNull(compressedBytes);
        Assert.AreNotEqual(0, compressedBytes.Length);

        // decompressing
        var decompressedBytes = MemoryZlib.Decompress(compressedBytes);
        Assert.IsNotNull(decompressedBytes);
        Assert.AreNotEqual(0, decompressedBytes.Length);

        // check if decompressed and original is same
        Assert.AreEqual(Encoding.UTF8.GetString(decompressedBytes), testString);
        Assert.AreEqual(Encoding.UTF8.GetString(bytes), Encoding.UTF8.GetString(decompressedBytes));
    }
}