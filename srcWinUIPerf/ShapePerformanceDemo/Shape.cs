using System;
using System.Drawing;

namespace ShapePerformanceDemo
{
    /// <summary>
    /// Represents a flying shape with position, velocity, and rendering.
    /// This code is intentionally not optimized to serve as a baseline for performance improvements.
    /// </summary>
    public class Shape
    {
        private static Random random = new Random();
        
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public Color Color { get; set; }
        public int Size { get; set; }

        public Shape(int canvasWidth, int canvasHeight)
        {
            // Initialize with random position and velocity
            X = random.Next(0, canvasWidth);
            Y = random.Next(0, canvasHeight);
            VelocityX = (float)(random.NextDouble() * 4 - 2); // Random velocity between -2 and 2
            VelocityY = (float)(random.NextDouble() * 4 - 2);
            
            // Random color
            Color = Color.FromArgb(255, random.Next(256), random.Next(256), random.Next(256));
            
            // Random size between 10 and 30 pixels
            Size = random.Next(10, 31);
        }

        /// <summary>
        /// Update shape position and handle bouncing off edges.
        /// Non-optimized: recalculating boundaries each frame.
        /// </summary>
        public void Update(int canvasWidth, int canvasHeight)
        {
            // Move the shape
            X += VelocityX;
            Y += VelocityY;

            // Bounce off left and right edges
            if (X < 0)
            {
                X = 0;
                VelocityX = Math.Abs(VelocityX); // Bounce right
            }
            else if (X + Size > canvasWidth)
            {
                X = canvasWidth - Size;
                VelocityX = -Math.Abs(VelocityX); // Bounce left
            }

            // Bounce off top and bottom edges
            if (Y < 0)
            {
                Y = 0;
                VelocityY = Math.Abs(VelocityY); // Bounce down
            }
            else if (Y + Size > canvasHeight)
            {
                Y = canvasHeight - Size;
                VelocityY = -Math.Abs(VelocityY); // Bounce up
            }
        }

        /// <summary>
        /// Draw the shape as a filled ellipse.
        /// Non-optimized: creating new brush each time.
        /// </summary>
        public void Draw(Graphics graphics)
        {
            // Creating a new brush each time is inefficient
            using (var brush = new SolidBrush(Color))
            {
                graphics.FillEllipse(brush, X, Y, Size, Size);
            }
        }
    }
}
