using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Ветка дерева по алгоритму Хаффмана
    /// </summary>
    public class Node
    {
        public int Count { get; set; } //Суммарная частота
        public bool EncodeDigit { get; set; } //0 или 1 - на текущей ветке
        public char? LeafValue { get; set; } //Буква (только в случае листа)

        public Node Parent { get; set; } //Родительский элемент
        public List<Node> Children { get; set; } = new List<Node>(); //Дочерние элементы
    }
}