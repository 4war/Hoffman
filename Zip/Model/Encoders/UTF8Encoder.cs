using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Encoders
{
    public class Utf8Encoder : AbstractEncoder
    {
        public override string Name => "UTF-8";
        public override string SizeString => "2 байта";
        public override int Size => 2;

        public override byte[] Pack(char c)
        {
            var result = Encoding.UTF8.GetBytes(new[] { c });
            if (result.Length != 2)
                throw new ArgumentException();

            return result;
        }

        public override char Decode(byte[] bytes)
        {
            var c = Encoding.UTF8.GetChars(bytes);
            if (c.Length != 1)
                throw new ArgumentException();

            return c.Single();
        }
    }
}