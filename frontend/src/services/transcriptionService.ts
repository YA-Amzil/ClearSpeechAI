import axios from "axios";

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
    { signal },
  );

  return response.data;
}

export async function transcribeYouTube(
  url: string,
  options?: Partial<TranscriptionOptions>,
): Promise<TranscriptionResult> {
  const formData = new FormData();
  formData.append("Url", url);
  formData.append("language", options?.language ?? "auto");
  formData.append("responseFormat", options?.responseFormat ?? "json");
  formData.append("temperature", String(options?.temperature ?? 0));
  if (options?.prompt) formData.append("prompt", options.prompt);

  const response = await axios.post<TranscriptionResult>(
    `${BASE_URL}/api/transcription/youtube`,
    formData,
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
  );

  return response.data;
}
