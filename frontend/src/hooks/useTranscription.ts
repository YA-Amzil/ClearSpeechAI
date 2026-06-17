import { useState, useRef } from "react";
import {
  transcribeAudio,
  transcribeYouTube,
  TranscriptionOptions,
  TranscriptionResult,
} from "../services/transcriptionService";

export type TranscriptionStatus =
  | "idle"
  | "uploading"
  | "processing"
  | "done"
  | "error";

export interface TranscriptionState {
  status: TranscriptionStatus;
  result: TranscriptionResult | null;
  error: string | null;
  elapsedSeconds: number | null;
}

export function useTranscription() {
  const [state, setState] = useState<TranscriptionState>({
    status: "idle",
    result: null,
    error: null,
    elapsedSeconds: null,
  });

  const abortRef = useRef<AbortController | null>(null);

  function startNewJob() {
    abortRef.current?.abort();
    abortRef.current = new AbortController();

    setState({
      status: "uploading",
      result: null,
      error: null,
      elapsedSeconds: null,
    });

    return abortRef.current.signal;
  }

  async function transcribeFile(file: File, options: TranscriptionOptions) {
    const signal = startNewJob();
    const start = Date.now();

    try {
      setState((s) => ({ ...s, status: "processing" }));

      const result = await transcribeAudio(file, options, signal);
      const elapsed = Number(((Date.now() - start) / 1000).toFixed(1));

      if (result.success) {
        setState({
          status: "done",
          result,
          error: null,
          elapsedSeconds: elapsed,
        });
      } else {
        setState({
          status: "error",
          result: null,
          error: result.errorMessage ?? "Unknown error",
          elapsedSeconds: null,
        });
      }
    } catch (err: any) {
      if (err.name === "CanceledError") return;

      setState({
        status: "error",
        result: null,
        error: err.message,
        elapsedSeconds: null,
      });
    }
  }

  async function transcribeAudioBlob(
    blob: Blob,
    options: TranscriptionOptions,
  ) {
    const file = new File([blob], "recording.webm", {
      type: "audio/webm",
      lastModified: Date.now(),
    });

    await transcribeFile(file, options);
  }

  async function transcribeYoutubeUrl(
    url: string,
    options: TranscriptionOptions,
  ) {
    const signal = startNewJob();
    const start = Date.now();

    try {
      setState((s) => ({ ...s, status: "processing" }));

      const result = await transcribeYouTube(url, options);
      const elapsed = Number(((Date.now() - start) / 1000).toFixed(1));

      if (result.success) {
        setState({
          status: "done",
          result,
          error: null,
          elapsedSeconds: elapsed,
        });
      } else {
        setState({
          status: "error",
          result: null,
          error: result.errorMessage ?? "Unknown error",
          elapsedSeconds: null,
        });
      }
    } catch (err: any) {
      setState({
        status: "error",
        result: null,
        error: err.message,
        elapsedSeconds: null,
      });
    }
  }

  function reset() {
    abortRef.current?.abort();
    setState({
      status: "idle",
      result: null,
      error: null,
      elapsedSeconds: null,
    });
  }

  return {
    state,
    transcribeFile,
    transcribeAudioBlob,
    transcribeYoutubeUrl,
    reset,
  };
}
