﻿/*
 * Created by SharpDevelop.
 * User: Diego
 * Date: 05/12/2016
 * Time: 23:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace LaserGRBL
{
    partial class ProjectDetailForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolTip TT;

        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectDetailForm));
            this.TT = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.btnLayerSetting = new System.Windows.Forms.Button();
            this.btnMoveLayerUp = new System.Windows.Forms.Button();
            this.btnMoveLayerDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.layerListBox = new System.Windows.Forms.ListBoxRowColor();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.btnResetAll, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnAddLayer, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnRemoveLayer, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnLayerSetting, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.layerListBox, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.btnMoveLayerUp, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnMoveLayerDown, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // btnResetAll
            // 
            resources.ApplyResources(this.btnResetAll, "btnResetAll");
            this.btnResetAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnAddLayer
            // 
            resources.ApplyResources(this.btnAddLayer, "btnAddLayer");
            this.btnAddLayer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // btnRemoveLayer
            // 
            resources.ApplyResources(this.btnRemoveLayer, "btnRemoveLayer");
            this.btnRemoveLayer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // btnLayerSetting
            // 
            resources.ApplyResources(this.btnLayerSetting, "btnLayerSetting");
            this.btnLayerSetting.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLayerSetting.Name = "btnLayerSetting";
            this.btnLayerSetting.UseVisualStyleBackColor = true;
            this.btnLayerSetting.Click += new System.EventHandler(this.btnLayerSetting_Click);
            // 
            // btnMoveLayerUp
            // 
            resources.ApplyResources(this.btnMoveLayerUp, "btnMoveLayerUp");
            this.btnMoveLayerUp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnMoveLayerUp.Name = "btnMoveLayerUp";
            this.btnMoveLayerUp.UseVisualStyleBackColor = true;
            this.btnMoveLayerUp.Click += new System.EventHandler(this.btnMoveLayerUp_Click);
            // 
            // btnMoveLayerDown
            // 
            resources.ApplyResources(this.btnMoveLayerDown, "btnMoveLayerDown");
            this.btnMoveLayerDown.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnMoveLayerDown.Name = "btnMoveLayerDown";
            this.btnMoveLayerDown.UseVisualStyleBackColor = true;
            this.btnMoveLayerDown.Click += new System.EventHandler(this.btnMoveLayerDown_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel4.SetColumnSpan(this.label1, 5);
            this.label1.Name = "label1";
            // 
            // layerListBox
            // 
            this.layerListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel4.SetColumnSpan(this.layerListBox, 5);
            resources.ApplyResources(this.layerListBox, "layerListBox");
            this.layerListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.layerListBox.FormattingEnabled = true;
            this.layerListBox.Name = "layerListBox";
            this.layerListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ProjectDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ProjectDetailForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddLayer;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.Button btnLayerSetting;
        private System.Windows.Forms.ListBoxRowColor layerListBox;
        private System.Windows.Forms.Button btnMoveLayerUp;
        private System.Windows.Forms.Button btnMoveLayerDown;
    }
}