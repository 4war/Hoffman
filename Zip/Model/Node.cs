using System.Collections.Generic;

namespace Model
{
    public class Node
    {
        public int Count { get; set; }
        public bool EncodeDigit { get; set; }
        public char? LeafValue { get; set; }

        public Node Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
    }
}