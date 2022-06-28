using System.Drawing;

namespace System.Windows.Forms
{

    public class ComboBoxRowColor : ComboBox
    {
        public ComboBoxRowColor()
        {
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if(this.Items.Count > 0 && e.Index > 0)
            {           
                base.OnDrawItem(e);
                e.DrawBackground();
                ComboBoxItem item = (ComboBoxItem)this.Items[e.Index];
                e.Graphics.DrawString(item.Text, this.Font, new SolidBrush(this.ForeColor), e.Bounds.X + 18, e.Bounds.Y);
                if (item.PreviewColor != null)
                {
                    Rectangle rec = e.Bounds;
                    rec.X += rec.Left;
                    rec.Width = 15;
                    e.Graphics.FillRectangle(new SolidBrush((Color)item.PreviewColor), rec);
                }
            }
        }
    }
    public class ComboBoxItem
    {
        public ComboBoxItem() { }

        public ComboBoxItem(string text, object value, Color? color = null)
        {
            this.text = text; 
            val = value;
            previewColor = color;
        }

        string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        object val;
        public object Value
        {
            get { return val; }
            set { val = value; }
        }

        Color? previewColor = Color.Black;
        public Color? PreviewColor
        {
            get { return previewColor; }
            set { previewColor = value; }
        }

        public override string ToString()
        {
            return text;
        }
    }

}
