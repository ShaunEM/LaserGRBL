//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBL.Extentions;
using LaserGRBL.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.ListBox;

namespace LaserGRBL
{
	/// <summary>
	/// Description of ConnectLogForm.
	/// </summary>
	public partial class ProjectDetailForm : System.Windows.Forms.UserControl
	{
		GrblCore Core;
        bool UpdateGUIBusy = false;
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
            UpdateGUIBusy = true;
            layerListBox.SuspendLayout();

            //List<int> selectedIndices = new List<int>();
            layerListBox.Items.Clear();
            //var imageList1 = new ImageList(this.components);
            //imageList1.Images.Add(Image.FromFile("C:\\DevHome\\LaserGRBL\\LaserGRBL\\Grafica\\add2.png"));
            //layerListview.StateImageList = imageList1;

            for (int idx = 0; idx < Core.ProjectCore.layers.Count; idx++)
			{
                ListBoxItem newItem = new ListBoxItem(Core.ProjectCore.layers[idx].LayerDescription, (int)idx,
                    Core.ProjectCore.layers[idx].Config.PreviewColor);
                int itemIdx = layerListBox.Items.Add(newItem);

               // layerListBox.SetSelected(itemIdx, Core.ProjectCore.layers[idx].Selected);
                if (Core.ProjectCore.layers[idx].Selected)
                {
                    layerListBox.SelectedIndex = idx;
                }
                //if (idx >= layerListBox.Items.Count)
                //{

                //}
                //else
                //{
                //    layerListBox.Items[idx] = newItem;
                //}

            }

            //layerListBox.Items.AddRange(items);
            //for (int idx = Core.ProjectCore.layers.Count; idx < layerListBox.Items.Count; idx++)
            //{
            //    layerListBox.Items.RemoveAt(idx);
            //}

               layerListBox.ResumeLayout();
            UpdateGUIBusy = false;
        }


		private void btnLayerSetting_Click(object sender, EventArgs e)
        {
            int idx = (int)((ListBoxItem)(layerListBox?.SelectedItem ?? -1))?.Value;
            if (idx >= 0)
            {
                Core.EditLayerSetting(this.ParentForm, idx);
                UpdateGUI();
            }

            //string layerName = lstLayers.SelectedItem?.ToString() ?? "";
            //if (!string.IsNullOrEmpty(layerName))
            //         {
            //	Core.EditLayerSetting(this.ParentForm, Core.ProjectCore.GetLayerIndex(layerName));
            //}
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            if(layerListBox?.SelectedItem != null)
            {
                int idx = (int)((ListBoxItem)(layerListBox?.SelectedItem ?? -1))?.Value;
                if (idx >= 0)
                {
                    Core.RemoveLayer(idx);
                    UpdateGUI();
                    //layerListBox.SelectedIndex = idx - 1; //force refresh
                }
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
            UpdateGUI();

        }


        private void btnResetAll_Click(object sender, EventArgs e)
        {
            Core.ResetAll();
            UpdateGUI();
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
            if(!UpdateGUIBusy)
            {
                int idxA = layerListBox.SelectedIndex;
                if (idxA >= 0)
                {
                    Core.ProjectCore.UnSelectAllLayers();
                    Core.ProjectCore.layers[idxA].Selected = true;
                    Core.LayerUpdated();
                    UpdateGUI();
                }
            }
        }

        private void btnMoveLayerUp_Click(object sender, EventArgs e)
        {
            int idxA = layerListBox.SelectedIndex;
            if(idxA > 0)
            {
                Core.ProjectCore.layers.Swap(idxA, idxA - 1);
                Core.LayerUpdated();
                //layerListBox.SelectedIndex = idxA - 1;//force refresh
                UpdateGUI();
            }
        }

        private void btnMoveLayerDown_Click(object sender, EventArgs e)
        {
            int idxA = layerListBox.SelectedIndex;
            if (idxA >= 0 && idxA < layerListBox.Items.Count-1)
            {
                Core.ProjectCore.layers.Swap(idxA, idxA + 1);
                Core.LayerUpdated();
                //layerListBox.Swap(idxA, idxA + 1);
                //layerListBox.SelectedIndex = idxA + 1;//force refresh
                UpdateGUI();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Core.SaveProject(this.ParentForm);
        }
    }
}
