import styles from './StatusBar.module.css'
import { TranscriptionStatus } from '../hooks/useTranscription'

interface Props { status: TranscriptionStatus }

const MESSAGES: Record<TranscriptionStatus, string> = {
  idle: "",
  uploading: "Uploading file...",
  processing: "Whisper is processing the audio...",
  done: "Transcription completed",
  error: "An error has occurred",
};


export default function StatusBar({ status }: Props) {
  if (status === 'idle') return null
  return (
    <div className={`${styles.bar} ${styles[status]}`}>
      <span className={`${styles.dot} ${status === 'processing' || status === 'uploading' ? styles.pulse : ''}`} />
      <span>{MESSAGES[status]}</span>
    </div>
  )
}
