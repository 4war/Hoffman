using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Encoders
{
    public class AsciiAbstractEncoder : AbstractEncoder
    {
        
        public override string Name => "ASCII";
        public override string SizeString => "1 байт";
        public override int Size => 1;

        public override byte[] Pack(char c)
        { 
            return new []{(byte)c};
        }

        public override char Decode(byte[] bytes)
        {
            if (bytes.Length != 1)
                throw new ArgumentException();
            
            return (char)bytes.Single();
        }
    }
}