using System.Drawing;
using System.Windows.Forms;

namespace Zip
{
    public partial class CustomTextBox : Control
    {
        public static readonly Font StaticFont = new Font(FontFamily.GenericMonospace, 12);
        public static readonly int StaticWidth = 700;
        public static readonly Color ReadOnlyColor = Color.FromArgb(208, 208, 208);

        public readonly Label Label;
        public readonly RichTextBox RichTextBox;
        public CustomTextBox(string label, int height, bool isEditable)
        {
            Size = new Size() { Height = height + 30, Width = StaticWidth };
            Label = new Label()
            {
                Location = new Point(0, 0),
                Font = StaticFont,
                Text = label,
                Size = new Size() { Height = 30, Width = StaticWidth }
            };
            Controls.Add(Label);
            RichTextBox = new RichTextBox()
            {
                Location = new Point(0, 30),
                Font = StaticFont,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size() { Height = height, Width = StaticWidth },
                ReadOnly = !isEditable
            };
            if (!isEditable)
            {
                RichTextBox.BackColor = ReadOnlyColor;
            }
            Controls.Add(RichTextBox);
        }
    }
}