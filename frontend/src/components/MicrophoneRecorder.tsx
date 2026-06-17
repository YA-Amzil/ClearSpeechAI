import { useEffect } from "react";
import styles from "./MicrophoneRecorder.module.css";

interface Props {
  startRecording: () => void;
  stopRecording: () => void;
  isRecording: boolean;
  isListening: boolean;
  levels: number[];
  recordedBlob?: Blob | null;
}

export default function MicrophoneRecorder({
  startRecording,
  stopRecording,
  isRecording,
  isListening,
  levels,
  recordedBlob,
}: Props) {
  useEffect(() => {
    const handleKey = (e: KeyboardEvent) => {
      if (e.code === "Space") {
        e.preventDefault();
        isRecording ? stopRecording() : startRecording();
      }
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [isRecording, startRecording, stopRecording]);

  let headerText = "Record Audio";
  if (isListening) headerText = "Listening...";
  if (!isRecording && recordedBlob) headerText = "Recording done";

  let stateClass = styles.idle;
  if (isListening) stateClass = styles.listening;
  if (isRecording) stateClass = styles.recording;

  return (
    <div className={styles.wrapper}>
      <div className={styles.centerBox}>
        <div className={styles.recordHeader}>{headerText}</div>

        <button
          className={`${styles.btn} ${stateClass}`}
          onClick={isRecording ? stopRecording : startRecording}
          aria-label={isRecording ? "Stop recording" : "Start recording"}
        >
          <svg className={styles.icon} viewBox="0 0 24 24" fill="currentColor">
            <path d="M12 14a3 3 0 0 0 3-3V6a3 3 0 0 0-6 0v5a3 3 0 0 0 3 3z" />
            <path d="M19 11a1 1 0 1 0-2 0 5 5 0 0 1-10 0 1 1 0 1 0-2 0 7 7 0 0 0 6 6.92V21H9v2h6v-2h-2v-3.08A7 7 0 0 0 19 11z" />
          </svg>
        </button>

        {/* Visualizer */}
        {(isRecording || isListening) && (
          <div className={styles.visualizer}>
            {levels.map((level, i) => (
              <div
                key={i}
                className={styles.bar}
                style={{ height: `${20 + level * 60}px` }}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
