# Speech Based Object Manipulation

[![Unity](https://img.shields.io/badge/Unity-2022.3+-000000?style=flat&logo=unity)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![OpenAI](https://img.shields.io/badge/OpenAI-Whisper-412991?style=flat&logo=openai)](https://openai.com)
[![Python](https://img.shields.io/badge/Python-3.8+-3776AB?style=flat&logo=python)](https://python.org)

A Unity-based 3D application that enables **voice-controlled object manipulation** using OpenAI's Whisper speech recognition model for natural language processing.

## Features

- **Speech Recognition**: Real-time voice commands using OpenAI Whisper
- **3D Object Control**: Manipulate objects through natural language
- **Real-time Processing**: Instant response to voice commands
- **Interactive Environment**: 3D scene with physics-based interactions
- **Multi-language Support**: Powered by Whisper's multilingual capabilities

## Quick Start

### Prerequisites
- Unity 2022.3 or later
- Python 3.8+
- OpenAI API key (if using API version)
- Microphone access

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/haotianli24/object-manipulation.git
cd object-manipulation

# 2. Install Python dependencies
pip install openai-whisper
pip install pyaudio numpy

# 3. Open in Unity
# Open Unity Hub and add the project folder
```

### Setup

1. **Configure Audio Input**
   - Ensure microphone permissions are enabled
   - Test audio input in Unity's Audio settings

2. **Whisper Model Setup**
   - Models are automatically downloaded on first use
   - Smaller models (tiny/base) for faster processing
   - Larger models (medium/large) for better accuracy

## Usage Examples

### Voice Commands

```csharp
// Example voice commands the system recognizes:
"Move the cube to the right"
"Rotate the sphere"
"Scale the object up"
"Change color to red"
"Delete the cylinder"
"Create a new box"
```

### Script Integration

```csharp
public class VoiceController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject[] manipulableObjects;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRecording();
        }
    }
    
    private void ProcessVoiceCommand(string command)
    {
        // Parse command and execute corresponding action
        if (command.Contains("move") && command.Contains("cube"))
        {
            MoveCube(ExtractDirection(command));
        }
        else if (command.Contains("rotate"))
        {
            RotateObject(ExtractObject(command));
        }
    }
}
```

### Object Manipulation System

```csharp
public class ObjectManipulator : MonoBehaviour
{
    public void MoveObject(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
    
    public void RotateObject(Vector3 axis)
    {
        transform.Rotate(axis * rotationSpeed * Time.deltaTime);
    }
    
    public void ScaleObject(float scaleFactor)
    {
        transform.localScale *= scaleFactor;
    }
}
```

## Architecture

```
object-manipulation/
├── Assets/
│   ├── Scripts/
│   │   ├── VoiceController.cs      # Speech recognition handler
│   │   ├── ObjectManipulator.cs   # Object control logic
│   │   └── CommandParser.cs       # NLP command parsing
│   ├── Scenes/
│   │   └── MainScene.unity        # Main interaction scene
│   ├── Prefabs/
│   │   └── InteractableObjects/   # 3D object prefabs
│   └── StreamingAssets/
│       └── WhisperModels/         # AI model files
├── Python/
│   ├── whisper_server.py          # Python speech processing
│   └── requirements.txt           # Python dependencies
└── README.md
```

## Key Components

### Speech Processing Pipeline

```python
# Python backend for speech recognition
import whisper
import pyaudio

class SpeechProcessor:
    def __init__(self, model_size="base"):
        self.model = whisper.load_model(model_size)
        
    def transcribe_audio(self, audio_file):
        result = self.model.transcribe(audio_file)
        return result["text"]
        
    def process_command(self, text):
        # Parse natural language commands
        return self.parse_intent(text)
```

### Unity Integration

```csharp
public class WhisperIntegration : MonoBehaviour
{
    private Process pythonProcess;
    
    void Start()
    {
        StartPythonBackend();
    }
    
    private void StartPythonBackend()
    {
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = "python";
        pythonProcess.StartInfo.Arguments = "Python/whisper_server.py";
        pythonProcess.Start();
    }
}
```

## Command Recognition

| Command Type | Example | Action |
|-------------|---------|---------|
| Movement | "Move cube left" | Translate object |
| Rotation | "Rotate sphere" | Apply rotation |
| Scaling | "Make it bigger" | Scale transform |
| Color | "Turn it red" | Change material |
| Creation | "Create a cylinder" | Instantiate prefab |
| Deletion | "Delete the box" | Destroy object |

## Performance

- **Whisper Tiny**: ~32MB, ~32x realtime on CPU
- **Whisper Base**: ~74MB, ~16x realtime on CPU  
- **Whisper Small**: ~244MB, ~6x realtime on CPU
- **Average Response Time**: 200-500ms depending on model size

## Development Scripts

```bash
# Start the application
cd object-manipulation
python Python/whisper_server.py  # Start Python backend
# Open Unity and press Play

# Test voice recognition
python Python/test_microphone.py

# Install additional models
python -c "import whisper; whisper.load_model('medium')"
```

## Supported Commands

```csharp
// Movement commands
"Move [object] [direction]"
"Translate the [object] to [position]"

// Rotation commands  
"Rotate [object] [axis/direction]"
"Turn the [object] around"

// Scaling commands
"Scale [object] [up/down/factor]"
"Make the [object] [bigger/smaller]"

// Material commands
"Change [object] color to [color]"
"Make [object] [transparent/opaque]"
```

## Requirements

**Unity Package Dependencies:**
- Unity 2022.3 LTS or newer
- Audio system enabled
- Microphone permissions

**Python Dependencies:**
```txt
openai-whisper>=20230314
pyaudio>=0.2.11
numpy>=1.21.0
scipy>=1.7.0
```

---

**Built for intuitive 3D interaction through natural speech**
