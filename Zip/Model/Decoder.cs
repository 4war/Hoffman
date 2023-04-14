using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Model.Encoders;

namespace Model
{
    public class Decoder
    {
        public IEnumerable<char> Execute(IEnumerable<byte> message, IEnumerable<byte> codes, AbstractEncoder abstractEncoder)
        {
            //Этот словарь словарей нужен для оптимизации поиска
            //Сейчас временная сложность O(1), а без него O(n)
            var dictionary = abstractEncoder.GetSizeCodeDictionary(codes);
            var result = new StringBuilder();
            byte currentCode = 0;
            byte currentSize = 0;
            foreach (var currentByte in message)
            {
                for (var bitsFromLeft = 7; bitsFromLeft >= 0; bitsFromLeft--)
                {
                    //currentCode |= (byte)((currentByte >> bitsFromLeft & 1) > 0 ? (1 << bitsFromLeft) : 0);
                    currentCode <<= 1;
                    currentCode |= (byte)(currentByte >> bitsFromLeft & 1);
                    currentSize++;
                    if (dictionary.ContainsKey(currentSize))
                    {
                        if (dictionary[currentSize].ContainsKey(currentCode))
                        {
                            yield return abstractEncoder.Decode(dictionary[currentSize][currentCode]);
                            currentCode = 0;
                            currentSize = 0;
                        }
                    }

                    if (currentSize > 8)
                    {
                        throw new ArgumentException(
                            "Размер закодированной буквы больше 8 бит или ошибка в переданных аргументах");
                    }
                }
            }

            if (currentCode > 0)
            {
                throw new ArgumentException("Не удалось декодировать последний символ");
            }
        }

        public IEnumerable GetSizeCodeDictionary(List<byte> packedCodes, AbstractEncoder abstractEncoder)
        {
            return abstractEncoder.GetSizeCodeDictionary(packedCodes);
        }
    }
}