using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace SphereCoolingSimulation
{
    public partial class MainWindow : Window
    {
        private const double Radius = 3; // радиус шара в см
        private const double TemperatureCoefficient = 2 * 1e-7; // коэффициент температуропроводности в м^2/с
        private const double InitialTemperature = 1000; // начальная температура в градусах Цельсия
        private const double BoundaryTemperature = 200; // температура на границе в градусах Цельсия
        public const int NumSteps = 50; // число шагов по координате
        private const double dt = 0.9; // шаг по времени

        private double[,] _temperatureGrid;
        private int _currentStep;
        double time = 0;
        private Color _sphereColor;
        public Color SphereColor
        {
            get { return _sphereColor; }
            set
            {
                _sphereColor = value;
               
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _temperatureGrid = new double[NumSteps, NumSteps];

        }




        private void InitializeTemperature()
        {
            for (int i = 0; i < NumSteps; i++)
            {
                for (int j = 0; j < NumSteps; j++)
                {
                    _temperatureGrid[i, j] = InitialTemperature;
                }
            }
        }


        private void UpdateTemperature()
        {
            for (int i = 0; i < NumSteps; i++)
            {
                for (int j = 0; j < NumSteps; j++)
                {
                    double laplacian = Laplacian(_temperatureGrid, i, j);
                    _temperatureGrid[i, j] += TemperatureCoefficient * laplacian * dt;
                }
            }
        }

        private double Laplacian(double[,] grid, int i, int j)
        {
            double laplacian = 0;
            laplacian += (i > 0) ? grid[i - 1, j] - grid[i, j] : 0;
            laplacian += (i < NumSteps - 1) ? grid[i + 1, j] - grid[i, j] : 0;
            laplacian += (j > 0) ? grid[i, j - 1] - grid[i, j] : 0;
            laplacian += (j < NumSteps - 1) ? grid[i, j + 1] - grid[i, j] : 0;
            return laplacian / Math.Pow(1.0 / NumSteps, 2);
        }
        private void UpdateSphereColor()
        {
            SphereColor = CalculateColor(_temperatureGrid[NumSteps / 2, NumSteps / 2]);
        }
        private Color CalculateColor(double temperature)
        {
            byte r = (byte)(255 * (BoundaryTemperature - temperature) / (BoundaryTemperature - InitialTemperature));
            byte g = (byte)(255 * (BoundaryTemperature - temperature) / (BoundaryTemperature - InitialTemperature));
            byte b = (byte)(255 * (BoundaryTemperature - temperature) / (BoundaryTemperature - InitialTemperature));

            return Color.FromRgb(r, g, b);
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            time = 0;
            InitializeTemperature();
            CompositionTarget.Rendering += StartAnimation;
        }



        private void StartAnimation(object sender, EventArgs e)
        {
            UpdateTemperature();
            UpdateSphereColor();

            time += dt;
            _currentStep++;
            if (_currentStep >= NumSteps)
            {
                CompositionTarget.Rendering -= StartAnimation;
                MessageBox.Show("Остывание завершено.");
            }


        }

        /*private void btnReset_Click(object sender, RoutedEventArgs e)
        {

            PendulumInitialize();
            StopAnim();
        }*/

        private void StopAnim()
        {

            CompositionTarget.Rendering -= StartAnimation;

        }

        /*private void PendulumInitialize()
        {


            ball.Center = new Point(140, 150);
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            line1.X2 = 140;

            ball.Center = new Point(140, 150);
            StopAnim();
        }*/
    }
}