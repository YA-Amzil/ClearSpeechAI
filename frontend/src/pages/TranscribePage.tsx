import { useState } from 'react'
import styles from './TranscribePage.module.css'
import DropZone from '../components/DropZone'
import FileInfo from '../components/FileInfo'
import TranscribeForm from '../components/TranscribeForm'
import StatusBar from '../components/StatusBar'
import TranscriptionResult from '../components/TranscriptionResult'
import { TranscriptionOptions } from '../services/transcriptionService'
import { useTranscription } from '../hooks/useTranscription'

const DEFAULT_OPTIONS: TranscriptionOptions = {
  language: 'en',
  responseFormat: 'json',
  temperature: 0,
  prompt: undefined
}

export default function TranscribePage() {
  const [file, setFile] = useState<File | null>(null)
  const [options, setOptions] = useState<TranscriptionOptions>(DEFAULT_OPTIONS)
  const { state, run, reset } = useTranscription()

  const isRunning = state.status === 'uploading' || state.status === 'processing'

  function handleFile(f: File) {
    setFile(f)
    reset()
  }

  function handleClear() {
    setFile(null)
    reset()
  }

  function handleSubmit() {
    if (!file) return
    run(file, options)
  }

  return (
    <main className={styles.main}>
      <div className={styles.container}>
        {/* Header */}
        <header className={styles.header}>
          <h1 className={styles.title}>ClearSpeechAI</h1>
          <p className={styles.sub}>// audio → tekst via whisper-1</p>
        </header>

        {/* Upload */}
        <section className={styles.card}>
          {file
            ? <FileInfo file={file} onClear={handleClear} />
            : <DropZone onFile={handleFile} />
          }
        </section>

        {/* Settings + Submit */}
        <section className={styles.card}>
          <TranscribeForm
            options={options}
            onChange={setOptions}
            onSubmit={handleSubmit}
            disabled={!file || isRunning}
          />
        </section>

        {/* Error */}
        {state.status === 'error' && state.error && (
          <div className={styles.errorBox}>{state.error}</div>
        )}

        {/* Status */}
        <StatusBar status={state.status} />

        {/* Result */}
        {state.status === 'done' && state.result && (
          <TranscriptionResult result={state.result} elapsedSeconds={state.elapsedSeconds} />
        )}
      </div>
    </main>
  )
}
