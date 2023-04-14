using System;

namespace Model
{
    /// <summary>
    /// Отвечает за статистическую информацию
    /// </summary>
    public class ZipStatistics
    {
        public int MessageBytes { get; set; } //Размер сообщения
        public int CompressedBytes { get; set; } //Размер сжатого сообщения
        public int DictionaryBytes { get; set; } //Размер словаря

        //Коэффициент сжатия
        public double CompressedRatio => ((double)DictionaryBytes + CompressedBytes) / MessageBytes;
    }
}