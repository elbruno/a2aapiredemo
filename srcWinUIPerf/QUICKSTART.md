# Quick Start Guide - Shape Performance Demo

## What is This?

A WinForms application that shows how performance decreases as you add more animated shapes. Perfect for demonstrating performance optimization techniques!

## Running the Application

### Option 1: Using dotnet CLI
```bash
cd srcWinUIPerf/ShapePerformanceDemo
dotnet run
```

### Option 2: Build and Run Executable
```bash
cd srcWinUIPerf/ShapePerformanceDemo
dotnet build
./bin/Debug/net9.0-windows/ShapePerformanceDemo.exe
```

### Option 3: Open in Visual Studio
1. Open `ShapePerformanceDemo.csproj` in Visual Studio
2. Press F5 to run

## What You'll See

When the application starts, you'll see:

```
┌─────────────────────────────────────────────────┐
│                                                 │
│  [Black canvas with colorful bouncing shapes]   │
│                                                 │
│                                                 │
│                                                 │
└─────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────┐
│ FPS: 60.0              Use slider or            │
│ Shapes: 50             CTRL+Mouse Wheel    [Slider]│
└─────────────────────────────────────────────────┘
```

## How to Use

### Adding More Shapes
1. **Drag the slider** at the bottom to change shape count (1-1000)
2. **Use CTRL+Mouse Wheel** for fine control:
   - Hold CTRL and scroll up = +10 shapes
   - Hold CTRL and scroll down = -10 shapes

### Observing Performance
- Start with 50 shapes (default)
- Watch the FPS counter (green text, top-left)
- Gradually increase shapes and watch FPS drop
- Try 100, 200, 500, 1000 shapes to see the impact

## Expected Performance

| Shape Count | Expected FPS | Notes |
|------------|--------------|-------|
| 1-50       | ~60 FPS      | Smooth animation |
| 100-200    | 30-45 FPS    | Slight slowdown |
| 300-500    | 15-30 FPS    | Noticeable lag |
| 500-1000   | <15 FPS      | Severe slowdown |

*Actual performance varies by hardware*

## What to Notice

### Visual Effects
- Shapes have **random colors** (assigned at creation)
- Shapes have **random sizes** (10-30 pixels)
- Shapes **bounce realistically** off window edges
- Each shape moves independently

### Performance Metrics
- **FPS updates every second** (not every frame)
- **Shape count** is always accurate
- **Lower FPS** = more shapes or slower hardware

## Why Is It Slow?

This code is **intentionally not optimized**. It demonstrates:
- Creating new objects every frame
- Sequential processing (not parallel)
- No rendering optimizations
- Full scene redraws

This makes it perfect for showing "before and after" optimization!

## Common Questions

**Q: Can I resize the window?**
A: Yes! Shapes will adjust to the new bounds.

**Q: What happens at 1000 shapes?**
A: It will be very slow but won't crash. That's the point!

**Q: Why is my FPS different than expected?**
A: Performance depends on your CPU, GPU, and screen resolution.

**Q: Can I make it faster?**
A: Yes! That's the point. This is the baseline for optimization.

## Keyboard Shortcuts

- **CTRL + Mouse Wheel Up**: Add 10 shapes
- **CTRL + Mouse Wheel Down**: Remove 10 shapes

## Troubleshooting

### Application won't start
- Ensure .NET 9 SDK is installed: `dotnet --version`
- Verify you're on Windows (WinForms requires Windows)

### Build errors
```bash
cd srcWinUIPerf/ShapePerformanceDemo
dotnet clean
dotnet restore
dotnet build
```

### Performance issues even with few shapes
- Close other applications
- Try reducing shape count to 10-20
- Check your system isn't under heavy load

## Next Steps

After seeing the baseline performance:
1. Document your observations
2. Identify bottlenecks (brush creation, sequential loops, etc.)
3. Apply optimizations
4. Compare before/after performance

## Technical Notes

- Built with **.NET 9**
- Uses **System.Drawing** for rendering
- Animation runs at **16ms per frame** (~60 FPS target)
- Double buffering enabled to reduce flicker
- No hardware acceleration used

## For Developers

See `IMPLEMENTATION.md` for detailed technical documentation about:
- Architecture decisions
- Non-optimized patterns used
- Optimization opportunities
- Code structure

## Support

For questions or issues, refer to:
- `README.md` - Full documentation
- `IMPLEMENTATION.md` - Technical details
- The source code (well-commented)
