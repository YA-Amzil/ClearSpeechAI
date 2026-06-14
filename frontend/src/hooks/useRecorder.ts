import { useState } from "react";
import { uploadRecording } from "../services/transcriptionService";

let mediaRecorder: MediaRecorder | null = null;
let chunks: BlobPart[] = [];

export function useRecorder() {
  const [isRecording, setIsRecording] = useState(false);

  async function startRecording() {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);

    chunks = [];
    setIsRecording(true);

    mediaRecorder.ondataavailable = (e) => chunks.push(e.data);

    mediaRecorder.onstop = async () => {
      const blob = new Blob(chunks, { type: "audio/webm" });
      await uploadRecording(blob);
      setIsRecording(false);
    };

    mediaRecorder.start();
  }

  function stopRecording() {
    mediaRecorder?.stop();
  }

  return { startRecording, stopRecording, isRecording };
}
