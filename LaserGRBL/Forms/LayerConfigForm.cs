using System;
using System.Drawing;
using System.Windows.Forms;

namespace LaserGRBL.Forms
{
    public partial class LayerConfigForm : Form
    {
        private Layer Layer { get; set; }
        private readonly int LayerNumber = 0;
        private readonly string FileName = "";

        public LayerConfigForm(int layer, string fileName)
        {
            InitializeComponent();
            LayerNumber = layer;
            FileName = fileName;
        }

        private void LayerConfigForm_Load(object sender, EventArgs e)
        {
            layerName.Text = $"Layer_{LayerNumber.ToString().PadLeft(2, '0')}";
            fileName.Text = FileName;

            if (string.IsNullOrEmpty(fileName.Text))
            {
                this.Close();
            }
            else
            {
                string[] layerColors = new string[] {
                    "Red",
                    "Blue",
                    "Green",
                    "Purple",
                    "Black",
                    "Pink",
                    "Grey"
                };
                foreach (string layerColor in layerColors)
                {
                    colorDropDown.Items.Add(layerColor);
                }
                colorDropDown.SelectedIndex = LayerNumber < layerColors.Length ? LayerNumber : 0;
            }
        }

        private static string GetFileFromUser(Form parent)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                //pre-select last file if exist
                string lastFN = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
                if (lastFN != null && System.IO.File.Exists(lastFN))
                {
                    ofd.FileName = lastFN;
                }
                ofd.Filter = "Any supported file|*.nc;*.cnc;*.tap;*.gcode;*.ngc;*.bmp;*.png;*.jpg;*.gif;*.svg;*.lps|GCODE Files|*.nc;*.cnc;*.tap;*.gcode;*.ngc|Raster Image|*.bmp;*.png;*.jpg;*.gif|Vector Image (experimental)|*.svg|LaserGRBL Project|*.lps";
                ofd.CheckFileExists = true;
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;

                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.DialogResult.Cancel;
                try
                {
                    dialogResult = ofd.ShowDialog(parent);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    ofd.AutoUpgradeEnabled = false;
                    dialogResult = ofd.ShowDialog(parent);
                }
                return (dialogResult == System.Windows.Forms.DialogResult.OK) ? ofd.FileName : null;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Layer = new Layer()
            {
                LayerDescription = layerName.Text,
                //FileName = fileName.Text,
                PreviewColor = Color.FromName(colorDropDown.Text)
            };
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        internal static Layer CreateAndShowDialog(Form parent, int layer = 0)
        {
            string fileName = GetFileFromUser(parent);
            if (string.IsNullOrEmpty(fileName))
                return null;

            using (LayerConfigForm sf = new LayerConfigForm(layer, fileName))
            {
                var result = sf.ShowDialog(parent);
                if (result == DialogResult.OK)
                {
                    return sf.Layer;
                }
            }
            return null;
        }
    }
}
