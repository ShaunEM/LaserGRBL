using System.Drawing;

namespace System.Windows.Forms
{
    public class ListBoxRowColor : ListBox
    {
        public ListBoxRowColor()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }
        protected override void OnClick(EventArgs e)
        {
            //Graphics g = this.CreateGraphics();
            MouseEventArgs e2 = (MouseEventArgs)e;
            if(e2.X > this.Left + 15 && e2.X < this.Left + 30)
            {
                // TODO: replace buttons with images
            }
        }





        protected override void OnDrawItem(DrawItemEventArgs e)
        {

            if (this.Items.Count > 0 && e.Index >= 0)
            {
                base.OnDrawItem(e);
                e.DrawBackground();
                if (this.Items[e?.Index ?? 0] is ListBoxItem item)
                {
                    e.Graphics.DrawString(item.Text, this.Font, new SolidBrush(this.ForeColor), e.Bounds.X + 15, e.Bounds.Y);
                    if (item.PreviewColor != null)
                    {
                        Rectangle rec = e.Bounds;
                        rec.X = 0; //+= rec.Width - 15;
                        rec.Width = 15;
                        e.Graphics.FillRectangle(new SolidBrush((Color)item.PreviewColor), rec);

                        // TODO: replace buttons with images
                        //Image img = Image.FromFile("C:\\DevHome\\LaserGRBL\\LaserGRBL\\Grafica\\add2.png");
                        //e.Graphics.DrawImage(img, rec.X + 15, rec.Y, 15, 15);
                    }
                }
            }
        }
    }
    public class ListBoxItem
    {
        public ListBoxItem() { }

        public ListBoxItem(string text, object value, Color? color = null)
        {
            this.text = text;
            this.value = value;
            previewColor = color;
        }

        string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        object value = null;
        public object Value
        {
            get { return value; }
            set { value = Value; }
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
