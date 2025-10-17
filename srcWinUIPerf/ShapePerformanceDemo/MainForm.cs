using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShapePerformanceDemo
{
    /// <summary>
    /// Main form that displays flying shapes with performance metrics.
    /// This code is intentionally not optimized to serve as a baseline for performance improvements.
    /// </summary>
    public partial class MainForm : Form
    {
        private List<Shape> shapes = null!;
        private System.Windows.Forms.Timer animationTimer = null!;
        private Stopwatch fpsStopwatch = null!;
        private int frameCount;
        private double currentFPS;
        private TrackBar shapeCountSlider = null!;
        private Label fpsLabel = null!;
        private Label shapeCountLabel = null!;
        private Label performanceLabel = null!;
        private Panel drawingPanel = null!;
        
        private const int MIN_SHAPES = 1;
        private const int MAX_SHAPES = 1000;
        private const int DEFAULT_SHAPES = 50;

        public MainForm()
        {
            InitializeComponent();
            InitializeShapes();
            InitializeTimers();
        }

        private void InitializeComponent()
        {
            // Set up the form
            this.Text = "Shape Performance Demo - Non-Optimized Baseline";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true; // Enable double buffering for the form
            
            // Create main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));

            // Create drawing panel
            drawingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            drawingPanel.Paint += DrawingPanel_Paint;
            
            // Create controls panel
            Panel controlsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            // FPS Label
            fpsLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Text = "FPS: 0"
            };
            controlsPanel.Controls.Add(fpsLabel);

            // Shape Count Label
            shapeCountLabel = new Label
            {
                Location = new Point(10, 35),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                Text = $"Shapes: {DEFAULT_SHAPES}"
            };
            controlsPanel.Controls.Add(shapeCountLabel);

            // Performance Info Label
            performanceLabel = new Label
            {
                Location = new Point(220, 10),
                Size = new Size(500, 50),
                ForeColor = Color.Yellow,
                Font = new Font("Consolas", 9),
                Text = "Use slider or CTRL+Mouse Wheel to change shape count\nNon-optimized baseline code for performance testing"
            };
            controlsPanel.Controls.Add(performanceLabel);

            // Slider for shape count
            Label sliderLabel = new Label
            {
                Location = new Point(730, 10),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Text = "Shape Count:"
            };
            controlsPanel.Controls.Add(sliderLabel);

            shapeCountSlider = new TrackBar
            {
                Location = new Point(730, 30),
                Size = new Size(250, 45),
                Minimum = MIN_SHAPES,
                Maximum = MAX_SHAPES,
                Value = DEFAULT_SHAPES,
                TickFrequency = 100,
                LargeChange = 100,
                SmallChange = 10
            };
            shapeCountSlider.ValueChanged += ShapeCountSlider_ValueChanged;
            controlsPanel.Controls.Add(sliderLabel);
            controlsPanel.Controls.Add(shapeCountSlider);

            // Add panels to main layout
            mainLayout.Controls.Add(drawingPanel, 0, 0);
            mainLayout.Controls.Add(controlsPanel, 0, 1);

            this.Controls.Add(mainLayout);

            // Enable mouse wheel handling for CTRL+Wheel
            drawingPanel.MouseWheel += DrawingPanel_MouseWheel;
            this.MouseWheel += DrawingPanel_MouseWheel;
        }

        private void InitializeShapes()
        {
            shapes = new List<Shape>();
            for (int i = 0; i < DEFAULT_SHAPES; i++)
            {
                shapes.Add(new Shape(drawingPanel.Width, drawingPanel.Height));
            }
        }

        private void InitializeTimers()
        {
            // Animation timer - updates at ~60 FPS
            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 16 // ~60 FPS
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // FPS calculation
            fpsStopwatch = Stopwatch.StartNew();
            frameCount = 0;
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            // Update all shapes - non-optimized loop
            foreach (var shape in shapes)
            {
                shape.Update(drawingPanel.Width, drawingPanel.Height);
            }

            // Invalidate the drawing panel to trigger repaint
            drawingPanel.Invalidate();
        }

        private void DrawingPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            // Non-optimized: not setting rendering hints for performance baseline
            // This makes rendering slower but easier to optimize later
            
            // Draw all shapes
            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }

            // Calculate FPS
            frameCount++;
            if (fpsStopwatch.ElapsedMilliseconds >= 1000)
            {
                currentFPS = frameCount / (fpsStopwatch.ElapsedMilliseconds / 1000.0);
                fpsLabel.Text = $"FPS: {currentFPS:F1}";
                frameCount = 0;
                fpsStopwatch.Restart();
            }
        }

        private void ShapeCountSlider_ValueChanged(object? sender, EventArgs e)
        {
            UpdateShapeCount(shapeCountSlider.Value);
        }

        private void DrawingPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            // Only process if CTRL is pressed
            if (ModifierKeys == Keys.Control)
            {
                int delta = e.Delta > 0 ? 10 : -10;
                int newValue = shapeCountSlider.Value + delta;
                
                // Clamp to valid range
                newValue = Math.Max(MIN_SHAPES, Math.Min(MAX_SHAPES, newValue));
                
                shapeCountSlider.Value = newValue;
            }
        }

        private void UpdateShapeCount(int newCount)
        {
            int currentCount = shapes.Count;
            
            if (newCount > currentCount)
            {
                // Add shapes
                for (int i = currentCount; i < newCount; i++)
                {
                    shapes.Add(new Shape(drawingPanel.Width, drawingPanel.Height));
                }
            }
            else if (newCount < currentCount)
            {
                // Remove shapes
                shapes.RemoveRange(newCount, currentCount - newCount);
            }

            shapeCountLabel.Text = $"Shapes: {shapes.Count}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
