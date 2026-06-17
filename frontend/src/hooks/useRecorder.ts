import { useState, useEffect } from "react";

let mediaRecorder: MediaRecorder | null = null;
let chunks: BlobPart[] = [];
let audioContext: AudioContext | null = null;
let analyser: AnalyserNode | null = null;
let source: MediaStreamAudioSourceNode | null = null;

export function useRecorder() {
  const [isRecording, setIsRecording] = useState(false);
  const [isListening, setIsListening] = useState(false);
  const [levels, setLevels] = useState<number[]>([0, 0, 0, 0, 0]);
  const [recordedBlob, setRecordedBlob] = useState<Blob | null>(null);

  async function startRecording() {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

    mediaRecorder = new MediaRecorder(stream);
    chunks = [];
    setRecordedBlob(null);
    setIsRecording(true);
    setIsListening(true);

    audioContext = new AudioContext();
    analyser = audioContext.createAnalyser();
    analyser.fftSize = 64;
    source = audioContext.createMediaStreamSource(stream);
    source.connect(analyser);

    const dataArray = new Uint8Array(analyser.frequencyBinCount);

    const tick = () => {
      if (!analyser) return;

      analyser.getByteFrequencyData(dataArray);

      const bands = 5;
      const bandSize = Math.floor(dataArray.length / bands);

      const newLevels = Array.from({ length: bands }, (_, i) => {
        const slice = dataArray.slice(i * bandSize, (i + 1) * bandSize);
        const avg = slice.reduce((s, v) => s + v, 0) / slice.length;
        return avg / 255;
      });

      setLevels(newLevels);

      if (isRecording) requestAnimationFrame(tick);
    };

    requestAnimationFrame(tick);

    mediaRecorder.ondataavailable = (e) => chunks.push(e.data);

    mediaRecorder.onstop = () => {
      setIsListening(false);
      setIsRecording(false);

      if (audioContext) audioContext.close();

      const blob = new Blob(chunks, { type: "audio/webm" });
      setRecordedBlob(blob);
      setLevels([0, 0, 0, 0, 0]);
    };

    mediaRecorder.start();
  }

  function stopRecording() {
    mediaRecorder?.stop();
  }

  return {
    startRecording,
    stopRecording,
    isRecording,
    isListening,
    levels,
    recordedBlob,
  };
}
