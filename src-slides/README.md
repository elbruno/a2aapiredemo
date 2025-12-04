# PowerPoint Generator for .NET Agentic Modernization Demo

This folder contains a Python script that generates a PowerPoint presentation from the slide content and speaker notes in the `docs/05_slide-content-and-speaker-notes.md` file.

## Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

## Installation

1. Create a virtual environment (recommended):

   **On macOS/Linux:**

   ```bash
   python -m venv venv
   source venv/bin/activate
   ```

   **On Windows:**

   ```cmd
   python -m venv venv
   venv\Scripts\activate
   ```

2. Install dependencies:

   ```bash
   pip install -r requirements.txt
   ```

## Usage

### Basic Usage

Generate a presentation with default styling:

```bash
python create_presentation.py
```

This will create `presentation.pptx` in the current directory.

### Using a Template

To apply styling from an existing PowerPoint template:

```bash
python create_presentation.py --template path/to/your/template.pptx
```

The script will use the template's styling (fonts, colors, layouts) while replacing the content with the generated slides.

### Custom Output Path

Specify where to save the generated presentation:

```bash
python create_presentation.py --output my_presentation.pptx
```

### Custom Markdown Source

Use a different markdown file as the source:

```bash
python create_presentation.py --markdown path/to/slides.md
```

### All Options Combined

```bash
python create_presentation.py --template company_template.pptx --output conference_talk.pptx --markdown custom_slides.md
```

## Output

The generated presentation includes:

- **14 slides** covering the .NET Agentic Modernization demo session
- **Speaker notes** for each slide (visible in presenter view)
- **Code blocks** with proper formatting for demo slides
- **Bullet points** with consistent styling

## Customization

### Template Requirements

When using a template, the script will:

1. Load the template file
2. Clear existing slides
3. Generate new slides using the template's master layouts and styling
4. Apply fonts and colors from the template

For best results, your template should have:

- A blank slide layout (layout index 6)
- Consistent fonts and color scheme

### Modifying Slide Content

To change the presentation content, edit the markdown file at:
`docs/05_slide-content-and-speaker-notes.md`

The script parses the following elements:

- `## Slide N: Title` - Slide headers
- `**Title**:` - Slide titles
- `**Key Points**:` - Bullet points
- `### Speaker Notes` - Notes for presenter view
- Code blocks in markdown format

## Troubleshooting

### Import Error for python-pptx

If you see `ImportError: No module named 'pptx'`:

```bash
pip install python-pptx
```

### Template Not Found

If the template path is invalid, the script will fall back to default styling and continue generating the presentation.

### Markdown File Not Found

Ensure the markdown file exists at the expected path. By default, the script looks for:
`../docs/05_slide-content-and-speaker-notes.md` (relative to the script location)

## License

This script is part of the a2aapiredemo repository and follows the repository's license.
