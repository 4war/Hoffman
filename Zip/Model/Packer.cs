using System;
using System.Collections.Generic;

namespace Model
{
    public class Packer
    {
        private readonly Dictionary<char, Figure> _codes;

        public Packer(Dictionary<char, Figure> codes)
        {
            _codes = codes;
        }

        //Пакую СЛЕВА НА ПРАВО, это важно!!!
        public IEnumerable<byte> PackMessage(string message)
        {
            //От 8 до 0, затем снова 8
            var counter = 7;
            byte currentByte = 0;
            foreach (var c in message)
            {
                var figure = _codes[c];
                for (var i = figure.Size - 1; i >= 0; i--)
                {
                    currentByte |= (byte)(((figure.Value >> i) & 1) > 0 ? 1 << counter : 0);
                    counter--;
                    if (counter < 0)
                    {
                        yield return currentByte;
                        counter = 7;
                        currentByte = 0;
                    }
                  
                }
            }

            if (counter != 7)
            {
                yield return currentByte;
            }
        }

        public IEnumerable<byte> PackDictionary()
        {
            foreach (var kvp in _codes)
            {
                //Формат: 0000 ssss | xxxx xxxx | yyyy yyyy
                //yy - общий код буквы 1 байт 
                //xx - код буквы по алгоритму сжатия
                //ss - длина кода буквы (иначе нельзя определить 000 или 00)
                //Это три отдельных байта, идущий друг за другом
                yield return kvp.Value.Size;
                yield return kvp.Value.Value;
                yield return (byte)kvp.Key;
            }
        }
    }
}