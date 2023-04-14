using System.Collections.Generic;
using System.Linq;

namespace Model.Encoders
{
    /// <summary>
    /// Пользователь сам выбирает кодировку
    /// </summary>
    public abstract class AbstractEncoder
    {        
        public abstract string Name { get; }
        public abstract string SizeString { get; }
        public abstract int Size { get; }
        
        public abstract byte[] Pack(char c);
        public abstract char Decode(byte[] bytes);

        /// <summary>
        /// Формирует словарь по длине - коду - букве для оптимизации
        /// При этом буква может состоять как из 1, так и из 2 байт
        /// </summary>
        /// <param name="codes">плоский словарь</param>
        /// <returns>Словарь по длине - коду - букве</returns>
        public Dictionary<byte, Dictionary<byte, byte[]>> GetSizeCodeDictionary(IEnumerable<byte> codes)
        {
            var result = new Dictionary<byte, Dictionary<byte, byte[]>>();
            var figure = new List<byte>();
            foreach (var something in codes)
            {
                figure.Add(something);
                if (figure.Count == 2 + Size)
                {
                    if (!result.ContainsKey(figure[0]))
                    {
                        result[figure[0]] = new Dictionary<byte, byte[]>();
                    }
                    
                    result[figure[0]][figure[1]] = figure.Skip(2).Take(Size).ToArray();
                    figure.Clear();
                }
            }

            return result;
        }
    }
}