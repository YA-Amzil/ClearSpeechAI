# ClearSpeechAI

[![Status](https://img.shields.io/badge/status-under%20development-yellow)]()
[![.NET](https://img.shields.io/badge/.NET-10.0%2B-blue)]()
[![License](https://img.shields.io/badge/license-MIT-green)]()

ClearSpeechAI is a audio-to-text transcription app. Convert speech to text using either **OpenAI's Whisper API** or **Local AI models** (via Docker).

---

## 🎯 Project Overview

ClearSpeechAI is a hands-on learning project exploring:

- 🎤 Audio-to-text transcription workflows
- 🧠 Microsoft Semantic Kernel integration
- 🌍 Multi-language NLP processing

Perfect for developers wanting to learn AI/ML integration in production-grade applications.

---

## 📋 Prerequisites

### Required

- **.NET 10 SDK** or higher ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Node.js 18+** ([Download](https://nodejs.org))
- **npm** or **yarn** (comes with Node.js)

### Optional (for local transcription)

- **Docker & Docker Compose** ([Download](https://www.docker.com/products/docker-desktop))
- **GPU Support**: NVIDIA CUDA toolkit (for faster local processing)

---

## 🚀 Quick Start

### 1️⃣ Clone & Setup (5 minutes)

```bash
git clone https://github.com/YA-Amzil/ClearSpeechAI
cd ClearSpeechAI
```

### 2️⃣ Choose Your Mode

#### **Option A: Using OpenAI (Cloud)**

Create:

```
backend/ClearSpeechAI.API/.env
```

Add:

```env
OpenAI__ApiKey=sk-YOUR_OPENAI_API_KEY_HERE      # e.g. sk-1234567890abcdef
OpenAI__AudioToTextModel=YOUR_OPENAI_API_MODEL  # e.g. Systran/faster-whisper-tiny
```

---

#### **Option B: Using Local Whisper (Docker)**

Create:

```
backend/ClearSpeechAI.API/.env
```

Add:

```env
OpenAI__BaseUrl=YOUR_BASE_URL   # e.g. http://localhost:8080
```

### 3️⃣ Run the Application

#### 🐳 Terminal 1 — Local Whisper (optional)

```bash
cd backend/docker
docker compose up -d
```

#### 🖥️ Terminal 2 — Backend API
```bash
cd backend
dotnet run --project ClearSpeechAI.API
```

#### 🌐 Terminal 3: Frontend (React)
```bash
cd frontend
npm install
npm run dev
```
Visit:

- 🌐 Frontend: `http://localhost:5173`
- 📚 API Docs: `http://localhost:5000/swagger`

---

## ⚙️ Configuration Guide

### Local Whisper Model Options

```yaml
environment:
  - WHISPER__MODEL=Systran/faster-whisper-tiny
  - WHISPER__MODEL=Systran/faster-whisper-small
  - WHISPER__MODEL=Systran/faster-whisper-medium
```

### Model Comparison

| Mode               | When to Use                      | Cost        | Speed        | Accuracy  |
| ------------------ | -------------------------------- | ----------- | ------------ | --------- |
| **Cloud (OpenAI)** | Production, highest accuracy     | Per‑request | ⚡ Fast      | ⭐⭐⭐⭐⭐ |
| **Local Tiny**     | Dev, fast iteration              | Free        | ⚡⚡ Very Fast | ⭐⭐ |
| **Local Small**    | Balanced                         | Free        | ⚡ Fast      | ⭐⭐⭐ |
| **Local Medium**   | High accuracy locally            | Free        | 🐢 Slow      | ⭐⭐⭐⭐ |

---

## 🔌 API Reference

### Transcribe Audio

**Endpoint:** `POST /api/transcription/transcribe`

**Request (multipart/form-data):**

```
audioFile          [file]    Audio file (mp3, wav, m4a, etc.)
language           [text]    Language code (e.g., "en", "nl", "es") - optional
responseFormat     [text]    Format: "json" (default), "text", "srt", "vtt"
temperature        [number]  0.0-1.0 (default: 0.0)
prompt             [text]    Hints for specific terms - optional
```

**Response (JSON):**

```json
{
  "success": true,
  "text": "Hello world, this is a transcription.",
  "language": "en",
  "format": "json"
}
```

**Error Response:**

```json
{
  "success": false,
  "error": "The request was canceled due to timeout"
}
```

**Example with curl:**

```bash
curl -X POST \
  -H "Content-Type: multipart/form-data" \
  -F "audioFile=@audio.mp3" \
  -F "language=en" \
  -F "responseFormat=json" \
  http://localhost:5000/api/transcription/transcribe
```

---

## 🛠️ Development Workflow

### Logging

Logs are configured via Serilog and appear in:

- Console output
- `backend/ClearSpeechAI.API/logs/` directory

---

## 🚢 Deployment

### Docker Deployment (Recommended)

```bash
# Build images
docker compose build

# Start services
docker compose up -d

# View logs
docker compose logs -f

# Stop services
docker compose down
```

## 🤝 Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit changes: `git commit -m "Add your feature"`
4. Push to branch: `git push origin feature/your-feature`
5. Submit a Pull Request

---


## Documentation

- [Semantic Kernel Docs](https://learn.microsoft.com/en-us/semantic-kernel/)
- [OpenAI Whisper API](https://platform.openai.com/docs/guides/speech-to-text)
- [Faster-Whisper](https://github.com/SYSTRAN/faster-whisper)


---

## 📜 License

This project is intended for learning and experimentation purposes.

---




