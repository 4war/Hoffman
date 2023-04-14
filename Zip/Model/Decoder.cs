using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Model.Encoders;

namespace Model
{
    /// <summary>
    /// Отвечает за распаковку сообщения
    /// </summary>
    public class Decoder
    {
        /// <summary>
        /// Распаковывает сжатое сообщение
        /// </summary>
        /// <param name="message">сжатое сообщение</param>
        /// <param name="codes">словарь</param>
        /// <param name="abstractEncoder">кодировка</param>
        /// <returns>Распакованное сообщение, которое должно соответствовать исходному сообщению пользователя</returns>
        public IEnumerable<char> Execute(IEnumerable<byte> message, IEnumerable<byte> codes, AbstractEncoder abstractEncoder)
        {
            //Этот словарь словарей нужен для оптимизации поиска
            //Сейчас временная сложность O(1), а без него O(n)
            var dictionary = abstractEncoder.GetSizeCodeDictionary(codes);
            
            byte currentCode = 0; //В этот байт будет записываться код до тех пор,
                                  //пока не совпадет с каким-нибудь кодом из словаря
                                  
            byte currentSize = 0; //Чтобы каждую итерацию не искать совпадения с каждой буквой символа
                                  //я проверяю только те коды, чья длина равна текущей длине (для оптимизации)

            foreach (var currentByte in message) //В цикле беру каждый бит 
            {
                for (var bitsFromLeft = 7; bitsFromLeft >= 0; bitsFromLeft--) //Слева на право беру биты текущего байта
                {
                    currentCode <<= 1; //Освобождаю место для записи нового бита (Если байт пустой - ничего не произойдет)
                    currentCode |= (byte)(currentByte >> bitsFromLeft & 1); //Добавляю бит из текущего байта
                    currentSize++; 
                    if (dictionary.ContainsKey(currentSize)) //Поиск кода по длине: временная сложность O(1)
                    {
                        if (dictionary[currentSize].ContainsKey(currentCode)) //Поиск самого кода: временная сложность O(1)
                        {
                            //Если нашелся код - декодирую и возвращаю
                            yield return abstractEncoder.Decode(dictionary[currentSize][currentCode]); 
                            currentCode = 0; //Обновляю код
                            currentSize = 0; //Обновляю длину кода
                        }
                    }
                }
            }

            if (currentCode > 0)
            {
                throw new ArgumentException("Не удалось декодировать последний символ");
            }
        }

        /// <summary>
        /// Для тестов
        /// </summary>
        public IEnumerable GetSizeCodeDictionary(List<byte> packedCodes, AbstractEncoder abstractEncoder)
        {
            return abstractEncoder.GetSizeCodeDictionary(packedCodes);
        }
    }
}