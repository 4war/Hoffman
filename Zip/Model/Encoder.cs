using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    /// <summary>
    /// Отвечает за генерацию словаря, но не за его сжатие
    /// </summary>
    public class Encoder
    {
        /// <summary>
        /// Создает словарь на основе исходного сообщения с учетом частот букв
        /// </summary>
        /// <param name="message">Исходное сообщение</param>
        /// <returns>Словарь букв</returns>
        public Dictionary<char, Figure> Execute(string message)
        {
            var countDictionary = GetFrequencies(message); //Частоты букв (Абсолютное значение)
            var tree = GetEncodingTree(countDictionary); //Дерево
            var codes = GetCodes(tree); //Словарь букв
            return codes;
        }

        /// <summary>
        /// Возвращает словарь количество каждых встречающихся букв в исходном сообщении
        /// Чтобы получить словарь частот, нужно просто полученные значения разделить на длину сообщения
        /// </summary>
        /// <param name="message">Исходное сообщение</param>
        /// <returns>Словарь частот букв</returns>
        public Dictionary<char, int> GetFrequencies(string message)
        {
            var result = message
                .GroupBy(x => x) // группирую по буквам 
                .ToDictionary(x => x.Key, x => x.Count()); //считаю количество в каждой группе

            return result;
        }


        /// <summary>
        /// Создает дерево по алгоритму Хаффмана
        /// </summary>
        /// <param name="dictionary">Словарь частот букв</param>
        /// <returns>Корень дерева</returns>
        public Node GetEncodingTree(Dictionary<char, int> dictionary)
        {
            //Изначально каждая уникальная буква - корень своего дерева из 1 элемента
            //То есть сколько уникальных букв - столько и деревьев
            //А потом эти деревья объединяются, пока это не станет одним деревом
            var unsortedLeafs = new List<Node>();
            foreach (var kvp in dictionary)
                unsortedLeafs.Add(new Node() { LeafValue = kvp.Key, Count = kvp.Value });
            
            //Сортирую буквы по их частоте
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

                //Чтобы затем получить код буквы, нужно хранить информацию о родителе текущей ветки
                var parent = new Node()
                {
                    Children = new List<Node>() { first, second },
                    Count = first.Count + second.Count,
                };
                first.Parent = parent;
                second.Parent = parent;

                //Теперь новая ветка тоже находится в общем массиве
                nodes.Add(parent);
            }

            //Утверждаю, что осталась единственная нераспределенная ветка, которая является корнем
            //Если это не так, программа выбросит ошибку
            return nodes.Single();
        }

        /// <summary>
        /// Собирает словарь по созданному дереву
        /// </summary>
        /// <param name="tree">Дерево по алгоритму Хаффмана</param>
        /// <returns>Словарь букв</returns>
        public Dictionary<char, Figure> GetCodes(Node tree)
        {
            var result = new Dictionary<char, Figure>();
            var stack = new Stack<Node>();
            
            //Стек - для обхода в глубину, очередь - для обхода в ширину. Выбрал стек
            stack.Push(tree);

            while (stack.Any()) //Пока в стеке есть элементы
            {
                var node = stack.Pop(); //Беру верхний элемент стека

                if (node.LeafValue.HasValue) //Если это лист (буква)
                    result[node.LeafValue.Value] = GetCodeFromLeaf(node); //Получаю код буквы и добавляю в конечный словарь
                
                foreach (var nodeChild in node.Children) //Каждый дочерний элемент заносится в стек
                    stack.Push(nodeChild);
            }

            return result;
        }

        /// <summary>
        /// Собирает код буквы по дереву
        /// </summary>
        /// <param name="node">Лист</param>
        /// <returns>Код буквы, длина кода, кодировка буквы</returns>
        private Figure GetCodeFromLeaf(Node node)
        {
            var newNode = node;
            byte result = 0;
            var counter = 0;
            
            //Поднимаюсь от листа до корня
            for (; newNode.Parent != null; counter++)
            {
                if (newNode.EncodeDigit) //Если код текущей ветки соответствует 1
                {
                    result |= (byte)(1 << counter);
                } //Если 0 - то ничего не делаю

                newNode = newNode.Parent; //Перемещаюсь вверх к следующей ветке
            }

            //Единственный случай во всем словаре, когда буква полностью состоит из нулей
            //Например w = "0000". В таком случае при декодировании может остаться от 0 до 7 нулей,
            //которые заполняют ПОСЛЕДНИЙ байт, алгоритм посчитает эти нули за существующую букву 'w',
            //которой в исходном сообщении не было (или была, но проверить это нельзя)
            //поэтому я добавляю 1 в конец и получается w = "00001"
            //причем абсолютно точно этот новый код не пересекается с другими кодами, так как "0000" уже удовлетворял
            //всем условиям
            if (result == 0) 
            {
                counter++;
                result = 1;
            }

            return new Figure()
            {
                Size = (byte)counter,
                Value = result
            };
        }
    }
}