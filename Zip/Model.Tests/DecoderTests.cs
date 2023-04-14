using System;
using System.Collections.Generic;
using System.Linq;
using Model.Encoders;
using NUnit.Framework;

namespace Model.Tests
{
    [TestFixture]
    public class DecoderTests
    {
        private readonly Dictionary<byte, Dictionary<byte, byte[]>> _expectedCodes =
            new Dictionary<byte, Dictionary<byte, byte[]>>()
            {
                [2] = new Dictionary<byte, byte[]>()
                {
                    [1] = new []{(byte)'t'},
                },
                [3] = new Dictionary<byte, byte[]>()
                {
                    [5] =  new []{(byte)' '},
                    [4] =  new []{(byte)'i'}
                },
                [4] = new Dictionary<byte, byte[]>()
                {
                    [Convert.ToByte("1110", 2)] = new []{(byte)'n'},
                    [Convert.ToByte("1101", 2)] =  new []{(byte)'h'},
                    [Convert.ToByte("1100", 2)] = new []{(byte)'s'},
                    [Convert.ToByte("0011", 2)] =  new []{(byte)'g'},
                    [Convert.ToByte("0010", 2)] =  new []{(byte)'e'},
                    [Convert.ToByte("0001", 2)] =  new []{(byte)'o'},
                },
                [5] = new Dictionary<byte, byte[]>()
                {
                    [Convert.ToByte("11111", 2)] =  new []{(byte)'r'},
                    [Convert.ToByte("11110", 2)] = new []{(byte)'a'},
                    [Convert.ToByte("00001", 2)] =  new []{(byte)'w'},
                }
            };
        
        //                                               i |t|   |s  |t|a    |r   | t|s  |   |w   |i  |t|h  |   |o  |n   |e  |  | t|h  |i  |n  |g   |
        private readonly string _encodedMessageString = "10001101 11000111 11011111 01110010 10000110 00111011 01000111 10001010 10111011 00111000 11000000";
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
            
            _packedCodes.Add(0x05);
            _packedCodes.Add(Convert.ToByte("00001", 2));
            _packedCodes.Add((byte)'w');

            _encodedMessage = _encodedMessageString.Split().Select(x => Convert.ToByte(x,2)).ToList();
        }
        
        [Test]
        public void Should_GetCorrectCodesDictionary()
        {
            var actual = _decoder.GetSizeCodeDictionary(_packedCodes, new AsciiAbstractEncoder());
            CollectionAssert.AreEquivalent(_expectedCodes, actual);
        }

        [Test]
        public void Should_Decode()
        {
            var actualEnumerable = _decoder.Execute(_encodedMessage, _packedCodes, new AsciiAbstractEncoder());
            var actualString = string.Join("", actualEnumerable);
            Assert.AreEqual(_expectedMessage, actualString);
        }
    }
}