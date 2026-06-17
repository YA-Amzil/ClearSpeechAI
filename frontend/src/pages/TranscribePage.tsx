import { useState } from "react";
import styles from "./TranscribePage.module.css";

import DropZone from "../components/DropZone";
import FileInfo from "../components/FileInfo";
import TranscribeForm from "../components/TranscribeForm";
import StatusBar from "../components/StatusBar";
import TranscriptionResult from "../components/TranscriptionResult";
import YouTubeInput from "../components/YouTubeInput";
import MicrophoneRecorder from "../components/MicrophoneRecorder";

import { useTranscription } from "../hooks/useTranscription";
import { useRecorder } from "../hooks/useRecorder";
import { TranscriptionOptions } from "../services/transcriptionService";

const DEFAULT_OPTIONS: TranscriptionOptions = {
  language: "en",
  responseFormat: "json",
  temperature: 0,
  prompt: undefined,
};

export default function TranscribePage() {
  const [file, setFile] = useState<File | null>(null);
  const [youtubeUrl, setYoutubeUrl] = useState("");
  const [activeTab, setActiveTab] = useState<"file" | "youtube" | "record">("file",);
  const [options, setOptions] = useState(DEFAULT_OPTIONS);

  const { state, transcribeFile, transcribeYoutubeUrl, transcribeAudioBlob, reset } = useTranscription();
  const recorder = useRecorder();

  const isRunning = state.status === "uploading" || state.status === "processing";

  function switchToFile() {
    if (isRunning) return;
    setActiveTab("file");
    setYoutubeUrl("");
    reset();
  }

  function switchToYouTube() {
    if (isRunning) return;
    setActiveTab("youtube");
    setFile(null);
    reset();
  }

  function switchToRecord() {
    if (isRunning) return;
    setActiveTab("record");
    setFile(null);
    setYoutubeUrl("");
    reset();
  }

  function handleFile(f: File) {
    if (isRunning) return;
    setFile(f);
    setYoutubeUrl("");
    reset();
  }

  function handleClear() {
    if (isRunning) return;
    setFile(null);
    reset();
  }

  async function handleSubmit() {
    if (isRunning) return;

    reset();

    if (activeTab === "file" && file) {
      await transcribeFile(file, options);
      return;
    }

    if (activeTab === "youtube" && youtubeUrl.trim()) {
      await transcribeYoutubeUrl(youtubeUrl, options);
      return;
    }

    if (activeTab === "record" && recorder.recordedBlob) {
      await transcribeAudioBlob(recorder.recordedBlob, options);
      return;
    }
  }

  return (
    <main className={styles.main}>
      <div className={styles.container}>
        {/* Header */}
        <header className={styles.header}>
          <div className={styles.headerRow}>
            <svg
              className={styles.headerIcon}
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
            >
              <path d="M12 14a3 3 0 0 0 3-3V6a3 3 0 0 0-6 0v5a3 3 0 0 0 3 3z" />
              <path d="M19 11a1 1 0 1 0-2 0 5 5 0 0 1-10 0 1 1 0 1 0-2 0 7 7 0 0 0 6 6.92V21H9a1 1 0 1 0 0 2h6a1 1 0 1 0 0-2h-2v-3.08A7 7 0 0 0 19 11z" />
            </svg>
            <h1 className={styles.title}>Instant Transcription</h1>
          </div>
          <p className={styles.sub}>
            Upload any audio, video or YouTube link to get a transcript
            instantly.
          </p>
        </header>

        {/* Tabs */}
        <div className={styles.tabs}>
          {/* FILE TAB */}
          <button
            className={`${styles.tab} ${styles.firstTab} ${activeTab === "file" ? styles.active : ""}`}
            onClick={switchToFile}
            disabled={isRunning}
          >
            <svg
              className={styles.tabIcon}
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
              <polyline points="14 2 14 8 20 8" />
            </svg>
            File
          </button>

          {/* YOUTUBE TAB */}
          <button
            className={`${styles.tab} ${activeTab === "youtube" ? styles.active : ""}`}
            onClick={switchToYouTube}
            disabled={isRunning}
          >
            <svg
              className={styles.tabIcon}
              viewBox="0 0 24 24"
              fill="currentColor"
            >
              <path d="M21.6 7.2c-.2-1-.8-1.8-1.6-2.1C18 4.5 12 4.5 12 4.5s-6 0-8 .6c-.8.3-1.4 1.1-1.6 2.1C2 9 2 12 2 12s0 3 .4 4.8c.2 1 .8 1.8 1.6 2.1 2 .6 8 .6 8 .6s6 0 8-.6c.8-.3 1.4-1.1 1.6-2.1.4-1.8.4-4.8.4-4.8s0-3-.4-4.8z" />
              <path d="M10 15l5-3-5-3v6z" fill="white" />
            </svg>
            YouTube
          </button>

          {/* RECORD TAB */}
          <button
            className={`${styles.tab} ${styles.lastTab} ${activeTab === "record" ? styles.active : ""}`}
            onClick={switchToRecord}
            disabled={isRunning}
          >
            <svg
              className={styles.tabIcon}
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M12 14a3 3 0 0 0 3-3V7a3 3 0 0 0-6 0v4a3 3 0 0 0 3 3z" />
              <path d="M19 11a7 7 0 0 1-14 0" />
              <path d="M12 18v4" />
              <path d="M8 22h8" />
            </svg>
            Record
          </button>
        </div>

        {/* Active tab content */}
        <section className={styles.card}>
          {activeTab === "file" &&
            (file ? (
              <FileInfo file={file} onClear={handleClear} />
            ) : (
              <DropZone onFile={handleFile} />
            ))}

          {activeTab === "youtube" && (
            <YouTubeInput url={youtubeUrl} onChange={setYoutubeUrl} />
          )}

          {activeTab === "record" && (
              <MicrophoneRecorder
                startRecording={recorder.startRecording}
                stopRecording={recorder.stopRecording}
                isRecording={recorder.isRecording}
                isListening={recorder.isListening}
                levels={recorder.levels}
              />
          )}
        </section>

        {/* Settings */}
        <section className={styles.card}>
          <TranscribeForm
            options={options}
            onChange={setOptions}
            onSubmit={handleSubmit}
            disabled={
              isRunning ||
              (activeTab === "file" && !file) ||
              (activeTab === "youtube" && !youtubeUrl.trim()) ||
              (activeTab === "record" && !recorder.recordedBlob)
            }
          />
        </section>

        {/* Error */}
        {state.status === "error" && state.error && (
          <div className={styles.errorBox}>{state.error}</div>
        )}

        {/* Status */}
        <StatusBar status={state.status} />

        {/* Result */}
        {state.result && (
          <TranscriptionResult
            result={state.result}
            elapsedSeconds={state.elapsedSeconds}
          />
        )}
      </div>
    </main>
  );
}
