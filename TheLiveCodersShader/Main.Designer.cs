namespace TheLiveCodersShader
{
  partial class Main
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
      this.glControl = new OpenGL.GlControl();
      this.SuspendLayout();
      //
      // glControl
      //
      this.glControl.Animation = true;
      this.glControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(200)))), ((int)(((byte)(64)))));
      this.glControl.CausesValidation = false;
      this.glControl.ColorBits = ((uint)(24u));
      this.glControl.DepthBits = ((uint)(0u));
      this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glControl.Location = new System.Drawing.Point(0, 0);
      this.glControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.glControl.MultisampleBits = ((uint)(0u));
      this.glControl.Name = "glControl";
      this.glControl.Size = new System.Drawing.Size(1578, 1144);
      this.glControl.StencilBits = ((uint)(0u));
      this.glControl.TabIndex = 0;
      this.glControl.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextCreated);
      this.glControl.ContextDestroying += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextDestroying);
      this.glControl.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_Render);
      this.glControl.ContextUpdate += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextUpdate);
      //
      // Main
      //
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(1578, 1144);
      this.Controls.Add(this.glControl);
      this.Name = "Main";
      this.Text = "The Live Coders Conference - April 9, 2020";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
      this.ResumeLayout(false);

    }

        #endregion

        private OpenGL.GlControl glControl;
    }
}

