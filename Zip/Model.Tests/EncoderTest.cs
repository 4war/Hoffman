using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Model.Tests
{
    [TestFixture]
    public class HoffmanTests
    {
        private readonly Encoder _encoder = new Encoder();

        private Dictionary<char, int> _expectedDictionary;
        private Node _expectedTree;
        private Dictionary<char, Figure> _expectedCodes;
        private readonly string _message = "it starts with one thing";
        
        [SetUp]
        public void SetUp()
        {
            _expectedDictionary = new Dictionary<char, int>()
            {
                ['i'] = 3,
                ['t'] = 5,
                [' '] = 4,
                ['s'] = 2,
                ['a'] = 1,
                ['r'] = 1,
                ['w'] = 1,
                ['h'] = 2,
                ['o'] = 1,
                ['n'] = 2,
                ['e'] = 1,
                ['g'] = 1
            };

            _expectedTree = new Node()
            {
                Count = 24,
                Children = new List<Node>()
                {
                    new Node()
                    {
                        Count = 9,
                        EncodeDigit = false,
                        Children = new List<Node>()
                        {
                            new Node()
                            {
                                Count = 4,
                                EncodeDigit = false,
                                Children = new List<Node>()
                                {
                                    new Node()
                                    {
                                        Count = 2,
                                        EncodeDigit = false,
                                        Children = new List<Node>()
                                        {
                                            new Node()
                                            {
                                                Count = 1,
                                                EncodeDigit = false,
                                                LeafValue = 'w'
                                            },
                                            new Node()
                                            {
                                                Count = 1,
                                                EncodeDigit = true,
                                                LeafValue = 'o'
                                            }
                                        }
                                    },
                                    new Node()
                                    {
                                        Count = 2,
                                        EncodeDigit = true,
                                        Children = new List<Node>()
                                        {
                                            new Node()
                                            {
                                                Count = 1,
                                                EncodeDigit = false,
                                                LeafValue = 'e'
                                            },
                                            new Node()
                                            {
                                                Count = 1,
                                                EncodeDigit = true,
                                                LeafValue = 'g'
                                            }
                                        }
                                    }
                                }
                            },
                            new Node()
                            {
                                Count = 5,
                                EncodeDigit = true,
                                LeafValue = 't'
                            }
                        }
                    },
                    new Node()
                    {
                        Count = 15,
                        EncodeDigit = true,
                        Children = new List<Node>()
                        {
                            new Node()
                            {
                                Count = 7,
                                EncodeDigit = false,
                                Children = new List<Node>()
                                {
                                    new Node()
                                    {
                                        Count = 3,
                                        EncodeDigit = false,
                                        LeafValue = 'i'
                                    },
                                    new Node()
                                    {
                                        Count = 4,
                                        EncodeDigit = true,
                                        LeafValue = ' '
                                    }
                                }
                            },
                            new Node()
                            {
                                Count = 8,
                                EncodeDigit = true,
                                Children = new List<Node>()
                                {
                                    new Node()
                                    {
                                        Count = 4,
                                        EncodeDigit = false,
                                        Children = new List<Node>()
                                        {
                                            new Node()
                                            {
                                                Count = 2,
                                                EncodeDigit = false,
                                                LeafValue = 's'
                                            },
                                            new Node()
                                            {
                                                Count = 2,
                                                EncodeDigit = true,
                                                LeafValue = 'h'
                                            },
                                        }
                                    },
                                    new Node()
                                    {
                                        Count = 4,
                                        EncodeDigit = true,
                                        Children = new List<Node>()
                                        {
                                            new Node()
                                            {
                                                Count = 2,
                                                EncodeDigit = false,
                                                LeafValue = 'n'
                                            },
                                            new Node()
                                            {
                                                Count = 2,
                                                EncodeDigit = true,
                                                Children = new List<Node>()
                                                {
                                                    new Node()
                                                    {
                                                        Count = 1,
                                                        EncodeDigit = false,
                                                        LeafValue = 'a'
                                                    },
                                                    new Node()
                                                    {
                                                        Count = 1,
                                                        EncodeDigit = true,
                                                        LeafValue = 'r'
                                                    }
                                                }
                                            },
                                        }
                                    },
                                }
                            },
                        }
                    },
                }
            };

            _expectedCodes = new Dictionary<char, Figure>()
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
        }

        [Test]
        public void Should_GetCorrectCountDictionary()
        {
            var actualDictionary = _encoder.GetCountDictionary(_message);
            CollectionAssert.AreEqual(_expectedDictionary, actualDictionary);
        }

        [Test]
        public void Should_BuildCorrectTree()
        {
            var actualTree = _encoder.GetEncodingTree(_expectedDictionary);
            var actual = FlattenTree(actualTree);
            var expected = FlattenTree(_expectedTree);
            AssertTreesAreEqual(expected, actual);
        }

        public void AssertTreesAreEqual(HashSet<Node> expected, HashSet<Node> actual)
        {
            foreach (var node in expected)
            {
                var actualNode = actual.SingleOrDefault(x =>
                    x.EncodeDigit == node.EncodeDigit &&
                    x.Count == node.Count &&
                    x.LeafValue == node.LeafValue &&
                    x.Children.Zip(node.Children, (a, b) => a.Count == b.Count && a.LeafValue == b.LeafValue)
                        .All(t => t)
                );

                Assert.IsNotNull(actualNode, $"Count: {node.Count}, " +
                                             $"LeafValue: {node.LeafValue ?? ' '}, " +
                                             $"EncodeDigit: {node.EncodeDigit}");
            }
        }

        private HashSet<Node> FlattenTree(Node tree)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            stack.Push(tree);

            while (stack.Any())
            {
                var node = stack.Pop();
                visited.Add(node);

                foreach (var nodeChild in node.Children.Reverse<Node>())
                {
                    stack.Push(nodeChild);
                }
            }

            return visited;
        }

        [Test]
        public void Should_GetCorrectCodes()
        {
            var expectedTree = _encoder.GetEncodingTree(_expectedDictionary);
            var actualCodes = _encoder.GetCodes(expectedTree);
            CollectionAssert.AreEquivalent(_expectedCodes, actualCodes);
        }

        [Test]
        public void Should_Encode()
        {
            var actualCodes = _encoder.Execute(_message);
            CollectionAssert.AreEquivalent(_expectedCodes, actualCodes);
        }
    }
}