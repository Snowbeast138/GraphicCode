using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GraphicCode
{
    public partial class Form1 : Form
    {
        // --- Estructuras Auxiliares ---
        private class EstrellaFugace
        {
            public PointF Posicion;

            public PointF Velocidad;

            public int

                    Vida,
                    VidaMax;
        }

        private class ElementoVegetacion
        {
            public PointF Posicion;

            public Color ColorPrincipal;

            public float

                    Escala,
                    FaseViento;

            public bool EsFlor;
        }

        private class Nube
        {
            public PointF Posicion;

            public float

                    Velocidad,
                    Escala;
        }

        private class OndaAgua
        {
            public PointF Posicion;

            public float Radio;

            public int Opacidad;
        }

        private class Pajaro
        {
            public PointF Posicion;

            public float Velocidad;
        }

        private class GotaLluvia
        {
            public PointF Posicion;

            public float Velocidad;
        }

        private class Luciernaga
        {
            public PointF Posicion;

            public float FaseBrillo;

            public PointF Direccion;
        }

        private class Arbol
        {
            public PointF Posicion;

            public float Escala;

            public List<PointF> ClusterOffsets;
        }

        private class Petalo
        {
            public PointF Posicion;

            public float

                    VelocidadX,
                    VelocidadY,
                    Rotacion,
                    VelRot;
        }

        // --- Estado Global ---
        private System.Windows.Forms.Timer mainTimer;

        private float minutosDia = 480;

        private float velocidadTiempo = 1.0f;

        private float faseAnimacion = 0;

        private float animacionPollo = 0;

        private Random rng = new Random();

        // Clima y Rayos
        private bool estaLloviendo = false;

        private float intensidadViento = 1.0f;

        private int flashRayo = 0;

        private List<PointF> puntosRayo = new List<PointF>();

        // Listas
        private List<Point> estrellasFijas = new List<Point>();

        private List<EstrellaFugace>
            estrellasFugaces = new List<EstrellaFugace>();

        private List<PointF>

                posPollitos = new List<PointF>(),
                targetPollitos = new List<PointF>();

        private List<ElementoVegetacion>
            vegetacion = new List<ElementoVegetacion>();

        private List<Nube> nubes = new List<Nube>();

        private List<OndaAgua> ondas = new List<OndaAgua>();

        private List<Pajaro> parvada = new List<Pajaro>();

        private List<GotaLluvia> lluvia = new List<GotaLluvia>();

        private List<Luciernaga> luciernagas = new List<Luciernaga>();

        private List<Arbol> arboles = new List<Arbol>();

        private List<Petalo> petalos = new List<Petalo>();

        public Form1()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(1000, 700);
            this.Text = "Simulación: Cerezos, Clima y Parvadas";
            InicializarEscena();

            mainTimer = new System.Windows.Forms.Timer { Interval = 16 };
            mainTimer.Tick += (s, e) =>
            {
                ActualizarLogica();
                this.Invalidate();
            };
            mainTimer.Start();
        }

        private void InicializarEscena()
        {
            for (int i = 0; i < 80; i++)
            estrellasFijas.Add(new Point(rng.Next(this.Width), rng.Next(400)));
            for (int i = 0; i < 6; i++)
            {
                posPollitos
                    .Add(new PointF(rng.Next(400, 800), rng.Next(450, 600)));
                targetPollitos
                    .Add(new PointF(rng.Next(400, 800), rng.Next(450, 600)));
            }
            for (int i = 0; i < 60; i++)
            vegetacion
                .Add(new ElementoVegetacion {
                    Posicion =
                        new PointF(rng.Next(10, 990), rng.Next(410, 680)),
                    EsFlor = rng.NextDouble() > 0.7,
                    ColorPrincipal =
                        Color
                            .FromArgb(rng.Next(200, 255),
                            rng.Next(100, 200),
                            0),
                    Escala = (float)(rng.NextDouble() * 0.4 + 0.8),
                    FaseViento = (float)(rng.NextDouble() * Math.PI)
                });
            for (int i = 0; i < 5; i++)
            nubes
                .Add(new Nube {
                    Posicion =
                        new PointF(rng.Next(this.Width), rng.Next(50, 180)),
                    Velocidad = (float)(rng.NextDouble() * 0.4 + 0.3),
                    Escala = (float)(rng.NextDouble() * 0.5 + 0.7)
                });
            for (int i = 0; i < 15; i++)
            luciernagas
                .Add(new Luciernaga {
                    Posicion =
                        new PointF(rng.Next(100, 900), rng.Next(400, 650)),
                    FaseBrillo = (float)(rng.NextDouble() * Math.PI * 2),
                    Direccion =
                        new PointF((float)(rng.NextDouble() - 0.5) * 2,
                            (float)(rng.NextDouble() - 0.5) * 2)
                });

            float[] arbolX = { 150, 850, 420 };
            foreach (float x in arbolX)
            {
                var a =
                    new Arbol {
                        Posicion = new PointF(x, 420),
                        Escala = (float)(rng.NextDouble() * 0.4 + 0.8),
                        ClusterOffsets = new List<PointF>()
                    };
                for (int j = 0; j < 6; j++)
                a
                    .ClusterOffsets
                    .Add(new PointF(rng.Next(-50, 50), rng.Next(-80, 0)));
                arboles.Add (a);
            }
        }

        private void ActualizarLogica()
        {
            minutosDia = (minutosDia + velocidadTiempo) % 1440;
            faseAnimacion += 0.05f;
            animacionPollo += 0.15f;

            // --- Lógica de Clima ---
            if (rng.Next(5000) < 5) estaLloviendo = !estaLloviendo;
            intensidadViento = estaLloviendo ? 3.5f : 1.2f;

            // --- Rayos y Lluvia ---
            if (estaLloviendo)
            {
                if (flashRayo <= 0 && rng.Next(250) < 2)
                {
                    flashRayo = 10;
                    puntosRayo.Clear();
                    float
                        cX = rng.Next(200, 800),
                        cY = 0;
                    puntosRayo.Add(new PointF(cX, cY));
                    while (cY < 450)
                    {
                        cX += rng.Next(-40, 41);
                        cY += rng.Next(30, 70);
                        puntosRayo.Add(new PointF(cX, cY));
                    }
                }
                for (int i = 0; i < 20; i++)
                lluvia
                    .Add(new GotaLluvia {
                        Posicion =
                            new PointF(rng.Next(this.Width + 200) - 100, -10),
                        Velocidad = rng.Next(18, 28)
                    });
                if (rng.Next(100) > 92)
                {
                    // Impacto de lluvia en el lago
                    PointF pL =
                        new PointF(rng.Next(50, 350), rng.Next(440, 580));
                    if (EstaEnLago(pL))
                        ondas
                            .Add(new OndaAgua {
                                Posicion = pL,
                                Radio = 1,
                                Opacidad = 100
                            });
                }
            }
            if (flashRayo > 0) flashRayo--;

            for (int i = lluvia.Count - 1; i >= 0; i--)
            {
                lluvia[i].Posicion.Y += lluvia[i].Velocidad;
                lluvia[i].Posicion.X += intensidadViento * 2;
                if (lluvia[i].Posicion.Y > this.Height) lluvia.RemoveAt(i);
            }

            // --- Parvada de Pájaros ---
            if (
                !EsNoche() &&
                !estaLloviendo &&
                parvada.Count == 0 &&
                rng.Next(1000) > 997
            )
            {
                float yBase = rng.Next(80, 200);
                for (int i = 0; i < 6; i++)
                parvada
                    .Add(new Pajaro {
                        Posicion = new PointF(-50 - (i * 30), yBase + (i * 12)),
                        Velocidad = 4.5f
                    });
            }
            for (int i = parvada.Count - 1; i >= 0; i--)
            {
                parvada[i].Posicion.X += parvada[i].Velocidad;
                if (parvada[i].Posicion.X > this.Width + 250)
                    parvada.RemoveAt(i);
            }

            // --- Pétalos ---
            if (rng.Next(100) < (estaLloviendo ? 15 : 5))
            {
                var arb = arboles[rng.Next(arboles.Count)];
                petalos
                    .Add(new Petalo {
                        Posicion =
                            new PointF(arb.Posicion.X + rng.Next(-50, 50),
                                arb.Posicion.Y - 80),
                        VelocidadX =
                            (float)(rng.NextDouble() * intensidadViento),
                        VelocidadY = (float)(rng.NextDouble() * 1 + 1),
                        Rotacion = 0,
                        VelRot = (float)(rng.NextDouble() * 10)
                    });
            }
            for (int i = petalos.Count - 1; i >= 0; i--)
            {
                petalos[i].Posicion.X +=
                    petalos[i].VelocidadX + (float) Math.Sin(faseAnimacion) * 2;
                petalos[i].Posicion.Y += petalos[i].VelocidadY;
                petalos[i].Rotacion += petalos[i].VelRot;
                if (petalos[i].Posicion.Y > 680) petalos.RemoveAt(i);
            }

            // --- Otros Movimientos (Nubes, Luciérnagas, Pollitos) ---
            foreach (var n in nubes)
            {
                n.Posicion.X += n.Velocidad;
                if (n.Posicion.X > this.Width + 100) n.Posicion.X = -150;
            }
            if (EsNoche())
            {
                foreach (var l in luciernagas)
                {
                    l.Posicion =
                        new PointF(l.Posicion.X + l.Direccion.X,
                            l.Posicion.Y + l.Direccion.Y);
                    l.FaseBrillo += 0.15f;
                    if (
                        l.Posicion.X < 20 ||
                        l.Posicion.X > 980 ||
                        l.Posicion.Y < 380 ||
                        rng.Next(100) > 98
                    )
                        l.Direccion =
                            new PointF((float)(rng.NextDouble() - 0.5) * 3,
                                (float)(rng.NextDouble() - 0.5) * 3);
                }
            }
            if (!EsNoche())
            {
                for (int i = 0; i < posPollitos.Count; i++)
                {
                    float
                        dx = targetPollitos[i].X - posPollitos[i].X,
                        dy = targetPollitos[i].Y - posPollitos[i].Y,
                        dist = (float) Math.Sqrt(dx * dx + dy * dy);
                    if (dist > 2)
                        posPollitos[i] =
                            new PointF(posPollitos[i].X + dx / dist * 1.2f,
                                posPollitos[i].Y + dy / dist * 0.8f);
                    else
                        targetPollitos[i] =
                            new PointF(rng.Next(50, 950), rng.Next(420, 650));
                    if (EstaEnLago(posPollitos[i]) && rng.Next(10) > 7)
                        ondas
                            .Add(new OndaAgua {
                                Posicion = posPollitos[i],
                                Radio = 1,
                                Opacidad = 150
                            });
                }
            }
            for (int i = ondas.Count - 1; i >= 0; i--)
            {
                ondas[i].Radio += 2.0f;
                ondas[i].Opacidad -= 5;
                if (ondas[i].Opacidad <= 0) ondas.RemoveAt(i);
            }
        }

        private bool EstaEnLago(PointF p)
        {
            using (GraphicsPath path = GetLagoPath())
                return path.IsVisible(p);
        }

        private GraphicsPath GetLagoPath()
        {
            GraphicsPath p = new GraphicsPath();
            Point[] pts =
            {
                new Point(50, 500),
                new Point(150, 440),
                new Point(300, 460),
                new Point(350, 520),
                new Point(250, 580),
                new Point(100, 560)
            };
            p.AddClosedCurve(pts, 0.7f);
            return p;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DibujarCieloSuave (g);
            if (EsNoche())
            {
                DibujarEstrellasFijas (g);
                DibujarEstrellasFugaces (g);
            }
            DibujarNubes (g);
            if (estaLloviendo) DibujarRayo(g);
            DibujarPajaros (g);
            DibujarSolLuna (g);
            DibujarPastoBase (g);
            DibujarSombras (g);
            DibujarLagoConOndas (g);
            DibujarVegetacionConViento (g);
            DibujarArbolesSakura (g);
            DibujarGranero3D (g);
            if (!EsNoche()) DibujarPollitosCaminantes(g);
            if (EsNoche()) DibujarLuciernagas(g);
            DibujarPetalos (g);
            if (estaLloviendo) DibujarLluvia(g);
            if (flashRayo > 0)
                g
                    .FillRectangle(new SolidBrush(Color
                            .FromArgb(flashRayo * 20, Color.White)),
                    this.ClientRectangle);
            DibujarCronometro (g);
        }

        private void DibujarArbolesSakura(Graphics g)
        {
            float fV =
                (float)
                Math.Sin(faseAnimacion * (estaLloviendo ? 2.8f : 1.2f)) *
                (5 * intensidadViento);

            // Colores estilo ilustración plana
            Color colorBase = Color.FromArgb(255, 105, 145); // Rosa vibrante
            Color colorLuz = Color.FromArgb(255, 160, 190); // Rosa pastel
            Color colorSombra = Color.FromArgb(200, 80, 120); // Rosa profundo

            foreach (var a in arboles)
            {
                g.TranslateTransform(a.Posicion.X, a.Posicion.Y);
                g.ScaleTransform(a.Escala, a.Escala);

                // 1. Dibujar el Tronco (Curvo y oscuro)
                using (Pen pTrunk = new Pen(Color.FromArgb(45, 30, 25), 14))
                {
                    pTrunk.StartCap = LineCap.Round;
                    pTrunk.EndCap = LineCap.Round;

                    // Curva principal del tronco
                    g
                        .DrawBezier(pTrunk,
                        new Point(0, 0),
                        new Point(-15, -30),
                        new Point(10, -60),
                        new Point(0, -90));
                }

                // 2. Dibujar los Cúmulos de Hojas (Sakura)
                foreach (var offset in a.ClusterOffsets)
                {
                    GraphicsState state = g.Save();
                    g.TranslateTransform(offset.X, offset.Y - 40); // Ajuste de altura
                    g.RotateTransform (fV);

                    // Creamos una forma de "nube" compuesta por varias elipses superpuestas
                    DibujarHojaEstilizada (g, colorBase, colorLuz, colorSombra);

                    g.Restore (state);
                }
                g.ResetTransform();
            }
        }

        // Método auxiliar para crear la forma orgánica de la imagen
        private void DibujarHojaEstilizada(
            Graphics g,
            Color baseCol,
            Color luzCol,
            Color sombraCol
        )
        {
            // Sombra (atrás y un poco hacia abajo)
            using (SolidBrush bSombra = new SolidBrush(sombraCol))
            {
                g.FillEllipse(bSombra, -35, -20, 70, 50);
                g.FillEllipse(bSombra, -10, -30, 60, 45);
            }

            // Base Principal
            using (SolidBrush bBase = new SolidBrush(baseCol))
            {
                g.FillEllipse(bBase, -30, -25, 65, 45);
                g.FillEllipse(bBase, -5, -35, 55, 40);
                g.FillEllipse(bBase, -20, -10, 50, 35);
            }

            // Brillos/Luz (parte superior)
            using (SolidBrush bLuz = new SolidBrush(luzCol))
            {
                g.FillEllipse(bLuz, -20, -32, 40, 20);
                g.FillEllipse(bLuz, 10, -38, 30, 15);
            }
        }

        private void DibujarPajaros(Graphics g)
        {
            foreach (var p in parvada)
            {
                float flap = (float) Math.Sin(faseAnimacion * 12) * 5;
                g
                    .DrawLines(new Pen(Color.Black, 2),
                    new PointF[] {
                        new PointF(p.Posicion.X - 8, p.Posicion.Y + flap),
                        new PointF(p.Posicion.X, p.Posicion.Y),
                        new PointF(p.Posicion.X + 8, p.Posicion.Y + flap)
                    });
            }
        }

        private void DibujarRayo(Graphics g)
        {
            if (flashRayo > 4 && puntosRayo.Count > 1)
            {
                using (Pen p = new Pen(Color.White, 3))
                {
                    g.DrawLines(p, puntosRayo.ToArray());
                    p.Color = Color.FromArgb(120, Color.LightBlue);
                    p.Width = 10;
                    g.DrawLines(p, puntosRayo.ToArray());
                }
            }
        }

        private void DibujarLluvia(Graphics g)
        {
            using (Pen p = new Pen(Color.FromArgb(140, Color.SkyBlue), 1))
                foreach (var gota in lluvia)
                g
                    .DrawLine(p,
                    gota.Posicion.X,
                    gota.Posicion.Y,
                    gota.Posicion.X + intensidadViento * 2,
                    gota.Posicion.Y + 12);
        }

        private void DibujarCieloSuave(Graphics g)
        {
            float h = minutosDia / 60f;
            Color
                t,
                b;
            if (estaLloviendo)
            {
                t = Color.FromArgb(60, 65, 80);
                b = Color.FromArgb(130, 135, 150);
            }
            else
            {
                if (h < 6)
                {
                    float f = h / 6f;
                    t =
                        InterpolateColor(Color.FromArgb(5, 5, 20),
                        Color.FromArgb(70, 90, 150),
                        f);
                    b =
                        InterpolateColor(Color.FromArgb(20, 20, 60),
                        Color.FromArgb(255, 160, 60),
                        f);
                }
                else if (h < 12)
                {
                    float f = (h - 6) / 6f;
                    t =
                        InterpolateColor(Color.FromArgb(70, 90, 150),
                        Color.FromArgb(100, 180, 240),
                        f);
                    b =
                        InterpolateColor(Color.FromArgb(255, 160, 60),
                        Color.FromArgb(200, 230, 255),
                        f);
                }
                else if (h < 18)
                {
                    float f = (h - 12) / 6f;
                    t =
                        InterpolateColor(Color.FromArgb(100, 180, 240),
                        Color.FromArgb(255, 100, 50),
                        f);
                    b =
                        InterpolateColor(Color.FromArgb(200, 230, 255),
                        Color.FromArgb(80, 30, 100),
                        f);
                }
                else
                {
                    float f = (h - 18) / 6f;
                    t =
                        InterpolateColor(Color.FromArgb(255, 100, 50),
                        Color.FromArgb(5, 5, 20),
                        f);
                    b =
                        InterpolateColor(Color.FromArgb(80, 30, 100),
                        Color.FromArgb(20, 20, 60),
                        f);
                }
            }
            using (
                LinearGradientBrush sky =
                    new LinearGradientBrush(this.ClientRectangle, t, b, 90f)
            )
                g.FillRectangle(sky, this.ClientRectangle);
        }

        private Color InterpolateColor(Color c1, Color c2, float f) =>
            Color
                .FromArgb((int)(c1.R + (c2.R - c1.R) * f),
                (int)(c1.G + (c2.G - c1.G) * f),
                (int)(c1.B + (c2.B - c1.B) * f));

        private void DibujarNubes(Graphics g)
        {
            Color cN =
                estaLloviendo
                    ? Color.FromArgb(160, 75, 75, 90)
                    : Color.FromArgb(195, Color.White);
            foreach (var n in nubes)
            {
                g.TranslateTransform(n.Posicion.X, n.Posicion.Y);
                using (SolidBrush b = new SolidBrush(cN))
                {
                    g.FillEllipse(b, 0, 0, 80 * n.Escala, 40 * n.Escala);
                    g.FillEllipse(b, 25, -15, 50 * n.Escala, 50 * n.Escala);
                }
                g.ResetTransform();
            }
        }

        private void DibujarSombras(Graphics g)
        {
            double ang = (minutosDia / 1440.0) * Math.PI * 2 - Math.PI;
            float
                lx = (float)(this.Width / 2 + 450 * Math.Cos(ang)),
                ly = (float)(450 + 350 * Math.Sin(ang));
            using (
                SolidBrush sb =
                    new SolidBrush(Color
                            .FromArgb(EsNoche() ? 20 : 50, Color.Black))
            )
            {
                GraphicsState s = g.Save();
                Matrix m = new Matrix();
                m.Translate(660, 520);
                m.Shear((660 - lx) / 350f, 0);
                m.Scale(1, 0.5f);
                m.Translate(-660, -520);
                g.MultiplyTransform (m);
                g.FillRectangle(sb, 600, 400, 120, 120);
                g.Restore (s);
                foreach (var p in posPollitos)
                g.FillEllipse(sb, p.X - 10 + (p.X - lx) / 30, p.Y + 10, 20, 7);
            }
        }

        private void DibujarLagoConOndas(Graphics g)
        {
            using (GraphicsPath p = GetLagoPath())
            {
                g.FillPath(Brushes.DodgerBlue, p);
                g.SetClip (p);
                foreach (var o in ondas)
                {
                    using (
                        Pen pen =
                            new Pen(Color.FromArgb(o.Opacidad, Color.White), 2)
                    )
                        g
                            .DrawEllipse(pen,
                            o.Posicion.X - o.Radio,
                            o.Posicion.Y - o.Radio / 2,
                            o.Radio * 2,
                            o.Radio);
                }
                g.ResetClip();
                g.DrawPath(Pens.AliceBlue, p);
            }
        }

        private void DibujarVegetacionConViento(Graphics g)
        {
            float fV =
                (float)
                Math.Sin(faseAnimacion * (estaLloviendo ? 2.5f : 1.2f)) *
                (5 * intensidadViento);
            foreach (var v in vegetacion)
            {
                g.TranslateTransform(v.Posicion.X, v.Posicion.Y);
                float inc =
                    fV + (float) Math.Cos(faseAnimacion + v.FaseViento) * 2;
                if (v.EsFlor)
                {
                    g.DrawLine(Pens.Green, 0, 0, inc, -10);
                    using (SolidBrush b = new SolidBrush(v.ColorPrincipal))
                        g.FillEllipse(b, inc - 4, -14, 8, 8);
                }
                else
                {
                    PointF[] p =
                    {
                        new PointF(0, 0),
                        new PointF(inc - 3, -13),
                        new PointF(inc + 2, -11),
                        new PointF(1, 0)
                    };
                    g.FillPolygon(Brushes.DarkGreen, p);
                }
                g.ResetTransform();
            }
        }

        private void DibujarGranero3D(Graphics g)
        {
            Point[] f =
            {
                new Point(600, 400),
                new Point(720, 400),
                new Point(720, 520),
                new Point(600, 520)
            };
            Point[] l =
            {
                new Point(720, 400),
                new Point(760, 370),
                new Point(760, 490),
                new Point(720, 520)
            };
            Point[] tF =
            { new Point(600, 400), new Point(720, 400), new Point(660, 330) };
            Point[] tL =
            {
                new Point(720, 400),
                new Point(760, 370),
                new Point(700, 300),
                new Point(660, 330)
            };
            g.FillPolygon(Brushes.Firebrick, f);
            g.FillPolygon(Brushes.Maroon, l);
            g.FillPolygon(Brushes.IndianRed, tF);
            g.FillPolygon(Brushes.DarkRed, tL);
            g.FillRectangle(Brushes.Black, 645, 465, 30, 55);
        }

        private void DibujarPetalos(Graphics g)
        {
            foreach (var p in petalos)
            {
                g.TranslateTransform(p.Posicion.X, p.Posicion.Y);
                g.RotateTransform(p.Rotacion);
                g.FillEllipse(Brushes.LightPink, -3, -2, 6, 4);
                g.ResetTransform();
            }
        }

        private void DibujarSolLuna(Graphics g)
        {
            double ang = (minutosDia / 1440.0) * Math.PI * 2 - Math.PI;
            int
                x = (int)(this.Width / 2 + 450 * Math.Cos(ang)),
                y = (int)(450 + 350 * Math.Sin(ang));

            float h = minutosDia / 60f; // Hora actual para el color del sol

            if (!EsNoche())
            {
                // --- Dibujar SOL con Volumen ---
                int radioSol = 35;

                // Definir colores base y de brillo según la hora (Atardecer vs Día)
                Color
                    colorCentro,
                    colorBorde;

                // Si es cerca del amanecer (6-8am) o atardecer (16-18pm), Sol más naranja
                if (h < 8 || h > 16)
                {
                    colorCentro = Color.FromArgb(255, 255, 200); // Blanco amarillento cálido
                    colorBorde = Color.FromArgb(255, 140, 0); // Naranja oscuro (DarkOrange)
                } // Mediodía, Sol amarillo brillante
                else
                {
                    colorCentro = Color.White; // Brillo puro central
                    colorBorde = Color.FromArgb(255, 215, 0); // Oro (Gold)
                }

                // Dibujar el círculo con degradado
                DibujarCirculoDegradado (
                    g,
                    x,
                    y,
                    radioSol,
                    colorCentro,
                    colorBorde
                );

                // Opcional: Añadir un ligero resplandor (Glow) externo
                DibujarResplandorExterno(g,
                x,
                y,
                radioSol + 15,
                Color.FromArgb(100, colorBorde));
            }
            else
            {
                // --- Dibujar LUNA con Volumen ---
                int radioLuna = 25;

                // Colores para una luna plateada con volumen
                Color colorCentroLuna = Color.White; // Brillo central
                Color colorBordeLuna = Color.FromArgb(200, 200, 220); // Gris azulado pálido

                DibujarCirculoDegradado (
                    g,
                    x,
                    y,
                    radioLuna,
                    colorCentroLuna,
                    colorBordeLuna
                );

                // Opcional: Resplandor frío externo
                DibujarResplandorExterno(g,
                x,
                y,
                radioLuna + 10,
                Color.FromArgb(50, Color.LightBlue));
            }
        }

        // --- Método para dibujar una esfera con degradado radial (volumen) ---
        private void DibujarCirculoDegradado(
            Graphics g,
            int x,
            int y,
            int radio,
            Color colorCentro,
            Color colorBorde
        )
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                // Definimos la trayectoria circular
                path.AddEllipse(x - radio, y - radio, radio * 2, radio * 2);

                using (PathGradientBrush pgb = new PathGradientBrush(path))
                {
                    // El color en el centro exacto del círculo
                    pgb.CenterColor = colorCentro;

                    // El color (o colores) en todo el borde de la trayectoria
                    pgb.SurroundColors = new Color[] { colorBorde };

                    // Opcional: Desplazar el centro del degradado ligeramente para simular
                    // que la luz viene de un lado (ej. arriba a la izquierda)
                    // pgb.CenterPoint = new PointF(x - radio/4, y - radio/4);
                    // Rellenar el círculo con el degradado
                    g.FillPath (pgb, path);
                }
            }
        }

        // --- Método opcional para un resplandor externo suave (Glow) ---
        private void DibujarResplandorExterno(
            Graphics g,
            int x,
            int y,
            int radioGlow,
            Color colorGlow
        )
        {
            using (GraphicsPath pathGlow = new GraphicsPath())
            {
                pathGlow
                    .AddEllipse(x - radioGlow,
                    y - radioGlow,
                    radioGlow * 2,
                    radioGlow * 2);

                using (
                    PathGradientBrush pgbGlow = new PathGradientBrush(pathGlow)
                )
                {
                    pgbGlow.CenterColor = colorGlow; // Color en el centro (con opacidad)
                    pgbGlow.SurroundColors = new Color[] { Color.Transparent }; // Se desvanece a transparente

                    g.FillPath (pgbGlow, pathGlow);
                }
            }
        }

        private void DibujarPollitosCaminantes(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < posPollitos.Count; i++)
            {
                PointF p = posPollitos[i];
                PointF target = targetPollitos[i];

                // Ya tienes calculada la dirección aquí:
                float direccion = (target.X < p.X) ? -1f : 1f;

                float balanceoY = (float) Math.Sin(animacionPollo + i) * 1.5f;

                GraphicsState state = g.Save();

                g.TranslateTransform(p.X, p.Y + balanceoY);

                // IMPORTANTE: Ya no necesitamos ScaleTransform para espejar todo el dibujo
                // porque ahora el método DibujarPollitoPro maneja la dirección internamente.
                // g.ScaleTransform(direccion, 1f); // <-- Puedes comentar o borrar esta línea
                // Pasamos los 3 parámetros: el Graphics, el índice y la dirección
                DibujarPollitoPro (g, i, direccion);

                g.Restore (state);
            }
        }

        private void DibujarPollitoPro(Graphics g, int index, float direccion)
        {
            // --- Paleta de Colores ---
            Color colorCuerpo = Color.FromArgb(255, 230, 0);
            Color colorCuerpoLuz = Color.FromArgb(255, 255, 150);
            Color colorCuerpoSombra = Color.FromArgb(220, 180, 0);
            Color colorPicoPatas = Color.FromArgb(255, 120, 0);
            Color colorPicoSombra = Color.FromArgb(200, 80, 0);

            float offSetAnim = index * 0.5f;
            float batidoAla =
                (float) Math.Sin(faseAnimacion * 12 + offSetAnim) * 10f;

            // Movimiento de patas
            float movPatas = (float) Math.Sin(faseAnimacion * 10 + index);
            float offsetPataAtras = movPatas * 3;
            float offsetPataAdelante = -movPatas * 3;

            // 1. DIBUJAR PATAS (Independientes del ScaleTransform del cuerpo)
            // Usamos 'direccion' para que el dedo siempre apunte hacia donde camina
            DibujarPataSola(g,
            new PointF(-3 + offsetPataAtras, 10),
            colorPicoPatas,
            direccion);
            DibujarPataSola(g,
            new PointF(3 + offsetPataAdelante, 11),
            colorPicoPatas,
            direccion);

            // --- APLICAR ROTACIÓN AL CUERPO ---
            GraphicsState cuerpoState = g.Save();
            g.ScaleTransform(direccion, 1f); // Esto voltea cuerpo, cabeza, ojo y pico

            // 2. CUERPO
            using (GraphicsPath pathCuerpo = new GraphicsPath())
            {
                pathCuerpo.AddEllipse(-12, -8, 24, 20);
                using (PathGradientBrush pgb = new PathGradientBrush(pathCuerpo)
                )
                {
                    pgb.CenterColor = colorCuerpoLuz;
                    pgb.SurroundColors = new Color[] { colorCuerpo };
                    pgb.CenterPoint = new PointF(2, -2);
                    g.FillPath (pgb, pathCuerpo);
                }
                g
                    .DrawEllipse(new Pen(colorCuerpoSombra, 0.5f),
                    -12,
                    -8,
                    24,
                    20);
            }

            // 3. CABEZA
            using (GraphicsPath pathCabeza = new GraphicsPath())
            {
                pathCabeza.AddEllipse(2, -14, 16, 16);
                using (PathGradientBrush pgb = new PathGradientBrush(pathCabeza)
                )
                {
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { colorCuerpo };
                    pgb.CenterPoint = new PointF(10, -10);
                    g.FillPath (pgb, pathCabeza);
                }
                g.DrawEllipse(new Pen(colorCuerpoSombra, 0.5f), 2, -14, 16, 16);
            }

            // 4. OJO (Ahora se voltea con el cuerpo)
            g.FillEllipse(Brushes.Black, 11, -10, 4, 5);
            g.FillEllipse(Brushes.White, 13, -9, 1.5f, 1.5f);

            // 5. PICO (Ahora se voltea con el cuerpo)
            PointF[] picoPts =
            { new PointF(16, -7), new PointF(23, -5), new PointF(16, -3) };
            using (
                LinearGradientBrush lgb =
                    new LinearGradientBrush(picoPts[0],
                        picoPts[1],
                        colorPicoPatas,
                        colorPicoSombra)
            )
                g.FillPolygon(lgb, picoPts);

            // 6. ALA
            GraphicsState alaState = g.Save();
            g.TranslateTransform(-2, -1);
            g.RotateTransform (batidoAla);
            g.FillEllipse(new SolidBrush(colorCuerpoLuz), -1, -1, 12, 7);
            g.DrawEllipse(Pens.DarkGoldenrod, -1, -1, 12, 7);
            g.Restore (alaState);

            g.Restore (cuerpoState); // Volvemos al estado normal
        }

        // --- NUEVO: Método auxiliar para dibujar UNA sola pata genérica en una posición ---
        private void DibujarPataSola(
            Graphics g,
            PointF posicion,
            Color color,
            float direccion
        )
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(posicion.X, posicion.Y);

            using (Pen pPata = new Pen(color, 2))
            {
                pPata.EndCap = LineCap.Round;

                // Dibujamos la pierna (vertical)
                g.DrawLine(pPata, 0, 0, 0, 8);

                // Dibujamos el dedo (forma de L).
                // La dirección X del dedo depende de la dirección del pollito
                float largoDedo = 4f * direccion; // El dedo apunta adelante
                g.DrawLine(pPata, 0, 8, largoDedo, 8);
            }

            g.Restore (state);
        }

        private void DibujarLuciernagas(Graphics g)
        {
            foreach (var l in luciernagas)
            {
                int b = (int)(Math.Abs(Math.Sin(l.FaseBrillo)) * 255);
                if (b < 60) continue;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddEllipse(l.Posicion.X - 5, l.Posicion.Y - 5, 10, 10);
                    using (PathGradientBrush pgb = new PathGradientBrush(gp))
                    {
                        pgb.CenterColor = Color.FromArgb(b, Color.Yellow);
                        pgb.SurroundColors = new Color[] { Color.Transparent };
                        g.FillPath (pgb, gp);
                    }
                }
            }
        }

        private void DibujarEstrellasFijas(Graphics g)
        {
            int a = (int)(Math.Abs(Math.Sin(faseAnimacion)) * 255);
            using (SolidBrush b = new SolidBrush(Color.FromArgb(a, Color.White))
            )
                foreach (var p in estrellasFijas)
                g.FillRectangle(b, p.X, p.Y, 2, 2);
        }

        private void DibujarEstrellasFugaces(Graphics g)
        {
            foreach (var ef in estrellasFugaces)
            {
                int a = (int)((ef.Vida / (float) ef.VidaMax) * 255);
                using (Pen p = new Pen(Color.FromArgb(a, Color.White), 2))
                    g
                        .DrawLine(p,
                        ef.Posicion.X,
                        ef.Posicion.Y,
                        ef.Posicion.X - ef.Velocidad.X * 2,
                        ef.Posicion.Y - ef.Velocidad.Y * 2);
            }
        }

        private void DibujarPastoBase(Graphics g) =>
            g.FillRectangle(Brushes.ForestGreen, 0, 400, this.Width, 300);

        private void DibujarCronometro(Graphics g)
        {
            int
                h = (int)(minutosDia / 60),
                m = (int)(minutosDia % 60);
            using (Font f = new Font("Consolas", 18, FontStyle.Bold))
                g.DrawString($"{h:D2}:{m:D2}", f, Brushes.White, 20, 20);
        }

        private bool EsNoche() => (minutosDia < 300 || minutosDia > 1260);
    }
}
