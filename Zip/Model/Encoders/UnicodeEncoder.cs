using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Encoders
{
    public class UnicodeAbstractEncoder : AbstractEncoder
    {
        public override string Name => "Unicode";
        public override string SizeString => "2 байта";
        public override int Size => 2;

        public override byte[] Pack(char c)
        {
            var result = Encoding.Unicode.GetBytes(new[] { c });
            if (result.Length != 2)
                throw new ArgumentException();

            return result;
        }

        public override char Decode(byte[] bytes)
        {
            var c = Encoding.Unicode.GetChars(bytes);
            if (c.Length != 1)
                throw new ArgumentException();

            return c.Single();
        }
    }
}