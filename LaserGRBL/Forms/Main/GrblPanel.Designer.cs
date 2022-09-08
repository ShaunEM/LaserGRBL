namespace LaserGRBL.UserControls
{
	partial class GrblPanel
	{
		/// <summary> 
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Liberare le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Codice generato da Progettazione componenti

		/// <summary> 
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // GrblPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GrblPanel";
            this.Size = new System.Drawing.Size(731, 492);
            this.Load += new System.EventHandler(this.GrblPanel_Load);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.GrblPanel_Scroll);
            this.Click += new System.EventHandler(this.GrblPanel_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GrblPanel_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GrblPanel_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GrblPanel_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GrblPanel_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GrblPanel_MouseUp);
            this.ResumeLayout(false);

		}

		#endregion

	}
}
