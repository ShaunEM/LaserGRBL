namespace LaserGRBL.Forms
{
    partial class LayerConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerConfigForm));
            this.NextButton = new System.Windows.Forms.Button();
            this.layerName = new System.Windows.Forms.TextBox();
            this.fileName = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tableLayoutPanel17 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.colorDropDown = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel17.SuspendLayout();
            this.SuspendLayout();
            // 
            // NextButton
            // 
            resources.ApplyResources(this.NextButton, "NextButton");
            this.NextButton.Name = "NextButton";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // layerName
            // 
            resources.ApplyResources(this.layerName, "layerName");
            this.layerName.Name = "layerName";
            // 
            // fileName
            // 
            resources.ApplyResources(this.fileName, "fileName");
            this.fileName.Name = "fileName";
            // 
            // tableLayoutPanel17
            // 
            resources.ApplyResources(this.tableLayoutPanel17, "tableLayoutPanel17");
            this.tableLayoutPanel17.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel17.Controls.Add(this.fileName, 1, 1);
            this.tableLayoutPanel17.Controls.Add(this.layerName, 1, 3);
            this.tableLayoutPanel17.Controls.Add(this.colorDropDown, 1, 4);
            this.tableLayoutPanel17.Controls.Add(this.label33, 0, 3);
            this.tableLayoutPanel17.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel17.Controls.Add(this.NextButton, 2, 5);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // colorDropDown
            // 
            resources.ApplyResources(this.colorDropDown, "colorDropDown");
            this.colorDropDown.FormattingEnabled = true;
            this.colorDropDown.Name = "colorDropDown";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // LayerConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel17);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LayerConfigForm";
            this.Load += new System.EventHandler(this.LayerConfigForm_Load);
            this.tableLayoutPanel17.ResumeLayout(false);
            this.tableLayoutPanel17.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.TextBox layerName;
        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel17;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.ComboBox colorDropDown;
    }
}