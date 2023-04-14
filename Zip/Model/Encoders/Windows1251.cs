using System;
using System.Linq;
using System.Text;

namespace Model.Encoders
{
    public class Windows1251 : AbstractEncoder
    {
        public override string Name => "Windows-1251";
        public override string SizeString => "1 байт";
        public override int Size => 1;
        
        private readonly Encoding _encoding;
        public Windows1251()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encoding = Encoding.GetEncoding("windows-1251");
        }

        public override byte[] Pack(char c)
        {
            var result = _encoding.GetBytes(new[] { c });
            if (result.Length != 1)
                throw new ArgumentException();

            return result;
        }

        public override char Decode(byte[] bytes)
        {
            var c = _encoding.GetChars(bytes);
            if (c.Length != 1)
                throw new ArgumentException();

            return c.Single();
        }
    }
}