using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Model.Tests
{
    [TestFixture]
    public class PackerTests
    {
        private readonly string _message = "it starts with one thing";
        private readonly Dictionary<char, Figure> _codes = new Dictionary<char, Figure>()
        {
            ['r'] = new Figure(){Value = Convert.ToByte("11111", 2), Size = 5},
            ['a'] = new Figure(){Value = Convert.ToByte("11110", 2), Size = 5},
            ['n'] = new Figure(){Value = Convert.ToByte("1110", 2), Size = 4},
            ['h'] = new Figure(){Value = Convert.ToByte("1101", 2), Size = 4},
            ['s'] = new Figure(){Value = Convert.ToByte("1100", 2), Size = 4},
            [' '] = new Figure(){Value = Convert.ToByte("101", 2), Size = 3},
            ['i'] = new Figure(){Value = Convert.ToByte("100", 2), Size = 3},
            ['t'] = new Figure(){Value = Convert.ToByte("01", 2), Size = 2},
            ['g'] = new Figure(){Value = Convert.ToByte("0011", 2), Size = 4},
            ['e'] = new Figure(){Value = Convert.ToByte("0010", 2), Size = 4},
            ['o'] = new Figure(){Value = Convert.ToByte("0001", 2), Size = 4},
            ['w'] = new Figure(){Value = Convert.ToByte("0000", 2), Size = 4},
        };

        //                                               i |t|   |s  |t|a    |r   | t|s  |   |w  |i | t|h  |   |o  |n   |e  |  | t|h  |i  |n  |g   |
        private readonly string _encodedMessageString = "10001101 11000111 11011111 01110010 10000100 01110110 10001111 00010101 01110110 01110001 10000000";
        
        private Packer _packer;
        private List<byte> _encodedMessage;
        private List<byte> _expectedPackedCodes;

        [SetUp]
        public void SetUp()
        {
            _packer = new Packer(_codes);
            
            // ReSharper disable once UseObjectOrCollectionInitializer
            _expectedPackedCodes = new List<byte>();
            
            _expectedPackedCodes.Add(0x05);
            _expectedPackedCodes.Add(Convert.ToByte("11111", 2));
            _expectedPackedCodes.Add((byte)'r');
            
            _expectedPackedCodes.Add(0x05);
            _expectedPackedCodes.Add(Convert.ToByte("11110", 2));
            _expectedPackedCodes.Add((byte)'a');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("1110", 2));
            _expectedPackedCodes.Add((byte)'n');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("1101", 2));
            _expectedPackedCodes.Add((byte)'h');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("1100", 2));
            _expectedPackedCodes.Add((byte)'s');
            
            _expectedPackedCodes.Add(0x03);
            _expectedPackedCodes.Add(Convert.ToByte("101", 2));
            _expectedPackedCodes.Add((byte)' ');
            
            _expectedPackedCodes.Add(0x03);
            _expectedPackedCodes.Add(Convert.ToByte("100", 2));
            _expectedPackedCodes.Add((byte)'i');
            
            _expectedPackedCodes.Add(0x02);
            _expectedPackedCodes.Add(Convert.ToByte("01", 2));
            _expectedPackedCodes.Add((byte)'t');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("0011", 2));
            _expectedPackedCodes.Add((byte)'g');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("0010", 2));
            _expectedPackedCodes.Add((byte)'e');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("0001", 2));
            _expectedPackedCodes.Add((byte)'o');
            
            _expectedPackedCodes.Add(0x04);
            _expectedPackedCodes.Add(Convert.ToByte("0000", 2));
            _expectedPackedCodes.Add((byte)'w');

            _encodedMessage = _encodedMessageString.Split().Select(x => Convert.ToByte(x,2)).ToList();
        }
        

        [Test]
        public void Should_PackCodesDictionaryCorrectly()
        {
            var actual = _packer.PackDictionary().ToList();
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(_expectedPackedCodes[i], actual[i], $"index: {i}");
            }
        }

        [Test]
        public void Should_PackMessageCorrectly()
        {
            var actual = _packer.PackMessage(_message).ToList();
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(_encodedMessage[i], actual[i], $"index: {i}");
            }
        }

        [Test]
        public void Should_LessenSize()
        {
            var compressedDictionary = _packer.PackDictionary().ToList();
            var compressedMessage = _packer.PackMessage(_message).ToList();

            var statistics = new ZipStatistics()
            {
                MessageBytes = _message.Length,
                CompressedBytes = compressedMessage.Count,
                DictionaryBytes = compressedDictionary.Count
            };
            
            Assert.IsFalse(statistics.CompressedRatio < 0.8);
        }
    }
}