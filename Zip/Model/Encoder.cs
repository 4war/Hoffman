using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class Encoder
    {
        public Dictionary<char, Figure> Execute(string message)
        {
            var countDictionary = GetCountDictionary(message);
            var tree = GetEncodingTree(countDictionary);
            var codes = GetCodes(tree);
            return codes;
        }

        public Dictionary<char, int> GetCountDictionary(string message)
        {
            var result = message
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            return result;
        }

        public Node GetEncodingTree(Dictionary<char, int> dictionary)
        {
            var unsortedLeafs = new List<Node>();
            foreach (var kvp in dictionary)
            {
                unsortedLeafs.Add(new Node() { LeafValue = kvp.Key, Count = kvp.Value });
            }

            var nodes = unsortedLeafs.OrderBy(x => x.Count).ToList();

            //Пока в массиве не останется только 1 ветка (которая будет корнем)
            for (; nodes.Count > 1;)
            {
                //Поиск первой и второй ветки с минимальной частотой 
                var first = nodes.TakeNodeByMinCount();
                var second = nodes.TakeNodeByMinCount();

                //Разделение на 1 и 0 в одной ветке
                first.EncodeDigit = false; //0
                second.EncodeDigit = true; //1 

                var parent = new Node()
                {
                    Children = new List<Node>() { first, second },
                    Count = first.Count + second.Count,
                };

                //Чтобы потом собрать код буквы
                first.Parent = parent;
                second.Parent = parent;

                //Теперь новая ветка тоже находится в общем массиве
                nodes.Add(parent);
            }

            return nodes.Single();
        }

        public Dictionary<char, Figure> GetCodes(Node tree)
        {
            var result = new Dictionary<char, Figure>();
            var stack = new Stack<Node>();
            stack.Push(tree);

            while (stack.Any())
            {
                var node = stack.Pop();

                if (node.LeafValue.HasValue)
                {
                    result[node.LeafValue.Value] = GetCodeFromLeaf(node);
                }

                foreach (var nodeChild in node.Children)
                {
                    stack.Push(nodeChild);
                }
            }

            return result;
        }

        private Figure GetCodeFromLeaf(Node node)
        {
            var newNode = node;
            byte result = 0;
            var counter = 0;
            for (; newNode.Parent != null; counter++)
            {
                if (newNode.EncodeDigit)
                {
                    result |= (byte)(1 << counter);
                }

                newNode = newNode.Parent;
            }

            return new Figure()
            {
                Size = (byte)counter,
                Value = result
            };
        }
    }
}