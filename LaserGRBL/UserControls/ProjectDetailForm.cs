//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBL.Extentions;
using LaserGRBL.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LaserGRBL
{
	/// <summary>
	/// Description of ConnectLogForm.
	/// </summary>
	public partial class ProjectDetailForm : System.Windows.Forms.UserControl
	{
		GrblCore Core;
		public ProjectDetailForm()
		{
			InitializeComponent();
		}
		public void SetCore(GrblCore core)
		{
			Core = core;
		}


		public void UpdateGUI()
        {
			layerListBox.Items.Clear();

			var imageList1 = new ImageList(this.components);

			//imageList1.Images.Add(Image.FromFile("C:\\DevHome\\LaserGRBL\\LaserGRBL\\Grafica\\add2.png"));
			
			//layerListview.StateImageList = imageList1;

			for ( int idx = 0; idx < Core.ProjectCore.layers.Count; idx++)
			{
				layerListBox.Items.Add(new ListBoxItem(Core.ProjectCore.layers[idx].LayerDescription, (int)idx, Core.ProjectCore.layers[idx].PreviewColor));
                //layerListview.Items.Add(new ListViewItem(new string[] { "", Core.ProjectCore.layers[idx].LayerDescription, "3" }));
                //this is very Important


                //layerListview.Items[idx].UseItemStyleForSubItems = false;



    //            var listViewItem1 = new ListBoxItem("","", -1);
				//listViewItem1.StateImageIndex = 0;
				//layerListBox.Items.Add(listViewItem1);



				// Now you can Change the Particular Cell Property of Style
				//layerListBox.Items[idx].SubItems[0].BackColor = Core.ProjectCore.layers[idx].PreviewColor;
			}
        }


		private void btnLayerSetting_Click(object sender, EventArgs e)
        {
            int idx = (int)((ListBoxItem)(layerListBox?.SelectedItem ?? -1))?.Value;
            if (idx >= 0)
            {
                Core.EditLayerSetting(this.ParentForm, idx);
            }

            //string layerName = lstLayers.SelectedItem?.ToString() ?? "";
            //if (!string.IsNullOrEmpty(layerName))
            //         {
            //	Core.EditLayerSetting(this.ParentForm, Core.ProjectCore.GetLayerIndex(layerName));
            //}
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            int idx = (int)((ListBoxItem)(layerListBox?.SelectedItem ?? -1))?.Value;
            if (idx >= 0)
            {
                Core.RemoveLayer(idx);
            }
            //string layerName = lstLayers.SelectedItem?.ToString() ?? "";
            //if (!string.IsNullOrEmpty(layerName))
            //{
            //	Core.RemoveLayer(Core.ProjectCore.GetLayerIndex(layerName));
            //}
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
			Core.AddLayer(this.ParentForm);
		}


        private void btnResetAll_Click(object sender, EventArgs e)
        {
			Core.ResetAll();
		}

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void layerListview_Click(object sender, EventArgs e)
        {

			//Point mousePosition = layerListBox.PointToClient(Control.MousePosition);
			//ListViewHitTestInfo hit = layerListBox.HitTest(mousePosition);
			//int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
			//MessageBox.Show("Double Clicked on :" + columnindex);
			//if (layerListview.SelectedItems.Count >= 1)
			//{
			//	ListViewItem item = layerListview.SelectedItems[0];

			//	MouseEventArgs e2 = (MouseEventArgs)e;
			//	//here i check for the Mouse pointer location on click if its contained 
			//	// in the actual selected item's bounds or not .
			//	// cuz i ran into a problem with the ui once because of that ..
			//	if (item.Bounds.Contains(e2.Location))
			//	{
			//		MessageBox.Show("Double Clicked on :" + item.Text);
			//	}
			//}
		}

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnMoveLayerUp_Click(object sender, EventArgs e)
        {
            int idxA = layerListBox.SelectedIndex;
            if(idxA > 0)
            {
                Core.ProjectCore.layers.Swap(idxA, idxA - 1);
                UpdateGUI();
                layerListBox.SelectedIndex = idxA - 1;
            }
        }

        private void btnMoveLayerDown_Click(object sender, EventArgs e)
        {
            int idxA = layerListBox.SelectedIndex;
            if (idxA < layerListBox.Items.Count-1)
            {
                Core.ProjectCore.layers.Swap(idxA, idxA + 1);
                UpdateGUI();
                layerListBox.SelectedIndex = idxA + 1;
            }
        }
    }
}
