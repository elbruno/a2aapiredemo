# Shape Performance Demo - WinForms Application

## Overview
This is a .NET 9 WinForms application designed to demonstrate performance characteristics with increasing numbers of animated shapes. The application intentionally uses **non-optimized code** to serve as a baseline for future performance improvements.

## Purpose
The goal of this application is to show how adding more shapes affects the frame rate (FPS). This baseline implementation can be analyzed and optimized later to demonstrate performance improvement techniques.

## Features

### Visual Display
- **Flying Shapes**: Colorful circular shapes that bounce off the edges of the window
- **FPS Counter**: Real-time display of current frames per second (in green)
- **Shape Counter**: Shows the current number of active shapes
- **Performance Information**: Displays helpful usage instructions

### Controls
1. **Slider Control**: Adjust the number of shapes from 1 to 1000
2. **CTRL + Mouse Wheel**: Fine-tune the shape count by scrolling while holding the CTRL key
   - Scroll up: Increase shape count by 10
   - Scroll down: Decrease shape count by 10

### Performance Metrics
The application displays:
- **Current FPS**: Updated every second
- **Shape Count**: Number of shapes currently being animated
- **Instructions**: Usage guidance displayed at the bottom

## Technical Details

### Non-Optimized Code
This application intentionally uses standard, non-optimized rendering techniques:
- Creates new brush objects for each shape on every frame
- Recalculates boundaries for each shape update
- Uses standard Graphics rendering without optimization hints
- Processes all shapes sequentially without batching

These inefficiencies make it easier to identify performance bottlenecks and demonstrate optimization techniques later.

### Architecture
- **Shape.cs**: Defines individual shapes with position, velocity, color, and rendering logic
- **MainForm.cs**: Main application form with animation loop and UI controls
- **Program.cs**: Application entry point

### Requirements
- .NET 9.0 SDK
- Windows OS (or Windows targeting enabled on cross-platform builds)
- System.Windows.Forms support

## Building and Running

### Build
```bash
cd srcWinUIPerf/ShapePerformanceDemo
dotnet build
```

### Run
```bash
cd srcWinUIPerf/ShapePerformanceDemo
dotnet run
```

Or execute the compiled binary:
```bash
./bin/Debug/net9.0-windows/ShapePerformanceDemo.exe
```

## Performance Expectations
- **1-50 shapes**: Should maintain 60 FPS on most systems
- **100-200 shapes**: FPS may drop to 30-45 FPS depending on hardware
- **500+ shapes**: Significant performance degradation expected (10-20 FPS)
- **1000 shapes**: Baseline performance will be quite slow (< 10 FPS)

These numbers will vary based on system specifications and screen resolution.

## Future Optimization Opportunities
This baseline code can be improved through:
1. Object pooling (reusing brush objects)
2. Batch rendering techniques
3. Graphics rendering optimizations
4. Parallel processing of shape updates
5. Hardware acceleration
6. Spatial partitioning for collision detection

## Notes
- The application uses double buffering to reduce flicker
- Shape colors and sizes are randomized on creation
- Shapes bounce realistically off window edges
- The animation runs at approximately 60 FPS target (16ms per frame)
