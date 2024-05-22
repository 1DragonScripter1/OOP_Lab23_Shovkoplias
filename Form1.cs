using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab23
{
    public partial class Form1 : Form
    {
        private double R = 1;
        private double tStart = -5;
        private double tEnd = 5;
        private double x0 = 0;
        private double y0 = 0;

        private new const int Margin = 40;
        private const int TickSpacing = 50; // Відстань між позначками на осях у пікселях

        private Label labelDomain;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ініціалізація компонентів
            this.Text = "Побудова графіка параметричної функці";
            this.Size = new Size(1000, 600);

            // Labels
            Label labelR = new Label { Text = "R:", Location = new Point(10, 10), Width = 100 };
            Label labelTStart = new Label { Text = "t (від):", Location = new Point(10, 40), Width = 100 };
            Label labelTEnd = new Label { Text = "t (до):", Location = new Point(10, 70), Width = 100 };
            Label labelX0 = new Label { Text = "x0:", Location = new Point(10, 100), Width = 100 };
            Label labelY0 = new Label { Text = "y0:", Location = new Point(10, 130), Width = 100 };

            // TextBoxes
            TextBox textBoxR = new TextBox { Location = new Point(120, 10), Width = 100, Text = R.ToString() };
            TextBox textBoxTStart = new TextBox { Location = new Point(120, 40), Width = 100, Text = tStart.ToString() };
            TextBox textBoxTEnd = new TextBox { Location = new Point(120, 70), Width = 100, Text = tEnd.ToString() };
            TextBox textBoxX0 = new TextBox { Location = new Point(120, 100), Width = 100, Text = x0.ToString() };
            TextBox textBoxY0 = new TextBox { Location = new Point(120, 130), Width = 100, Text = y0.ToString() };

            // Button
            Button buttonDraw = new Button { Text = "Побудувати графік", Location = new Point(10, 160), Width = 210 };
            buttonDraw.Click += (s, ev) =>
            {
                if (double.TryParse(textBoxR.Text, out R) &&
                    double.TryParse(textBoxTStart.Text, out tStart) &&
                    double.TryParse(textBoxTEnd.Text, out tEnd) &&
                    double.TryParse(textBoxX0.Text, out x0) &&
                    double.TryParse(textBoxY0.Text, out y0))
                {
                    pictureBoxGraph.Invalidate();
                    UpdateDomainLabel();
                }
                else
                {
                    MessageBox.Show("Введіть коректні значення параметів.");
                }
            };

            // PictureBox
            pictureBoxGraph = new PictureBox { Location = new Point(250, 10), Size = new Size(700, 550), BorderStyle = BorderStyle.FixedSingle };
            pictureBoxGraph.Paint += PictureBoxGraph_Paint;

            // Label для області визначення функцій
            labelDomain = new Label { Text = "", Location = new Point(10, 200), Width = 210 };

            // Додавання компонентів на форму
            this.Controls.Add(labelR);
            this.Controls.Add(labelTStart);
            this.Controls.Add(labelTEnd);
            this.Controls.Add(labelX0);  // Додаєму цю позначку
            this.Controls.Add(labelY0);  // Додаєму цю позначку
            this.Controls.Add(textBoxR);
            this.Controls.Add(textBoxTStart);
            this.Controls.Add(textBoxTEnd);
            this.Controls.Add(textBoxX0);
            this.Controls.Add(textBoxY0);
            this.Controls.Add(buttonDraw);
            this.Controls.Add(pictureBoxGraph);
            this.Controls.Add(labelDomain);
        }

        private void PictureBoxGraph_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);

            // Відображення системи координат
            DrawAxes(graphics);

            // Параметричні рівняння кривих
            double FunctionX(double t)
            {
                return x0 + R * t * Math.Cos(t);
            }
            double FunctionY(double t)
            {
                return y0 + R * t * Math.Sin(t);
            }

            // Масштабування графіка
            var scaleX = TickSpacing / 2.0;  // масштабування по x
            var scaleY = TickSpacing / 2.0; // масштабування по y

            // Зсув центру координат
            var centerX = (pictureBoxGraph.Width / 2.0 / TickSpacing) * TickSpacing;
            var centerY = (pictureBoxGraph.Height / 2.0 / TickSpacing) * TickSpacing;

            var pen = new Pen(Color.Red, 2);
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            // Генерація кривої
            bool firstPoint = true;

            for (double t = tStart; t <= tEnd; t += 0.01) // Збільшили кількості точок для плавності кривої
            {
                var x = FunctionX(t);
                var y = FunctionY(t);

                if (double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(x) || double.IsInfinity(y))
                {
                    continue;
                }

                x = x * scaleX + centerX;
                y = -y * scaleY + centerY;

                if (x < Margin || x > pictureBoxGraph.Width - Margin || y < Margin || y > pictureBoxGraph.Height - Margin)
                {
                    continue;
                }

                if (firstPoint)
                {
                    path.StartFigure();
                    path.AddLine((float)x, (float)y, (float)x, (float)y);
                    firstPoint = false;
                }
                else
                {
                    path.AddLine(path.GetLastPoint(), new PointF((float)x, (float)y));
                }
            }

            if (path.PointCount > 0)
            {
                graphics.DrawPath(pen, path);
            }
            else
            {
                MessageBox.Show("Не вдалося побудувати графік. Перевірте значення параметрів.");
            }

            // Відображення x0 і y0
            var font = new Font("Arial", 10);
            var brush = Brushes.Blue;
            var x0Screen = x0 * scaleX + centerX;
            var y0Screen = -y0 * scaleY + centerY;
            graphics.DrawString($"x0: {x0}", font, brush, (float)x0Screen + 5, (float)y0Screen - 20);
            graphics.DrawString($"y0: {y0}", font, brush, (float)x0Screen + 5, (float)y0Screen);

            // Рисування точки початку координат
            var pointBrush = Brushes.Blue;
            var pointRadius = 5;
            graphics.FillEllipse(pointBrush, (float)x0Screen - pointRadius, (float)y0Screen - pointRadius, pointRadius * 2, pointRadius * 2);
        }

        private void DrawAxes(Graphics graphics)
        {
            var centerX = (pictureBoxGraph.Width / 2.0 / TickSpacing) * TickSpacing;
            var centerY = (pictureBoxGraph.Height / 2.0 / TickSpacing) * TickSpacing;

            var penAxis = new Pen(Color.Black, 2);
            var font = new Font("Arial", 8);
            var brush = Brushes.Black;

            // Вертикальна вісь
            graphics.DrawLine(penAxis, (float)centerX, Margin, (float)centerX, pictureBoxGraph.Height - Margin);
            // Горизонтальна вісь
            graphics.DrawLine(penAxis, Margin, (float)centerY, pictureBoxGraph.Width - Margin, (float)centerY);

            // Поділки на осях
            for (int i = Margin; i <= pictureBoxGraph.Width - Margin; i += TickSpacing)
            {
                if (i != centerX)
                {
                    int xVal = (int)((i - centerX) / TickSpacing * 2);
                    graphics.DrawString(xVal.ToString(), font, brush, i - 10, (float)centerY + 5);
                }
            }

            for (int i = Margin; i <= pictureBoxGraph.Height - Margin; i += TickSpacing)
            {
                if (i != centerY)
                {
                    int yVal = (int)((centerY - i) / TickSpacing * 2);
                    graphics.DrawString(yVal.ToString(), font, brush, (float)centerX + 5, i - 10);
                }
            }

            // Підписи осей
            var axisFont = new Font("Arial", 10);
            graphics.DrawString("X", axisFont, brush, pictureBoxGraph.Width - Margin + 10, (float)centerY - 20);
            graphics.DrawString("Y", axisFont, brush, (float)centerX + 10, Margin - 20);
        }

        private void UpdateDomainLabel()
        {
            labelDomain.Text = $"Область визначення функцій: \nвід {tStart} до {tEnd}";
        }

        private PictureBox pictureBoxGraph;
    }
}