using System;
using System.Collections.Generic;
using Model.Encoders;

namespace Model
{
    /// <summary>
    /// Отвечает за сжатие сообщение и словаря
    /// </summary>
    public class Packer
    {
        private readonly Dictionary<char, Figure> _codes;

        public Packer(Dictionary<char, Figure> codes)
        {
            _codes = codes;
        }

        //Пакую СЛЕВА НА ПРАВО, это важно!!!
        /// <summary>
        /// Возвращает ленивый массив байтов сжатого сообщение 
        /// </summary>
        /// <param name="message">исходное сообщение</param>
        /// <returns>Ленивый массив байтов сжатого сообщение</returns>
        public IEnumerable<byte> PackMessage(string message)
        {
            var counter = 7; //меняется от 7 до 0, затем снова 7, когда байт заканчивается
            byte currentByte = 0; //Текущий байт, в который записываются буквы или часть буквы
            
            foreach (var c in message) //Цикл по каждой букве сообщения
            {
                var figure = _codes[c]; //Узнаю код и длину буквы из переданного словаря
                for (var i = figure.Size - 1; i >= 0; i--) //Слева на право считываю биты кода текущей буквы
                {
                    //Если очередной бит в коде == 1, то записываю его в текущий байт на позицию 'counter' (тоже слева на право) 
                    //                      |   узнаю бит              | записываю в байт |
                    currentByte |= (byte)(((figure.Value >> i) & 1) > 0 ? 1 << counter : 0);

                    counter--; //указатель передвигается ВПРАВО на 1
                    if (counter < 0) //Если байт заполнился
                    {
                        yield return currentByte; //возвращаю этот байт
                        counter = 7; //переношу указатель на левый бит нового байта
                        currentByte = 0; //обновляю байт
                    }
                }
            }

            if (counter != 7) //Если сообщение закончилось, но байт ещё не заполнился
            {
                yield return currentByte;
            }
        }

        /// <summary>
        /// Возвращает ленивый массив байт словаря
        /// </summary>
        /// <param name="abstractEncoder">Кодировка</param>
        /// <returns>Ленивый массив байт словаря</returns>
        public IEnumerable<byte> PackDictionary(AbstractEncoder abstractEncoder)
        {
            foreach (var kvp in _codes)
            {
                //Формат: 0000 ssss | xxxx xxxx | aaaa aaaa - ASCII
                //Формат: 0000 ssss | xxxx xxxx | uuuu uuuu | uuuu uuuu - Unicode
                //aaaa или uuuu - код ASCII или Unicode
                yield return kvp.Value.Size; //ss - длина кода буквы (иначе нельзя определить 000 или 00)
                yield return kvp.Value.Value; //xx - код буквы по алгоритму сжатия
                foreach (var b in abstractEncoder.Pack(kvp.Key)) //Логика кодирования буквы своя у каждого кодировщика
                {
                    yield return b; //aaaa или uuuu
                }
                //Это три-четыре отдельных байта, идущий друг за другом
            }
        }
    }
}