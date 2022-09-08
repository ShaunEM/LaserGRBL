namespace LaserGRBL.RasterConverter
{
	partial class RasterToLaserForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RasterToLaserForm));
            this.TlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.TCOriginalPreview = new System.Windows.Forms.TabControl();
            this.TpPreview = new System.Windows.Forms.TabPage();
            this.WB = new LaserGRBL.UserControls.WaitingProgressBar();
            this.PbConverted = new System.Windows.Forms.PictureBox();
            this.TpOriginal = new System.Windows.Forms.TabPage();
            this.PbOriginal = new System.Windows.Forms.PictureBox();
            this.FlipControl = new System.Windows.Forms.TableLayoutPanel();
            this.BtFlipV = new LaserGRBL.UserControls.ImageButton();
            this.BtFlipH = new LaserGRBL.UserControls.ImageButton();
            this.BtRotateCW = new LaserGRBL.UserControls.ImageButton();
            this.BtRotateCCW = new LaserGRBL.UserControls.ImageButton();
            this.BtnRevert = new LaserGRBL.UserControls.ImageButton();
            this.BtnCrop = new LaserGRBL.UserControls.ImageButton();
            this.BtnReverse = new LaserGRBL.UserControls.ImageButton();
            this.BtnAutoTrim = new LaserGRBL.UserControls.ImageButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnCreate = new System.Windows.Forms.Button();
            this.TlpLeft = new System.Windows.Forms.TableLayoutPanel();
            this.GbSize = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.IIOffsetX = new LaserGRBL.UserControls.NumericInput.DecimalInputRanged();
            this.IIOffsetY = new LaserGRBL.UserControls.NumericInput.DecimalInputRanged();
            this.IISizeH = new LaserGRBL.UserControls.NumericInput.DecimalInputRanged();
            this.IISizeW = new LaserGRBL.UserControls.NumericInput.DecimalInputRanged();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.CbAutosize = new System.Windows.Forms.CheckBox();
            this.IIDpi = new LaserGRBL.UserControls.NumericInput.IntegerInputRanged();
            this.BtnDPI = new LaserGRBL.UserControls.ImageButton();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnReset = new LaserGRBL.UserControls.ImageButton();
            this.BtnCenter = new LaserGRBL.UserControls.ImageButton();
            this.BtnUnlockProportion = new LaserGRBL.UserControls.ImageButton();
            this.GbLaser = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnModulationInfo = new LaserGRBL.UserControls.ImageButton();
            this.IIMinPower = new LaserGRBL.UserControls.NumericInput.IntegerInputRanged();
            this.CBLaserON = new System.Windows.Forms.ComboBox();
            this.LblSmax = new System.Windows.Forms.Label();
            this.IIMaxPower = new LaserGRBL.UserControls.NumericInput.IntegerInputRanged();
            this.LblMinPerc = new System.Windows.Forms.Label();
            this.LblMaxPerc = new System.Windows.Forms.Label();
            this.BtnOnOffInfo = new LaserGRBL.UserControls.ImageButton();
            this.LblBorderTracing = new System.Windows.Forms.Label();
            this.LblLinearFilling = new System.Windows.Forms.Label();
            this.IIBorderTracing = new LaserGRBL.UserControls.NumericInput.IntegerInputRanged();
            this.IILinearFilling = new LaserGRBL.UserControls.NumericInput.IntegerInputRanged();
            this.LblBorderTracingmm = new System.Windows.Forms.Label();
            this.LblLinearFillingmm = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.IILoopCounter = new System.Windows.Forms.NumericUpDown();
            this.LblLaserMode = new System.Windows.Forms.Label();
            this.LblSmin = new System.Windows.Forms.Label();
            this.GbParameters = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TBRed = new LaserGRBL.UserControls.ColorSlider();
            this.CbResize = new LaserGRBL.UserControls.EnumComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.LblGrayscale = new System.Windows.Forms.Label();
            this.CbMode = new LaserGRBL.UserControls.EnumComboBox();
            this.LblRed = new System.Windows.Forms.Label();
            this.LblGreen = new System.Windows.Forms.Label();
            this.LblBlue = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TbBright = new LaserGRBL.UserControls.ColorSlider();
            this.TbContrast = new LaserGRBL.UserControls.ColorSlider();
            this.label3 = new System.Windows.Forms.Label();
            this.CbThreshold = new System.Windows.Forms.CheckBox();
            this.TbThreshold = new LaserGRBL.UserControls.ColorSlider();
            this.TBWhiteClip = new LaserGRBL.UserControls.ColorSlider();
            this.label4 = new System.Windows.Forms.Label();
            this.TBBlue = new LaserGRBL.UserControls.ColorSlider();
            this.TBGreen = new LaserGRBL.UserControls.ColorSlider();
            this.GbConversionTool = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLineTrace = new System.Windows.Forms.TabPage();
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.CbDither = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.CbDirections = new LaserGRBL.UserControls.EnumComboBox();
            this.UDQuality = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BtnQualityInfo = new LaserGRBL.UserControls.ImageButton();
            this.CbLinePreview = new System.Windows.Forms.CheckBox();
            this.tabVectorize = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnAdaptiveQualityInfo = new LaserGRBL.UserControls.ImageButton();
            this.CbAdaptiveQuality = new System.Windows.Forms.CheckBox();
            this.LAdaptiveQuality = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.UDSpotRemoval = new System.Windows.Forms.NumericUpDown();
            this.CbSpotRemoval = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.UDOptimize = new System.Windows.Forms.NumericUpDown();
            this.UDSmoothing = new System.Windows.Forms.NumericUpDown();
            this.CbOptimize = new System.Windows.Forms.CheckBox();
            this.CbSmoothing = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.CbFillingDirection = new LaserGRBL.UserControls.EnumComboBox();
            this.LblFillingQuality = new System.Windows.Forms.Label();
            this.UDFillingQuality = new System.Windows.Forms.NumericUpDown();
            this.LblFillingLineLbl = new System.Windows.Forms.Label();
            this.UDDownSample = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.CbDownSample = new System.Windows.Forms.CheckBox();
            this.lOptimizeFast = new System.Windows.Forms.Label();
            this.BtnFillingQualityInfo = new LaserGRBL.UserControls.ImageButton();
            this.CbOptimizeFast = new System.Windows.Forms.CheckBox();
            this.tabCenterline = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TBLineThreshold = new LaserGRBL.UserControls.ColorSlider();
            this.TBCornerThreshold = new LaserGRBL.UserControls.ColorSlider();
            this.CbLineThreshold = new System.Windows.Forms.CheckBox();
            this.CbCornerThreshold = new System.Windows.Forms.CheckBox();
            this.WT = new System.Windows.Forms.Timer(this.components);
            this.TT = new System.Windows.Forms.ToolTip(this.components);
            this.TlpMain.SuspendLayout();
            this.TCOriginalPreview.SuspendLayout();
            this.TpPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbConverted)).BeginInit();
            this.TpOriginal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbOriginal)).BeginInit();
            this.FlipControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.TlpLeft.SuspendLayout();
            this.GbSize.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.GbLaser.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IILoopCounter)).BeginInit();
            this.GbParameters.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.GbConversionTool.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLineTrace.SuspendLayout();
            this.TLP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDQuality)).BeginInit();
            this.tabVectorize.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDSpotRemoval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDOptimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDSmoothing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDFillingQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDDownSample)).BeginInit();
            this.tabCenterline.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpMain
            // 
            resources.ApplyResources(this.TlpMain, "TlpMain");
            this.TlpMain.BackColor = System.Drawing.SystemColors.Control;
            this.TlpMain.Controls.Add(this.TCOriginalPreview, 1, 0);
            this.TlpMain.Controls.Add(this.FlipControl, 1, 1);
            this.TlpMain.Controls.Add(this.tableLayoutPanel1, 3, 1);
            this.TlpMain.Controls.Add(this.TlpLeft, 0, 0);
            this.TlpMain.Name = "TlpMain";
            // 
            // TCOriginalPreview
            // 
            resources.ApplyResources(this.TCOriginalPreview, "TCOriginalPreview");
            this.TlpMain.SetColumnSpan(this.TCOriginalPreview, 3);
            this.TCOriginalPreview.Controls.Add(this.TpPreview);
            this.TCOriginalPreview.Controls.Add(this.TpOriginal);
            this.TCOriginalPreview.Name = "TCOriginalPreview";
            this.TCOriginalPreview.SelectedIndex = 0;
            // 
            // TpPreview
            // 
            this.TpPreview.Controls.Add(this.WB);
            this.TpPreview.Controls.Add(this.PbConverted);
            resources.ApplyResources(this.TpPreview, "TpPreview");
            this.TpPreview.Name = "TpPreview";
            this.TpPreview.UseVisualStyleBackColor = true;
            // 
            // WB
            // 
            this.WB.BarColor = System.Drawing.Color.SteelBlue;
            this.WB.BorderColor = System.Drawing.Color.Black;
            this.WB.BouncingMode = LaserGRBL.UserControls.WaitingProgressBar.BouncingModeEnum.PingPong;
            this.WB.DrawProgressString = false;
            this.WB.FillColor = System.Drawing.Color.White;
            this.WB.FillStyle = LaserGRBL.UserControls.FillStyles.Solid;
            this.WB.Interval = 25D;
            resources.ApplyResources(this.WB, "WB");
            this.WB.Maximum = 20D;
            this.WB.Minimum = 0D;
            this.WB.Name = "WB";
            this.WB.ProgressStringDecimals = 0;
            this.WB.Reverse = true;
            this.WB.Running = false;
            this.WB.Step = 1D;
            this.WB.ThrowExceprion = false;
            this.WB.Value = 0D;
            // 
            // PbConverted
            // 
            this.PbConverted.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.PbConverted, "PbConverted");
            this.PbConverted.Name = "PbConverted";
            this.PbConverted.TabStop = false;
            this.PbConverted.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbConvertedMouseDown);
            this.PbConverted.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PbConvertedMouseMove);
            this.PbConverted.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PbConvertedMouseUp);
            this.PbConverted.Resize += new System.EventHandler(this.PbConverted_Resize);
            // 
            // TpOriginal
            // 
            this.TpOriginal.Controls.Add(this.PbOriginal);
            resources.ApplyResources(this.TpOriginal, "TpOriginal");
            this.TpOriginal.Name = "TpOriginal";
            this.TpOriginal.UseVisualStyleBackColor = true;
            // 
            // PbOriginal
            // 
            this.PbOriginal.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.PbOriginal, "PbOriginal");
            this.PbOriginal.Name = "PbOriginal";
            this.PbOriginal.TabStop = false;
            // 
            // FlipControl
            // 
            resources.ApplyResources(this.FlipControl, "FlipControl");
            this.FlipControl.Controls.Add(this.BtFlipV, 5, 0);
            this.FlipControl.Controls.Add(this.BtFlipH, 4, 0);
            this.FlipControl.Controls.Add(this.BtRotateCW, 2, 0);
            this.FlipControl.Controls.Add(this.BtRotateCCW, 3, 0);
            this.FlipControl.Controls.Add(this.BtnRevert, 0, 0);
            this.FlipControl.Controls.Add(this.BtnCrop, 6, 0);
            this.FlipControl.Controls.Add(this.BtnReverse, 8, 0);
            this.FlipControl.Controls.Add(this.BtnAutoTrim, 7, 0);
            this.FlipControl.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.FlipControl.Name = "FlipControl";
            // 
            // BtFlipV
            // 
            this.BtFlipV.AltImage = null;
            this.BtFlipV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtFlipV.Caption = null;
            this.BtFlipV.Coloration = System.Drawing.Color.Empty;
            this.BtFlipV.Image = ((System.Drawing.Image)(resources.GetObject("BtFlipV.Image")));
            resources.ApplyResources(this.BtFlipV, "BtFlipV");
            this.BtFlipV.Name = "BtFlipV";
            this.BtFlipV.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtFlipV, resources.GetString("BtFlipV.ToolTip"));
            this.BtFlipV.UseAltImage = false;
            this.BtFlipV.Click += new System.EventHandler(this.BtFlipVClick);
            // 
            // BtFlipH
            // 
            this.BtFlipH.AltImage = null;
            this.BtFlipH.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtFlipH.Caption = null;
            this.BtFlipH.Coloration = System.Drawing.Color.Empty;
            this.BtFlipH.Image = ((System.Drawing.Image)(resources.GetObject("BtFlipH.Image")));
            resources.ApplyResources(this.BtFlipH, "BtFlipH");
            this.BtFlipH.Name = "BtFlipH";
            this.BtFlipH.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtFlipH, resources.GetString("BtFlipH.ToolTip"));
            this.BtFlipH.UseAltImage = false;
            this.BtFlipH.Click += new System.EventHandler(this.BtFlipHClick);
            // 
            // BtRotateCW
            // 
            this.BtRotateCW.AltImage = null;
            this.BtRotateCW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtRotateCW.Caption = null;
            this.BtRotateCW.Coloration = System.Drawing.Color.Empty;
            this.BtRotateCW.Image = ((System.Drawing.Image)(resources.GetObject("BtRotateCW.Image")));
            resources.ApplyResources(this.BtRotateCW, "BtRotateCW");
            this.BtRotateCW.Name = "BtRotateCW";
            this.BtRotateCW.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtRotateCW, resources.GetString("BtRotateCW.ToolTip"));
            this.BtRotateCW.UseAltImage = false;
            this.BtRotateCW.Click += new System.EventHandler(this.BtRotateCWClick);
            // 
            // BtRotateCCW
            // 
            this.BtRotateCCW.AltImage = null;
            this.BtRotateCCW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtRotateCCW.Caption = null;
            this.BtRotateCCW.Coloration = System.Drawing.Color.Empty;
            this.BtRotateCCW.Image = ((System.Drawing.Image)(resources.GetObject("BtRotateCCW.Image")));
            resources.ApplyResources(this.BtRotateCCW, "BtRotateCCW");
            this.BtRotateCCW.Name = "BtRotateCCW";
            this.BtRotateCCW.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtRotateCCW, resources.GetString("BtRotateCCW.ToolTip"));
            this.BtRotateCCW.UseAltImage = false;
            this.BtRotateCCW.Click += new System.EventHandler(this.BtRotateCCWClick);
            // 
            // BtnRevert
            // 
            this.BtnRevert.AltImage = null;
            this.BtnRevert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnRevert.Caption = null;
            this.BtnRevert.Coloration = System.Drawing.Color.Empty;
            this.BtnRevert.Image = ((System.Drawing.Image)(resources.GetObject("BtnRevert.Image")));
            resources.ApplyResources(this.BtnRevert, "BtnRevert");
            this.BtnRevert.Name = "BtnRevert";
            this.BtnRevert.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnRevert, resources.GetString("BtnRevert.ToolTip"));
            this.BtnRevert.UseAltImage = false;
            this.BtnRevert.Click += new System.EventHandler(this.BtnRevertClick);
            // 
            // BtnCrop
            // 
            this.BtnCrop.AltImage = null;
            this.BtnCrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnCrop.Caption = null;
            this.BtnCrop.Coloration = System.Drawing.Color.Empty;
            this.BtnCrop.Image = ((System.Drawing.Image)(resources.GetObject("BtnCrop.Image")));
            resources.ApplyResources(this.BtnCrop, "BtnCrop");
            this.BtnCrop.Name = "BtnCrop";
            this.BtnCrop.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnCrop, resources.GetString("BtnCrop.ToolTip"));
            this.BtnCrop.UseAltImage = false;
            this.BtnCrop.Click += new System.EventHandler(this.BtnCropClick);
            // 
            // BtnReverse
            // 
            this.BtnReverse.AltImage = null;
            this.BtnReverse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnReverse.Caption = null;
            this.BtnReverse.Coloration = System.Drawing.Color.Empty;
            this.BtnReverse.Image = ((System.Drawing.Image)(resources.GetObject("BtnReverse.Image")));
            resources.ApplyResources(this.BtnReverse, "BtnReverse");
            this.BtnReverse.Name = "BtnReverse";
            this.BtnReverse.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnReverse, resources.GetString("BtnReverse.ToolTip"));
            this.BtnReverse.UseAltImage = false;
            this.BtnReverse.Click += new System.EventHandler(this.BtnReverse_Click);
            // 
            // BtnAutoTrim
            // 
            this.BtnAutoTrim.AltImage = null;
            this.BtnAutoTrim.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnAutoTrim.Caption = null;
            this.BtnAutoTrim.Coloration = System.Drawing.Color.Empty;
            this.BtnAutoTrim.Image = ((System.Drawing.Image)(resources.GetObject("BtnAutoTrim.Image")));
            resources.ApplyResources(this.BtnAutoTrim, "BtnAutoTrim");
            this.BtnAutoTrim.Name = "BtnAutoTrim";
            this.BtnAutoTrim.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnAutoTrim, resources.GetString("BtnAutoTrim.ToolTip"));
            this.BtnAutoTrim.UseAltImage = false;
            this.BtnAutoTrim.Click += new System.EventHandler(this.BtnAutoTrim_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.BtnCancel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnCreate, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // BtnCancel
            // 
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // BtnCreate
            // 
            resources.ApplyResources(this.BtnCreate, "BtnCreate");
            this.BtnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnCreate.Name = "BtnCreate";
            this.BtnCreate.UseVisualStyleBackColor = true;
            this.BtnCreate.Click += new System.EventHandler(this.BtnCreateClick);
            // 
            // TlpLeft
            // 
            resources.ApplyResources(this.TlpLeft, "TlpLeft");
            this.TlpLeft.BackColor = System.Drawing.SystemColors.Control;
            this.TlpLeft.Controls.Add(this.GbSize, 0, 2);
            this.TlpLeft.Controls.Add(this.GbLaser, 0, 3);
            this.TlpLeft.Controls.Add(this.GbParameters, 0, 1);
            this.TlpLeft.Controls.Add(this.GbConversionTool, 0, 0);
            this.TlpLeft.Name = "TlpLeft";
            // 
            // GbSize
            // 
            resources.ApplyResources(this.GbSize, "GbSize");
            this.GbSize.Controls.Add(this.tableLayoutPanel6);
            this.GbSize.Name = "GbSize";
            this.GbSize.TabStop = false;
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.label10, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.label11, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.IIOffsetX, 2, 2);
            this.tableLayoutPanel6.Controls.Add(this.IIOffsetY, 4, 2);
            this.tableLayoutPanel6.Controls.Add(this.IISizeH, 4, 1);
            this.tableLayoutPanel6.Controls.Add(this.IISizeW, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.label12, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.label13, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.label15, 3, 1);
            this.tableLayoutPanel6.Controls.Add(this.label16, 3, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel9, 5, 2);
            this.tableLayoutPanel6.Controls.Add(this.BtnUnlockProportion, 5, 1);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // IIOffsetX
            // 
            this.IIOffsetX.CurrentValue = 0F;
            this.IIOffsetX.DecimalPositions = 1;
            resources.ApplyResources(this.IIOffsetX, "IIOffsetX");
            this.IIOffsetX.ForceMinMax = false;
            this.IIOffsetX.MaxValue = 1000F;
            this.IIOffsetX.MinValue = 0F;
            this.IIOffsetX.Name = "IIOffsetX";
            this.IIOffsetX.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.IIOffsetX.CurrentValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IIOffsetXYCurrentValueChanged);
            // 
            // IIOffsetY
            // 
            this.IIOffsetY.CurrentValue = 0F;
            this.IIOffsetY.DecimalPositions = 1;
            this.IIOffsetY.ForceMinMax = false;
            resources.ApplyResources(this.IIOffsetY, "IIOffsetY");
            this.IIOffsetY.MaxValue = 1000F;
            this.IIOffsetY.MinValue = 0F;
            this.IIOffsetY.Name = "IIOffsetY";
            this.IIOffsetY.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.IIOffsetY.CurrentValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IIOffsetXYCurrentValueChanged);
            // 
            // IISizeH
            // 
            this.IISizeH.CurrentValue = 0F;
            this.IISizeH.DecimalPositions = 1;
            resources.ApplyResources(this.IISizeH, "IISizeH");
            this.IISizeH.ForceMinMax = false;
            this.IISizeH.MaxValue = 1000F;
            this.IISizeH.MinValue = 10F;
            this.IISizeH.Name = "IISizeH";
            this.IISizeH.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.IISizeH.CurrentValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IISizeH_CurrentValueChanged);
            this.IISizeH.OnTheFlyValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IISizeH_OnTheFlyValueChanged);
            // 
            // IISizeW
            // 
            this.IISizeW.CurrentValue = 0F;
            this.IISizeW.DecimalPositions = 1;
            resources.ApplyResources(this.IISizeW, "IISizeW");
            this.IISizeW.ForceMinMax = false;
            this.IISizeW.MaxValue = 1000F;
            this.IISizeW.MinValue = 10F;
            this.IISizeW.Name = "IISizeW";
            this.IISizeW.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.IISizeW.CurrentValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IISizeW_CurrentValueChanged);
            this.IISizeW.OnTheFlyValueChanged += new LaserGRBL.UserControls.NumericInput.DecimalInputBase.CurrentValueChangedDlg(this.IISizeW_OnTheFlyValueChanged);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // tableLayoutPanel8
            // 
            resources.ApplyResources(this.tableLayoutPanel8, "tableLayoutPanel8");
            this.tableLayoutPanel6.SetColumnSpan(this.tableLayoutPanel8, 7);
            this.tableLayoutPanel8.Controls.Add(this.label17, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.CbAutosize, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.IIDpi, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.BtnDPI, 3, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // CbAutosize
            // 
            resources.ApplyResources(this.CbAutosize, "CbAutosize");
            this.CbAutosize.Name = "CbAutosize";
            this.CbAutosize.UseVisualStyleBackColor = true;
            this.CbAutosize.CheckedChanged += new System.EventHandler(this.CbAutosize_CheckedChanged);
            // 
            // IIDpi
            // 
            resources.ApplyResources(this.IIDpi, "IIDpi");
            this.IIDpi.CurrentValue = 300;
            this.IIDpi.ForcedText = null;
            this.IIDpi.ForceMinMax = false;
            this.IIDpi.MaxValue = 10000;
            this.IIDpi.MinValue = 1;
            this.IIDpi.Name = "IIDpi";
            this.IIDpi.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // BtnDPI
            // 
            this.BtnDPI.AltImage = null;
            resources.ApplyResources(this.BtnDPI, "BtnDPI");
            this.BtnDPI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnDPI.Caption = null;
            this.BtnDPI.Coloration = System.Drawing.Color.Empty;
            this.BtnDPI.Image = ((System.Drawing.Image)(resources.GetObject("BtnDPI.Image")));
            this.BtnDPI.Name = "BtnDPI";
            this.BtnDPI.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnDPI, resources.GetString("BtnDPI.ToolTip"));
            this.BtnDPI.UseAltImage = false;
            this.BtnDPI.Click += new System.EventHandler(this.BtnDPI_Click);
            // 
            // tableLayoutPanel9
            // 
            resources.ApplyResources(this.tableLayoutPanel9, "tableLayoutPanel9");
            this.tableLayoutPanel9.Controls.Add(this.BtnReset, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.BtnCenter, 1, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            // 
            // BtnReset
            // 
            this.BtnReset.AltImage = null;
            this.BtnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnReset.Caption = null;
            this.BtnReset.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnReset, "BtnReset");
            this.BtnReset.Image = ((System.Drawing.Image)(resources.GetObject("BtnReset.Image")));
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.BtnReset.TabStop = false;
            this.BtnReset.UseAltImage = false;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // BtnCenter
            // 
            this.BtnCenter.AltImage = null;
            this.BtnCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnCenter.Caption = null;
            this.BtnCenter.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnCenter, "BtnCenter");
            this.BtnCenter.Image = ((System.Drawing.Image)(resources.GetObject("BtnCenter.Image")));
            this.BtnCenter.Name = "BtnCenter";
            this.BtnCenter.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnCenter, resources.GetString("BtnCenter.ToolTip"));
            this.BtnCenter.UseAltImage = false;
            this.BtnCenter.Click += new System.EventHandler(this.BtnCenter_Click);
            // 
            // BtnUnlockProportion
            // 
            this.BtnUnlockProportion.AltImage = ((System.Drawing.Image)(resources.GetObject("BtnUnlockProportion.AltImage")));
            this.BtnUnlockProportion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnUnlockProportion.Caption = null;
            this.BtnUnlockProportion.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnUnlockProportion, "BtnUnlockProportion");
            this.BtnUnlockProportion.Image = ((System.Drawing.Image)(resources.GetObject("BtnUnlockProportion.Image")));
            this.BtnUnlockProportion.Name = "BtnUnlockProportion";
            this.BtnUnlockProportion.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnUnlockProportion, resources.GetString("BtnUnlockProportion.ToolTip"));
            this.BtnUnlockProportion.UseAltImage = false;
            this.BtnUnlockProportion.Click += new System.EventHandler(this.BtnUnlockProportion_Click);
            // 
            // GbLaser
            // 
            resources.ApplyResources(this.GbLaser, "GbLaser");
            this.GbLaser.Controls.Add(this.tableLayoutPanel7);
            this.GbLaser.Name = "GbLaser";
            this.GbLaser.TabStop = false;
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.BtnModulationInfo, 3, 1);
            this.tableLayoutPanel7.Controls.Add(this.IIMinPower, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.CBLaserON, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.LblSmax, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.IIMaxPower, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.LblMinPerc, 2, 1);
            this.tableLayoutPanel7.Controls.Add(this.LblMaxPerc, 2, 2);
            this.tableLayoutPanel7.Controls.Add(this.BtnOnOffInfo, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.LblBorderTracing, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.LblLinearFilling, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.IIBorderTracing, 1, 3);
            this.tableLayoutPanel7.Controls.Add(this.IILinearFilling, 1, 4);
            this.tableLayoutPanel7.Controls.Add(this.LblBorderTracingmm, 2, 3);
            this.tableLayoutPanel7.Controls.Add(this.LblLinearFillingmm, 2, 4);
            this.tableLayoutPanel7.Controls.Add(this.label9, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.IILoopCounter, 1, 5);
            this.tableLayoutPanel7.Controls.Add(this.LblLaserMode, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.LblSmin, 0, 1);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel7_Paint);
            // 
            // BtnModulationInfo
            // 
            this.BtnModulationInfo.AltImage = null;
            this.BtnModulationInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnModulationInfo.Caption = null;
            this.BtnModulationInfo.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnModulationInfo, "BtnModulationInfo");
            this.BtnModulationInfo.Image = ((System.Drawing.Image)(resources.GetObject("BtnModulationInfo.Image")));
            this.BtnModulationInfo.Name = "BtnModulationInfo";
            this.BtnModulationInfo.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnModulationInfo, resources.GetString("BtnModulationInfo.ToolTip"));
            this.BtnModulationInfo.UseAltImage = false;
            // 
            // IIMinPower
            // 
            resources.ApplyResources(this.IIMinPower, "IIMinPower");
            this.IIMinPower.ForcedText = null;
            this.IIMinPower.ForceMinMax = false;
            this.IIMinPower.MaxValue = 999;
            this.IIMinPower.MinValue = 0;
            this.IIMinPower.Name = "IIMinPower";
            this.IIMinPower.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // CBLaserON
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.CBLaserON, 2);
            this.CBLaserON.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBLaserON.FormattingEnabled = true;
            resources.ApplyResources(this.CBLaserON, "CBLaserON");
            this.CBLaserON.Name = "CBLaserON";
            // 
            // LblSmax
            // 
            resources.ApplyResources(this.LblSmax, "LblSmax");
            this.LblSmax.Name = "LblSmax";
            this.LblSmax.Click += new System.EventHandler(this.LblSmax_Click);
            // 
            // IIMaxPower
            // 
            this.IIMaxPower.CurrentValue = 1000;
            resources.ApplyResources(this.IIMaxPower, "IIMaxPower");
            this.IIMaxPower.ForcedText = null;
            this.IIMaxPower.ForceMinMax = false;
            this.IIMaxPower.MaxValue = 1000;
            this.IIMaxPower.MinValue = 1;
            this.IIMaxPower.Name = "IIMaxPower";
            this.IIMaxPower.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.IIMaxPower.CurrentValueChanged += new LaserGRBL.UserControls.NumericInput.IntegerInputBase.CurrentValueChangedEventHandler(this.IIMaxPower_CurrentValueChanged);
            // 
            // LblMinPerc
            // 
            resources.ApplyResources(this.LblMinPerc, "LblMinPerc");
            this.LblMinPerc.Name = "LblMinPerc";
            // 
            // LblMaxPerc
            // 
            resources.ApplyResources(this.LblMaxPerc, "LblMaxPerc");
            this.LblMaxPerc.Name = "LblMaxPerc";
            // 
            // BtnOnOffInfo
            // 
            this.BtnOnOffInfo.AltImage = null;
            this.BtnOnOffInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnOnOffInfo.Caption = null;
            this.BtnOnOffInfo.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnOnOffInfo, "BtnOnOffInfo");
            this.BtnOnOffInfo.Image = ((System.Drawing.Image)(resources.GetObject("BtnOnOffInfo.Image")));
            this.BtnOnOffInfo.Name = "BtnOnOffInfo";
            this.BtnOnOffInfo.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnOnOffInfo, resources.GetString("BtnOnOffInfo.ToolTip"));
            this.BtnOnOffInfo.UseAltImage = false;
            // 
            // LblBorderTracing
            // 
            resources.ApplyResources(this.LblBorderTracing, "LblBorderTracing");
            this.LblBorderTracing.Name = "LblBorderTracing";
            // 
            // LblLinearFilling
            // 
            resources.ApplyResources(this.LblLinearFilling, "LblLinearFilling");
            this.LblLinearFilling.Name = "LblLinearFilling";
            // 
            // IIBorderTracing
            // 
            this.IIBorderTracing.CurrentValue = 1000;
            resources.ApplyResources(this.IIBorderTracing, "IIBorderTracing");
            this.IIBorderTracing.ForcedText = null;
            this.IIBorderTracing.ForceMinMax = false;
            this.IIBorderTracing.MaxValue = 4000;
            this.IIBorderTracing.MinValue = 1;
            this.IIBorderTracing.Name = "IIBorderTracing";
            this.IIBorderTracing.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // IILinearFilling
            // 
            this.IILinearFilling.CurrentValue = 1000;
            resources.ApplyResources(this.IILinearFilling, "IILinearFilling");
            this.IILinearFilling.ForcedText = null;
            this.IILinearFilling.ForceMinMax = false;
            this.IILinearFilling.MaxValue = 4000;
            this.IILinearFilling.MinValue = 1;
            this.IILinearFilling.Name = "IILinearFilling";
            this.IILinearFilling.NormalBorderColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // LblBorderTracingmm
            // 
            resources.ApplyResources(this.LblBorderTracingmm, "LblBorderTracingmm");
            this.LblBorderTracingmm.Name = "LblBorderTracingmm";
            // 
            // LblLinearFillingmm
            // 
            resources.ApplyResources(this.LblLinearFillingmm, "LblLinearFillingmm");
            this.LblLinearFillingmm.Name = "LblLinearFillingmm";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // IILoopCounter
            // 
            resources.ApplyResources(this.IILoopCounter, "IILoopCounter");
            this.IILoopCounter.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.IILoopCounter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.IILoopCounter.Name = "IILoopCounter";
            this.TT.SetToolTip(this.IILoopCounter, resources.GetString("IILoopCounter.ToolTip"));
            this.IILoopCounter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LblLaserMode
            // 
            resources.ApplyResources(this.LblLaserMode, "LblLaserMode");
            this.LblLaserMode.Name = "LblLaserMode";
            // 
            // LblSmin
            // 
            resources.ApplyResources(this.LblSmin, "LblSmin");
            this.LblSmin.Name = "LblSmin";
            // 
            // GbParameters
            // 
            resources.ApplyResources(this.GbParameters, "GbParameters");
            this.GbParameters.BackColor = System.Drawing.SystemColors.Control;
            this.GbParameters.Controls.Add(this.tableLayoutPanel2);
            this.GbParameters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GbParameters.Name = "GbParameters";
            this.GbParameters.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.TBRed, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.CbResize, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label28, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.LblGrayscale, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.CbMode, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.LblRed, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.LblGreen, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.LblBlue, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.TbBright, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.TbContrast, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.CbThreshold, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.TbThreshold, 1, 9);
            this.tableLayoutPanel2.Controls.Add(this.TBWhiteClip, 1, 8);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.TBBlue, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.TBGreen, 1, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // TBRed
            // 
            resources.ApplyResources(this.TBRed, "TBRed");
            this.TBRed.BackColor = System.Drawing.Color.Transparent;
            this.TBRed.BarInnerColor = System.Drawing.Color.Firebrick;
            this.TBRed.BarOuterColor = System.Drawing.Color.DarkRed;
            this.TBRed.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBRed.ElapsedInnerColor = System.Drawing.Color.Red;
            this.TBRed.ElapsedOuterColor = System.Drawing.Color.DarkRed;
            this.TBRed.LargeChange = ((uint)(5u));
            this.TBRed.Maximum = 160;
            this.TBRed.Minimum = 40;
            this.TBRed.Name = "TBRed";
            this.TBRed.SmallChange = ((uint)(1u));
            this.TBRed.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TBRed.ThumbSize = 8;
            this.TBRed.Value = 100;
            this.TBRed.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbResize
            // 
            resources.ApplyResources(this.CbResize, "CbResize");
            this.CbResize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbResize.FormattingEnabled = true;
            this.CbResize.Name = "CbResize";
            this.CbResize.SelectedItem = null;
            this.TT.SetToolTip(this.CbResize, resources.GetString("CbResize.ToolTip"));
            this.CbResize.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // LblGrayscale
            // 
            resources.ApplyResources(this.LblGrayscale, "LblGrayscale");
            this.LblGrayscale.Name = "LblGrayscale";
            // 
            // CbMode
            // 
            resources.ApplyResources(this.CbMode, "CbMode");
            this.CbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbMode.FormattingEnabled = true;
            this.CbMode.Name = "CbMode";
            this.CbMode.SelectedItem = null;
            this.TT.SetToolTip(this.CbMode, resources.GetString("CbMode.ToolTip"));
            this.CbMode.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // LblRed
            // 
            resources.ApplyResources(this.LblRed, "LblRed");
            this.LblRed.Name = "LblRed";
            // 
            // LblGreen
            // 
            resources.ApplyResources(this.LblGreen, "LblGreen");
            this.LblGreen.Name = "LblGreen";
            // 
            // LblBlue
            // 
            resources.ApplyResources(this.LblBlue, "LblBlue");
            this.LblBlue.Name = "LblBlue";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TbBright
            // 
            resources.ApplyResources(this.TbBright, "TbBright");
            this.TbBright.BackColor = System.Drawing.Color.Transparent;
            this.TbBright.BarInnerColor = System.Drawing.Color.Black;
            this.TbBright.BarOuterColor = System.Drawing.Color.Black;
            this.TbBright.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TbBright.ElapsedInnerColor = System.Drawing.Color.White;
            this.TbBright.ElapsedOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TbBright.LargeChange = ((uint)(5u));
            this.TbBright.Maximum = 160;
            this.TbBright.Minimum = 40;
            this.TbBright.Name = "TbBright";
            this.TbBright.SmallChange = ((uint)(1u));
            this.TbBright.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TbBright.ThumbSize = 8;
            this.TbBright.Value = 100;
            this.TbBright.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // TbContrast
            // 
            resources.ApplyResources(this.TbContrast, "TbContrast");
            this.TbContrast.BackColor = System.Drawing.Color.Transparent;
            this.TbContrast.BarInnerColor = System.Drawing.Color.Black;
            this.TbContrast.BarOuterColor = System.Drawing.Color.Black;
            this.TbContrast.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TbContrast.ElapsedInnerColor = System.Drawing.Color.White;
            this.TbContrast.ElapsedOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TbContrast.LargeChange = ((uint)(5u));
            this.TbContrast.Maximum = 160;
            this.TbContrast.Minimum = 40;
            this.TbContrast.Name = "TbContrast";
            this.TbContrast.SmallChange = ((uint)(1u));
            this.TbContrast.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TbContrast.ThumbSize = 8;
            this.TbContrast.Value = 100;
            this.TbContrast.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // CbThreshold
            // 
            resources.ApplyResources(this.CbThreshold, "CbThreshold");
            this.CbThreshold.Name = "CbThreshold";
            this.TT.SetToolTip(this.CbThreshold, resources.GetString("CbThreshold.ToolTip"));
            this.CbThreshold.UseVisualStyleBackColor = true;
            this.CbThreshold.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // TbThreshold
            // 
            resources.ApplyResources(this.TbThreshold, "TbThreshold");
            this.TbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.TbThreshold.BarInnerColor = System.Drawing.Color.Black;
            this.TbThreshold.BarOuterColor = System.Drawing.Color.Black;
            this.TbThreshold.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TbThreshold.ElapsedInnerColor = System.Drawing.Color.White;
            this.TbThreshold.ElapsedOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TbThreshold.LargeChange = ((uint)(5u));
            this.TbThreshold.Name = "TbThreshold";
            this.TbThreshold.SmallChange = ((uint)(1u));
            this.TbThreshold.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TbThreshold.ThumbSize = 8;
            this.TbThreshold.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // TBWhiteClip
            // 
            resources.ApplyResources(this.TBWhiteClip, "TBWhiteClip");
            this.TBWhiteClip.BackColor = System.Drawing.Color.Transparent;
            this.TBWhiteClip.BarInnerColor = System.Drawing.Color.Black;
            this.TBWhiteClip.BarOuterColor = System.Drawing.Color.Black;
            this.TBWhiteClip.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBWhiteClip.ElapsedInnerColor = System.Drawing.Color.White;
            this.TBWhiteClip.ElapsedOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TBWhiteClip.LargeChange = ((uint)(5u));
            this.TBWhiteClip.Name = "TBWhiteClip";
            this.TBWhiteClip.SmallChange = ((uint)(1u));
            this.TBWhiteClip.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TBWhiteClip.ThumbSize = 8;
            this.TBWhiteClip.Value = 5;
            this.TBWhiteClip.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // TBBlue
            // 
            resources.ApplyResources(this.TBBlue, "TBBlue");
            this.TBBlue.BackColor = System.Drawing.Color.Transparent;
            this.TBBlue.BarInnerColor = System.Drawing.Color.MediumBlue;
            this.TBBlue.BarOuterColor = System.Drawing.Color.DarkBlue;
            this.TBBlue.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBBlue.ElapsedInnerColor = System.Drawing.Color.DodgerBlue;
            this.TBBlue.ElapsedOuterColor = System.Drawing.Color.SteelBlue;
            this.TBBlue.LargeChange = ((uint)(5u));
            this.TBBlue.Maximum = 160;
            this.TBBlue.Minimum = 40;
            this.TBBlue.Name = "TBBlue";
            this.TBBlue.SmallChange = ((uint)(1u));
            this.TBBlue.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TBBlue.ThumbSize = 8;
            this.TBBlue.Value = 100;
            this.TBBlue.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // TBGreen
            // 
            resources.ApplyResources(this.TBGreen, "TBGreen");
            this.TBGreen.BackColor = System.Drawing.Color.Transparent;
            this.TBGreen.BarInnerColor = System.Drawing.Color.Green;
            this.TBGreen.BarOuterColor = System.Drawing.Color.DarkGreen;
            this.TBGreen.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBGreen.LargeChange = ((uint)(5u));
            this.TBGreen.Maximum = 160;
            this.TBGreen.Minimum = 40;
            this.TBGreen.Name = "TBGreen";
            this.TBGreen.SmallChange = ((uint)(1u));
            this.TBGreen.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TBGreen.ThumbSize = 8;
            this.TBGreen.Value = 100;
            this.TBGreen.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // GbConversionTool
            // 
            resources.ApplyResources(this.GbConversionTool, "GbConversionTool");
            this.GbConversionTool.BackColor = System.Drawing.SystemColors.Control;
            this.GbConversionTool.Controls.Add(this.tabControl1);
            this.GbConversionTool.Name = "GbConversionTool";
            this.GbConversionTool.TabStop = false;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabLineTrace);
            this.tabControl1.Controls.Add(this.tabVectorize);
            this.tabControl1.Controls.Add(this.tabCenterline);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // tabLineTrace
            // 
            this.tabLineTrace.BackColor = System.Drawing.SystemColors.Control;
            this.tabLineTrace.Controls.Add(this.TLP);
            resources.ApplyResources(this.tabLineTrace, "tabLineTrace");
            this.tabLineTrace.Name = "tabLineTrace";
            // 
            // TLP
            // 
            resources.ApplyResources(this.TLP, "TLP");
            this.TLP.BackColor = System.Drawing.SystemColors.Control;
            this.TLP.Controls.Add(this.CbDither, 0, 1);
            this.TLP.Controls.Add(this.label21, 0, 1);
            this.TLP.Controls.Add(this.CbDirections, 1, 2);
            this.TLP.Controls.Add(this.UDQuality, 1, 3);
            this.TLP.Controls.Add(this.label5, 0, 3);
            this.TLP.Controls.Add(this.label27, 0, 2);
            this.TLP.Controls.Add(this.label8, 2, 3);
            this.TLP.Controls.Add(this.BtnQualityInfo, 3, 3);
            this.TLP.Controls.Add(this.CbLinePreview, 0, 4);
            this.TLP.Name = "TLP";
            // 
            // CbDither
            // 
            this.TLP.SetColumnSpan(this.CbDither, 3);
            resources.ApplyResources(this.CbDither, "CbDither");
            this.CbDither.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbDither.FormattingEnabled = true;
            this.CbDither.Name = "CbDither";
            this.CbDither.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // CbDirections
            // 
            resources.ApplyResources(this.CbDirections, "CbDirections");
            this.TLP.SetColumnSpan(this.CbDirections, 3);
            this.CbDirections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbDirections.FormattingEnabled = true;
            this.CbDirections.Name = "CbDirections";
            this.CbDirections.SelectedItem = null;
            this.TT.SetToolTip(this.CbDirections, resources.GetString("CbDirections.ToolTip"));
            this.CbDirections.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // UDQuality
            // 
            this.UDQuality.DecimalPlaces = 3;
            resources.ApplyResources(this.UDQuality, "UDQuality");
            this.UDQuality.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.UDQuality.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UDQuality.Name = "UDQuality";
            this.TT.SetToolTip(this.UDQuality, resources.GetString("UDQuality.ToolTip"));
            this.UDQuality.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.UDQuality.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // BtnQualityInfo
            // 
            this.BtnQualityInfo.AltImage = null;
            resources.ApplyResources(this.BtnQualityInfo, "BtnQualityInfo");
            this.BtnQualityInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnQualityInfo.Caption = null;
            this.BtnQualityInfo.Coloration = System.Drawing.Color.Empty;
            this.BtnQualityInfo.Image = ((System.Drawing.Image)(resources.GetObject("BtnQualityInfo.Image")));
            this.BtnQualityInfo.Name = "BtnQualityInfo";
            this.BtnQualityInfo.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnQualityInfo, resources.GetString("BtnQualityInfo.ToolTip"));
            this.BtnQualityInfo.UseAltImage = false;
            // 
            // CbLinePreview
            // 
            resources.ApplyResources(this.CbLinePreview, "CbLinePreview");
            this.CbLinePreview.Checked = true;
            this.CbLinePreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TLP.SetColumnSpan(this.CbLinePreview, 3);
            this.CbLinePreview.Name = "CbLinePreview";
            this.TT.SetToolTip(this.CbLinePreview, resources.GetString("CbLinePreview.ToolTip"));
            this.CbLinePreview.UseVisualStyleBackColor = true;
            this.CbLinePreview.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // tabVectorize
            // 
            this.tabVectorize.BackColor = System.Drawing.SystemColors.Control;
            this.tabVectorize.Controls.Add(this.tableLayoutPanel5);
            resources.ApplyResources(this.tabVectorize, "tabVectorize");
            this.tabVectorize.Name = "tabVectorize";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.BtnAdaptiveQualityInfo, 6, 0);
            this.tableLayoutPanel5.Controls.Add(this.CbAdaptiveQuality, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.LAdaptiveQuality, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.label22, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.UDSpotRemoval, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.CbSpotRemoval, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.label24, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label23, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.UDOptimize, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.UDSmoothing, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.CbOptimize, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.CbSmoothing, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.label14, 4, 2);
            this.tableLayoutPanel5.Controls.Add(this.CbFillingDirection, 5, 2);
            this.tableLayoutPanel5.Controls.Add(this.LblFillingQuality, 4, 3);
            this.tableLayoutPanel5.Controls.Add(this.UDFillingQuality, 5, 3);
            this.tableLayoutPanel5.Controls.Add(this.LblFillingLineLbl, 7, 3);
            this.tableLayoutPanel5.Controls.Add(this.UDDownSample, 1, 3);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.CbDownSample, 2, 3);
            this.tableLayoutPanel5.Controls.Add(this.lOptimizeFast, 4, 1);
            this.tableLayoutPanel5.Controls.Add(this.BtnFillingQualityInfo, 8, 3);
            this.tableLayoutPanel5.Controls.Add(this.CbOptimizeFast, 5, 1);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // BtnAdaptiveQualityInfo
            // 
            this.BtnAdaptiveQualityInfo.AltImage = null;
            this.BtnAdaptiveQualityInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnAdaptiveQualityInfo.Caption = null;
            this.BtnAdaptiveQualityInfo.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnAdaptiveQualityInfo, "BtnAdaptiveQualityInfo");
            this.BtnAdaptiveQualityInfo.Image = ((System.Drawing.Image)(resources.GetObject("BtnAdaptiveQualityInfo.Image")));
            this.BtnAdaptiveQualityInfo.Name = "BtnAdaptiveQualityInfo";
            this.BtnAdaptiveQualityInfo.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnAdaptiveQualityInfo, resources.GetString("BtnAdaptiveQualityInfo.ToolTip"));
            this.BtnAdaptiveQualityInfo.UseAltImage = false;
            // 
            // CbAdaptiveQuality
            // 
            resources.ApplyResources(this.CbAdaptiveQuality, "CbAdaptiveQuality");
            this.CbAdaptiveQuality.Name = "CbAdaptiveQuality";
            this.CbAdaptiveQuality.UseVisualStyleBackColor = true;
            this.CbAdaptiveQuality.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // LAdaptiveQuality
            // 
            resources.ApplyResources(this.LAdaptiveQuality, "LAdaptiveQuality");
            this.LAdaptiveQuality.Name = "LAdaptiveQuality";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // UDSpotRemoval
            // 
            resources.ApplyResources(this.UDSpotRemoval, "UDSpotRemoval");
            this.UDSpotRemoval.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.UDSpotRemoval.Name = "UDSpotRemoval";
            this.TT.SetToolTip(this.UDSpotRemoval, resources.GetString("UDSpotRemoval.ToolTip"));
            this.UDSpotRemoval.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.UDSpotRemoval.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbSpotRemoval
            // 
            resources.ApplyResources(this.CbSpotRemoval, "CbSpotRemoval");
            this.CbSpotRemoval.Name = "CbSpotRemoval";
            this.CbSpotRemoval.UseVisualStyleBackColor = true;
            this.CbSpotRemoval.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // UDOptimize
            // 
            resources.ApplyResources(this.UDOptimize, "UDOptimize");
            this.UDOptimize.DecimalPlaces = 1;
            this.UDOptimize.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.UDOptimize.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.UDOptimize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.UDOptimize.Name = "UDOptimize";
            this.TT.SetToolTip(this.UDOptimize, resources.GetString("UDOptimize.ToolTip"));
            this.UDOptimize.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.UDOptimize.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // UDSmoothing
            // 
            resources.ApplyResources(this.UDSmoothing, "UDSmoothing");
            this.UDSmoothing.DecimalPlaces = 1;
            this.UDSmoothing.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.UDSmoothing.Name = "UDSmoothing";
            this.TT.SetToolTip(this.UDSmoothing, resources.GetString("UDSmoothing.ToolTip"));
            this.UDSmoothing.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.UDSmoothing.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbOptimize
            // 
            resources.ApplyResources(this.CbOptimize, "CbOptimize");
            this.CbOptimize.Name = "CbOptimize";
            this.CbOptimize.UseVisualStyleBackColor = true;
            this.CbOptimize.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbSmoothing
            // 
            resources.ApplyResources(this.CbSmoothing, "CbSmoothing");
            this.CbSmoothing.Name = "CbSmoothing";
            this.CbSmoothing.UseVisualStyleBackColor = true;
            this.CbSmoothing.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // CbFillingDirection
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.CbFillingDirection, 3);
            resources.ApplyResources(this.CbFillingDirection, "CbFillingDirection");
            this.CbFillingDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbFillingDirection.FormattingEnabled = true;
            this.CbFillingDirection.Name = "CbFillingDirection";
            this.CbFillingDirection.SelectedItem = null;
            this.TT.SetToolTip(this.CbFillingDirection, resources.GetString("CbFillingDirection.ToolTip"));
            this.CbFillingDirection.SelectedIndexChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // LblFillingQuality
            // 
            resources.ApplyResources(this.LblFillingQuality, "LblFillingQuality");
            this.LblFillingQuality.Name = "LblFillingQuality";
            // 
            // UDFillingQuality
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.UDFillingQuality, 2);
            this.UDFillingQuality.DecimalPlaces = 3;
            resources.ApplyResources(this.UDFillingQuality, "UDFillingQuality");
            this.UDFillingQuality.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.UDFillingQuality.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.UDFillingQuality.Name = "UDFillingQuality";
            this.TT.SetToolTip(this.UDFillingQuality, resources.GetString("UDFillingQuality.ToolTip"));
            this.UDFillingQuality.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.UDFillingQuality.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // LblFillingLineLbl
            // 
            resources.ApplyResources(this.LblFillingLineLbl, "LblFillingLineLbl");
            this.LblFillingLineLbl.Name = "LblFillingLineLbl";
            // 
            // UDDownSample
            // 
            resources.ApplyResources(this.UDDownSample, "UDDownSample");
            this.UDDownSample.DecimalPlaces = 1;
            this.UDDownSample.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.UDDownSample.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.UDDownSample.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UDDownSample.Name = "UDDownSample";
            this.TT.SetToolTip(this.UDDownSample, resources.GetString("UDDownSample.ToolTip"));
            this.UDDownSample.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UDDownSample.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CbDownSample
            // 
            resources.ApplyResources(this.CbDownSample, "CbDownSample");
            this.CbDownSample.Name = "CbDownSample";
            this.CbDownSample.UseVisualStyleBackColor = true;
            this.CbDownSample.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // lOptimizeFast
            // 
            resources.ApplyResources(this.lOptimizeFast, "lOptimizeFast");
            this.lOptimizeFast.Name = "lOptimizeFast";
            // 
            // BtnFillingQualityInfo
            // 
            this.BtnFillingQualityInfo.AltImage = null;
            this.BtnFillingQualityInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnFillingQualityInfo.Caption = null;
            this.BtnFillingQualityInfo.Coloration = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BtnFillingQualityInfo, "BtnFillingQualityInfo");
            this.BtnFillingQualityInfo.Image = ((System.Drawing.Image)(resources.GetObject("BtnFillingQualityInfo.Image")));
            this.BtnFillingQualityInfo.Name = "BtnFillingQualityInfo";
            this.BtnFillingQualityInfo.SizingMode = LaserGRBL.UserControls.ImageButton.SizingModes.FixedSize;
            this.TT.SetToolTip(this.BtnFillingQualityInfo, resources.GetString("BtnFillingQualityInfo.ToolTip"));
            this.BtnFillingQualityInfo.UseAltImage = false;
            // 
            // CbOptimizeFast
            // 
            resources.ApplyResources(this.CbOptimizeFast, "CbOptimizeFast");
            this.CbOptimizeFast.Name = "CbOptimizeFast";
            this.CbOptimizeFast.UseVisualStyleBackColor = true;
            this.CbOptimizeFast.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // tabCenterline
            // 
            this.tabCenterline.BackColor = System.Drawing.SystemColors.Control;
            this.tabCenterline.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.tabCenterline, "tabCenterline");
            this.tabCenterline.Name = "tabCenterline";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.TBLineThreshold, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.TBCornerThreshold, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.CbLineThreshold, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.CbCornerThreshold, 3, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // TBLineThreshold
            // 
            resources.ApplyResources(this.TBLineThreshold, "TBLineThreshold");
            this.TBLineThreshold.BackColor = System.Drawing.Color.Transparent;
            this.TBLineThreshold.BarInnerColor = System.Drawing.Color.LightGoldenrodYellow;
            this.TBLineThreshold.BarOuterColor = System.Drawing.Color.Gold;
            this.TBLineThreshold.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBLineThreshold.ElapsedInnerColor = System.Drawing.Color.Yellow;
            this.TBLineThreshold.ElapsedOuterColor = System.Drawing.Color.Gold;
            this.TBLineThreshold.LargeChange = ((uint)(5u));
            this.TBLineThreshold.Name = "TBLineThreshold";
            this.TBLineThreshold.SmallChange = ((uint)(1u));
            this.TBLineThreshold.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TT.SetToolTip(this.TBLineThreshold, resources.GetString("TBLineThreshold.ToolTip"));
            this.TBLineThreshold.Value = 10;
            this.TBLineThreshold.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // TBCornerThreshold
            // 
            resources.ApplyResources(this.TBCornerThreshold, "TBCornerThreshold");
            this.TBCornerThreshold.BackColor = System.Drawing.Color.Transparent;
            this.TBCornerThreshold.BarInnerColor = System.Drawing.Color.LightGoldenrodYellow;
            this.TBCornerThreshold.BarOuterColor = System.Drawing.Color.Gold;
            this.TBCornerThreshold.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.TBCornerThreshold.ElapsedInnerColor = System.Drawing.Color.Yellow;
            this.TBCornerThreshold.ElapsedOuterColor = System.Drawing.Color.Gold;
            this.TBCornerThreshold.LargeChange = ((uint)(5u));
            this.TBCornerThreshold.Maximum = 360;
            this.TBCornerThreshold.Name = "TBCornerThreshold";
            this.TBCornerThreshold.SmallChange = ((uint)(1u));
            this.TBCornerThreshold.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.TT.SetToolTip(this.TBCornerThreshold, resources.GetString("TBCornerThreshold.ToolTip"));
            this.TBCornerThreshold.Value = 110;
            this.TBCornerThreshold.ValueChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbLineThreshold
            // 
            resources.ApplyResources(this.CbLineThreshold, "CbLineThreshold");
            this.CbLineThreshold.Name = "CbLineThreshold";
            this.CbLineThreshold.UseVisualStyleBackColor = true;
            this.CbLineThreshold.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // CbCornerThreshold
            // 
            resources.ApplyResources(this.CbCornerThreshold, "CbCornerThreshold");
            this.CbCornerThreshold.Name = "CbCornerThreshold";
            this.CbCornerThreshold.UseVisualStyleBackColor = true;
            this.CbCornerThreshold.CheckedChanged += new System.EventHandler(this.UISetting_Changed);
            // 
            // WT
            // 
            this.WT.Interval = 50;
            this.WT.Tick += new System.EventHandler(this.WTTick);
            // 
            // RasterToLaserForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Controls.Add(this.TlpMain);
            this.MinimizeBox = false;
            this.Name = "RasterToLaserForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RasterToLaserFormFormClosing);
            this.Load += new System.EventHandler(this.RasterToLaserForm_Load);
            this.TlpMain.ResumeLayout(false);
            this.TlpMain.PerformLayout();
            this.TCOriginalPreview.ResumeLayout(false);
            this.TpPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PbConverted)).EndInit();
            this.TpOriginal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PbOriginal)).EndInit();
            this.FlipControl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.TlpLeft.ResumeLayout(false);
            this.GbSize.ResumeLayout(false);
            this.GbSize.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.GbLaser.ResumeLayout(false);
            this.GbLaser.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IILoopCounter)).EndInit();
            this.GbParameters.ResumeLayout(false);
            this.GbParameters.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.GbConversionTool.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabLineTrace.ResumeLayout(false);
            this.tabLineTrace.PerformLayout();
            this.TLP.ResumeLayout(false);
            this.TLP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDQuality)).EndInit();
            this.tabVectorize.ResumeLayout(false);
            this.tabVectorize.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UDSpotRemoval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDOptimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDSmoothing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDFillingQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UDDownSample)).EndInit();
            this.tabCenterline.ResumeLayout(false);
            this.tabCenterline.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel TlpMain;
		private System.Windows.Forms.TabControl TCOriginalPreview;
		private System.Windows.Forms.TabPage TpPreview;
		private System.Windows.Forms.PictureBox PbConverted;
		private System.Windows.Forms.TabPage TpOriginal;
		private System.Windows.Forms.PictureBox PbOriginal;
		private System.Windows.Forms.Button BtnCreate;
		private System.Windows.Forms.GroupBox GbConversionTool;
		private System.Windows.Forms.TableLayoutPanel TlpLeft;
		private System.Windows.Forms.Timer WT;
		private System.Windows.Forms.TableLayoutPanel FlipControl;
		private LaserGRBL.UserControls.ImageButton BtFlipV;
		private LaserGRBL.UserControls.ImageButton BtFlipH;
		private LaserGRBL.UserControls.ImageButton BtRotateCW;
		private LaserGRBL.UserControls.ImageButton BtRotateCCW;
		private LaserGRBL.UserControls.ImageButton BtnCrop;
		private LaserGRBL.UserControls.ImageButton BtnRevert;
		private System.Windows.Forms.ToolTip TT;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button BtnCancel;
		private UserControls.ImageButton BtnReverse;
		private UserControls.ImageButton BtnAutoTrim;
        private System.Windows.Forms.GroupBox GbLaser;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private UserControls.ImageButton BtnModulationInfo;
        private System.Windows.Forms.Label LblSmin;
        private UserControls.NumericInput.IntegerInputRanged IIMinPower;
        private System.Windows.Forms.Label LblLaserMode;
        private System.Windows.Forms.ComboBox CBLaserON;
        private System.Windows.Forms.Label LblSmax;
        private UserControls.NumericInput.IntegerInputRanged IIMaxPower;
        private System.Windows.Forms.Label LblMinPerc;
        private System.Windows.Forms.Label LblMaxPerc;
        private UserControls.ImageButton BtnOnOffInfo;
        private System.Windows.Forms.Label LblBorderTracing;
        private System.Windows.Forms.Label LblLinearFilling;
        private UserControls.NumericInput.IntegerInputRanged IIBorderTracing;
        private UserControls.NumericInput.IntegerInputRanged IILinearFilling;
        private System.Windows.Forms.Label LblBorderTracingmm;
        private System.Windows.Forms.Label LblLinearFillingmm;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown IILoopCounter;
        private System.Windows.Forms.GroupBox GbSize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private UserControls.NumericInput.DecimalInputRanged IIOffsetX;
        private UserControls.NumericInput.DecimalInputRanged IIOffsetY;
        private UserControls.NumericInput.DecimalInputRanged IISizeH;
        private UserControls.NumericInput.DecimalInputRanged IISizeW;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox CbAutosize;
        private UserControls.NumericInput.IntegerInputRanged IIDpi;
        private UserControls.ImageButton BtnDPI;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private UserControls.ImageButton BtnReset;
        private UserControls.ImageButton BtnCenter;
        private UserControls.ImageButton BtnUnlockProportion;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLineTrace;
        private System.Windows.Forms.TabPage tabVectorize;
        private System.Windows.Forms.TabPage tabCenterline;
        private System.Windows.Forms.GroupBox GbParameters;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private UserControls.EnumComboBox CbResize;
        private System.Windows.Forms.Label LblGrayscale;
        private UserControls.EnumComboBox CbMode;
        private System.Windows.Forms.Label LblRed;
        private System.Windows.Forms.Label LblBlue;
        private System.Windows.Forms.Label LblGreen;
        private System.Windows.Forms.Label label2;
        private UserControls.ColorSlider TbBright;
        private UserControls.ColorSlider TBBlue;
        private UserControls.ColorSlider TbContrast;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox CbThreshold;
        private System.Windows.Forms.Label label28;
        private UserControls.ColorSlider TbThreshold;
        private UserControls.ColorSlider TBWhiteClip;
        private System.Windows.Forms.Label label4;
        private UserControls.ColorSlider TBGreen;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private UserControls.ImageButton BtnAdaptiveQualityInfo;
        private System.Windows.Forms.CheckBox CbAdaptiveQuality;
        private System.Windows.Forms.Label LAdaptiveQuality;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown UDSpotRemoval;
        private System.Windows.Forms.CheckBox CbSpotRemoval;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown UDOptimize;
        private System.Windows.Forms.NumericUpDown UDSmoothing;
        private System.Windows.Forms.CheckBox CbOptimize;
        private System.Windows.Forms.CheckBox CbSmoothing;
        private System.Windows.Forms.Label label14;
        private UserControls.EnumComboBox CbFillingDirection;
        private System.Windows.Forms.Label LblFillingQuality;
        private System.Windows.Forms.NumericUpDown UDFillingQuality;
        private System.Windows.Forms.Label LblFillingLineLbl;
        private System.Windows.Forms.NumericUpDown UDDownSample;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CbDownSample;
        private System.Windows.Forms.Label lOptimizeFast;
        private UserControls.ImageButton BtnFillingQualityInfo;
        private System.Windows.Forms.CheckBox CbOptimizeFast;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private UserControls.ColorSlider TBLineThreshold;
        private UserControls.ColorSlider TBCornerThreshold;
        private System.Windows.Forms.CheckBox CbLineThreshold;
        private System.Windows.Forms.CheckBox CbCornerThreshold;
        private System.Windows.Forms.TableLayoutPanel TLP;
        private UserControls.EnumComboBox CbDirections;
        private System.Windows.Forms.NumericUpDown UDQuality;
        private System.Windows.Forms.CheckBox CbLinePreview;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label8;
        private UserControls.ImageButton BtnQualityInfo;
        private System.Windows.Forms.ComboBox CbDither;
        private System.Windows.Forms.Label label21;
        private UserControls.WaitingProgressBar WB;
        private UserControls.ColorSlider TBRed;
    }
}