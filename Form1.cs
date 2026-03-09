namespace GraphicCode;

using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.WinForms;

public partial class Form1 : Form
{
    // El null! quita el warning CS8618
    private GLControl glControl = null!;
    private System.Windows.Forms.Timer _timer = null!;

    public Form1()
    {
        InitializeComponent();

        glControl = new GLControl();
        glControl.Dock = DockStyle.Fill;
        
        // Eventos esenciales
        glControl.Load += OnLoad;
        glControl.Paint += OnPaint;
        glControl.Resize += OnResize;

        this.Text = "GraphicOS";
        this.Size = new System.Drawing.Size(1024, 768);
        this.StartPosition = FormStartPosition.CenterScreen;
        
        this.Controls.Add(glControl);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        glControl.MakeCurrent();
        
        // Configuramos estados una sola vez
        GL.ClearColor(0.05f, 0.15f, 0.3f, 1.0f);
        
        // Timer para el bucle de renderizado
        _timer = new System.Windows.Forms.Timer();
        _timer.Interval = 16; 
        _timer.Tick += (s, ev) => glControl.Invalidate();
        _timer.Start();
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (!glControl.IsHandleCreated) return;
        glControl.MakeCurrent();
        GL.Viewport(0, 0, glControl.Width, glControl.Height);
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        if (!glControl.IsHandleCreated) return;

        glControl.MakeCurrent();

        // LIMPIEZA ABSOLUTA DE ESTADOS
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // RESET DE MATRICES
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

        // DIBUJO DIRECTO (Probaremos este modo de nuevo con estados forzados)
        DrawDesktop();

        glControl.SwapBuffers();
    }

    private void DrawDesktop()
    {
        // 1. Fondo con Degradado
        GL.Begin(PrimitiveType.Quads);
            // Abajo (Oscuro)
            GL.Color3(0.01f, 0.02f, 0.1f);
            GL.Vertex2(-1.0f, -1.0f);
            GL.Vertex2(1.0f, -1.0f);
            // Arriba (Claro)
            GL.Color3(0.1f, 0.4f, 0.7f);
            GL.Vertex2(1.0f, 1.0f);
            GL.Vertex2(-1.0f, 1.0f);
        GL.End();

        // 2. Barra de Tareas (Semi-transparente)
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Begin(PrimitiveType.Quads);
            GL.Color4(0.0f, 0.0f, 0.0f, 0.8f);
            GL.Vertex2(-1.0f, -1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.Vertex2(1.0f, -0.9f);
            GL.Vertex2(-1.0f, -0.9f);
        GL.End();

        GL.Disable(EnableCap.Blend);
    }

   
}