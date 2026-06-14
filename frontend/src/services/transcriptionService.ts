import axios, { AxiosError } from "axios";

const BASE_URL =
  (import.meta.env["VITE_API_BASE_URL"] as string | undefined) ??
  "http://localhost:5000";

export interface TranscriptionOptions {
  language: string;
  responseFormat: string;
  temperature: number;
  prompt?: string;
}

export interface TranscriptionResult {
  success: boolean;
  text?: string;
  language?: string;
  format?: string;
  errorMessage?: string;
  processedAt?: string;
}

export async function transcribeAudio(
  file: File,
  options: TranscriptionOptions,
  signal?: AbortSignal,
): Promise<TranscriptionResult> {
  const formData = new FormData();
  formData.append("audioFile", file, file.name);
  formData.append("language", options.language);
  formData.append("responseFormat", options.responseFormat);
  formData.append("temperature", String(options.temperature));
  if (options.prompt) formData.append("prompt", options.prompt);

  const response = await axios.post<TranscriptionResult>(
    `${BASE_URL}/api/transcription/transcribe`,
    formData,
    {
      headers: { "Content-Type": "multipart/form-data" },
      signal,
    },
  );

  return response.data;
}

export async function uploadRecording(
  blob: Blob,
  options?: Partial<TranscriptionOptions>,
): Promise<TranscriptionResult> {
  const formData = new FormData();
  formData.append("audioFile", blob, "recording.webm");
  formData.append("language", options?.language ?? "auto");
  formData.append("responseFormat", options?.responseFormat ?? "json");
  formData.append("temperature", String(options?.temperature ?? 0));
  if (options?.prompt) formData.append("prompt", options.prompt);

  const response = await axios.post<TranscriptionResult>(
    `${BASE_URL}/api/transcription/transcribe`,
    formData,
    {
      headers: { "Content-Type": "multipart/form-data" },
    },
  );

  return response.data;
}

export async function transcribeYouTube(
  url: string,
  options?: Partial<TranscriptionOptions>,
): Promise<TranscriptionResult> {
  const formData = new FormData();
  formData.append("url", url);
  formData.append("language", options?.language ?? "auto");
  formData.append("responseFormat", options?.responseFormat ?? "json");
  formData.append("temperature", String(options?.temperature ?? 0));
  if (options?.prompt) formData.append("prompt", options.prompt);

  const response = await axios.post<TranscriptionResult>(
    `${BASE_URL}/api/transcription/youtube`,
    formData,
    {
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
    },
  );

  return response.data;
}

export async function checkHealth(): Promise<boolean> {
  try {
    await axios.get(`${BASE_URL}/api/transcription/health`, { timeout: 3000 });
    return true;
  } catch (err) {
    const _e = err as AxiosError;
    console.warn("Health check failed:", _e.message);
    return false;
  }
}
