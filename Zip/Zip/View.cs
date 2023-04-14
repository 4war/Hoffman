using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Model;
using Model.Encoders;

namespace Zip
{
    public partial class View : Form
    {
        private readonly Encoder _encoder = new Encoder();
        private readonly Decoder _decoder = new Decoder();

        private readonly List<AbstractEncoder> _encoders = new List<AbstractEncoder>()
        {
            new AsciiAbstractEncoder(),
            new Windows1251(),
            //new Utf8Encoder(),
            new UnicodeAbstractEncoder(),
        };

        private readonly List<string> _labels;
        private readonly Label _encodingsLabel = new Label()
        {
            Text = "Кодировка:",
            Location = new Point() { X = 50, Y = 10, },
            Font = CustomTextBox.StaticFont,
            AutoSize = true
        };

        private readonly ComboBox _encodingsComboBox = new ComboBox()
        {
            Location = new Point() { X = 200, Y = 10, },
            Items = {  },
            Width = 300,
            FlatStyle = FlatStyle.Flat,
            Font = CustomTextBox.StaticFont,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        private readonly CustomTextBox _inputMessageTextBox = new CustomTextBox("Исходное сообщение", 200, true)
        {
            Location = new Point() { X = 50, Y = 60 },
            
        };

        private readonly CustomTextBox _dictionaryTextBox =
            new CustomTextBox("Словарь: Длина кода | Код при сжатии | Реальный код символа", 100, false)
            {
                Location = new Point() { X = 50, Y = 310 },
            };

        private readonly CustomTextBox _zippedTextBox = new CustomTextBox("Сжатое сообщение", 100, false)
        {
            Location = new Point() { X = 50, Y = 450 },
        };

        private readonly CustomTextBox _decodedMessageTextBox =
            new CustomTextBox("Декодированное сообщение", 260, false)
            {
                Location = new Point() { X = 50, Y = 600 },
            };

        private readonly Label _frequencyLabel = new Label()
        {
            Location = new Point() { X = CustomTextBox.StaticWidth + 80, Y = 60 },
            Font = CustomTextBox.StaticFont,
            Text = "Частоты символов",
            AutoSize = true,
        };

        private readonly RichTextBox _frequencyInfo = new RichTextBox()
        {
            Location = new Point() { X = CustomTextBox.StaticWidth + 80, Y = 90 },
            Font = CustomTextBox.StaticFont,
            Size = new Size() { Height = 600, Width = 450 },
            ReadOnly = true,
            BackColor = CustomTextBox.ReadOnlyColor,
        };

        private readonly Label _statisticsLabel = new Label()
        {
            Location = new Point() { X = CustomTextBox.StaticWidth + 80, Y = 700 },
            Font = CustomTextBox.StaticFont,
            Text = "Статистика",
            AutoSize = true,
        };

        private readonly RichTextBox _statisticsInfo = new RichTextBox()
        {
            Location = new Point() { X = CustomTextBox.StaticWidth + 80, Y = 730 },
            Font = CustomTextBox.StaticFont,
            Size = new Size() { Height = 160, Width = 450 },
            ReadOnly = true,
            BackColor = CustomTextBox.ReadOnlyColor,
        };

        public View()
        {
            InitializeComponent();
            _labels = _encoders.Select(x => string.Join(" | ", Enumerable.Repeat(x.Name, x.Size))).ToList();
            foreach (var item in _encoders.Select(x => $"{x.Name} ({x.SizeString})"))
                _encodingsComboBox.Items.Add(item);
            _encodingsComboBox.SelectedIndex = 0;
            Text = "Модель сжатия сообщения";
            BackColor = Color.FromArgb(171, 171, 171);
            Size = new Size() { Width = 1280, Height = 960 };
            _inputMessageTextBox.RichTextBox.TextChanged += (_, _) => Launch();
            _encodingsComboBox.SelectedIndexChanged += (_, _) => Launch();
            Controls.Add(_encodingsLabel);
            Controls.Add(_encodingsComboBox);
            Controls.Add(_inputMessageTextBox);
            Controls.Add(_zippedTextBox);
            Controls.Add(_dictionaryTextBox);
            Controls.Add(_decodedMessageTextBox);
            Controls.Add(_frequencyLabel);
            Controls.Add(_frequencyInfo);
            Controls.Add(_statisticsLabel);
            Controls.Add(_statisticsInfo);

            Launch();
        }

        private void Launch()
        {
            var dictionary = UpdateDictionary();
            UpdateCustomTextBoxes(dictionary);
        }

        private Dictionary<char, Figure> UpdateDictionary()
        {
            var message = _inputMessageTextBox.RichTextBox.Text;

            if (message.Length == 0)
            {
                _frequencyInfo.Text = string.Empty;
                return new Dictionary<char, Figure>();
            }

            var frequencies = _encoder.GetCountDictionary(message);
            var tree = _encoder.GetEncodingTree(frequencies);
            var dictionary = _encoder.GetCodes(tree);

            var list = new List<string>();
            foreach (var kvp in frequencies.AsEnumerable().OrderByDescending(x => x.Value))
            {
                var code = Convert.ToString(dictionary[kvp.Key].Value, 2).PadLeft(dictionary[kvp.Key].Size, '0');
                list.Add($"{kvp.Key}: {code}: ({Math.Round(kvp.Value / (double)message.Length * 100, 2)}%)");
            }

            _frequencyInfo.Text = string.Join("\n", list);
            return dictionary;
        }

        private void UpdateCustomTextBoxes(Dictionary<char, Figure> dictionary)
        {
            var packer = new Packer(dictionary);
            AbstractEncoder abstractEncoder = _encoders[_encodingsComboBox.SelectedIndex]; 

            var packedDictionary = packer.PackDictionary(abstractEncoder).ToList();
            var packedMessage = packer.PackMessage(_inputMessageTextBox.RichTextBox.Text).ToList();
            _dictionaryTextBox.RichTextBox.Text =
                string.Join("|", packedDictionary.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
            _zippedTextBox.RichTextBox.Text = string.Join("|", packedMessage.Select(x => Convert.ToString(x, 2)));

            _dictionaryTextBox.Label.Text =
                $"Словарь: Длина кода | Код при сжатии | {_labels[_encodingsComboBox.SelectedIndex]}";
            var decodedMessage = _decoder.Execute(packedMessage, packedDictionary, abstractEncoder).ToList();
            _decodedMessageTextBox.RichTextBox.Text = string.Join("", decodedMessage);

            var statistics = new ZipStatistics()
            {
                MessageBytes = decodedMessage.Count,
                DictionaryBytes = packedDictionary.Count,
                CompressedBytes = packedMessage.Count
            };

            var statisticsList = new List<string>();
            statisticsList.Add($"Размер сообщения: {statistics.MessageBytes} байт");
            statisticsList.Add($"Размер словаря: {statistics.DictionaryBytes} байт");
            statisticsList.Add($"Размер сжатого сообщения: {statistics.CompressedBytes} байт");
            statisticsList.Add($"Коэффициент сжатия: {Math.Round(statistics.CompressedRatio, 2)}");
            _statisticsInfo.Text = string.Join("\n", statisticsList);
        }
    }
}