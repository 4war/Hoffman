using System;

namespace Model
{
    public class ZipStatistics
    {
        public int MessageBytes { get; set; }
        public int CompressedBytes { get; set; }
        public int DictionaryBytes { get; set; }

        public double CompressedRatio => ((double)DictionaryBytes + CompressedBytes) / MessageBytes;

        public string Compressed => $"{Math.Round(CompressedRatio * 100, 2)}%";
    }
}