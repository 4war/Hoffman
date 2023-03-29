using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Model.Tests
{
    [TestFixture]
    public class DecoderTests
    {
        private readonly Dictionary<byte, Dictionary<byte, byte>> _expectedCodes =
            new Dictionary<byte, Dictionary<byte, byte>>()
            {
                [2] = new Dictionary<byte, byte>()
                {
                    [1] = (byte)'t',
                },
                [3] = new Dictionary<byte, byte>()
                {
                    [5] = (byte)' ',
                    [4] = (byte)'i'
                },
                [4] = new Dictionary<byte, byte>()
                {
                    [Convert.ToByte("1110", 2)] = (byte)'n',
                    [Convert.ToByte("1101", 2)] = (byte)'h',
                    [Convert.ToByte("1100", 2)] = (byte)'s',
                    [Convert.ToByte("0011", 2)] = (byte)'g',
                    [Convert.ToByte("0010", 2)] = (byte)'e',
                    [Convert.ToByte("0001", 2)] = (byte)'o',
                    [Convert.ToByte("0000", 2)] = (byte)'w',
                },
                [5] = new Dictionary<byte, byte>()
                {
                    [Convert.ToByte("11111", 2)] = (byte)'r',
                    [Convert.ToByte("11110", 2)] = (byte)'a',
                }
            };
        
        //                                               i |t|   |s  |t|a    |r   | t|s  |   |w  |i | t|h  |   |o  |n   |e  |  | t|h  |i  |n  |g   |
        private readonly string _encodedMessageString = "10001101 11000111 11011111 01110010 10000100 01110110 10001111 00010101 01110110 01110001 10000000";
        private readonly string _expectedMessage = "it starts with one thing";
        private readonly Decoder _decoder = new Decoder();

        private List<byte> _encodedMessage;
        private List<byte> _packedCodes;
        
            [SetUp]
        public void SetUp()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            _packedCodes = new List<byte>();
            
            _packedCodes.Add(0x05);
            _packedCodes.Add(Convert.ToByte("11111", 2));
            _packedCodes.Add((byte)'r');
            
            _packedCodes.Add(0x05);
            _packedCodes.Add(Convert.ToByte("11110", 2));
            _packedCodes.Add((byte)'a');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("1110", 2));
            _packedCodes.Add((byte)'n');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("1101", 2));
            _packedCodes.Add((byte)'h');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("1100", 2));
            _packedCodes.Add((byte)'s');
            
            _packedCodes.Add(0x03);
            _packedCodes.Add(Convert.ToByte("101", 2));
            _packedCodes.Add((byte)' ');
            
            _packedCodes.Add(0x03);
            _packedCodes.Add(Convert.ToByte("100", 2));
            _packedCodes.Add((byte)'i');
            
            _packedCodes.Add(0x02);
            _packedCodes.Add(Convert.ToByte("01", 2));
            _packedCodes.Add((byte)'t');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("0011", 2));
            _packedCodes.Add((byte)'g');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("0010", 2));
            _packedCodes.Add((byte)'e');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("0001", 2));
            _packedCodes.Add((byte)'o');
            
            _packedCodes.Add(0x04);
            _packedCodes.Add(Convert.ToByte("0000", 2));
            _packedCodes.Add((byte)'w');

            _encodedMessage = _encodedMessageString.Split().Select(x => Convert.ToByte(x,2)).ToList();
        }
        
        [Test]
        public void Should_GetCorrectCodesDictionary()
        {
            var actual = _decoder.GetSizeCodeDictionary(_packedCodes);
            CollectionAssert.AreEquivalent(_expectedCodes, actual);
        }

        [Test]
        public void Should_Decode()
        {
            var actualEnumerable = _decoder.Execute(_encodedMessage, _packedCodes);
            var actualString = string.Join("", actualEnumerable);
            Assert.AreEqual(_expectedMessage, actualString);
        }
    }
}