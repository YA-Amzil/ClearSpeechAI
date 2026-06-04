import { useState, useRef } from 'react'
import { transcribeAudio, TranscriptionOptions, TranscriptionResult } from '../services/transcriptionService'

export type TranscriptionStatus = 'idle' | 'uploading' | 'processing' | 'done' | 'error'

export interface TranscriptionState {
  status: TranscriptionStatus
  result: TranscriptionResult | null
  error: string | null
  elapsedSeconds: number | null
}

export function useTranscription() {
  const [state, setState] = useState<TranscriptionState>({
    status: 'idle',
    result: null,
    error: null,
    elapsedSeconds: null
  })
  const abortRef = useRef<AbortController | null>(null)

  async function run(file: File, options: TranscriptionOptions) {
    abortRef.current?.abort()
    abortRef.current = new AbortController()

    setState({ status: 'uploading', result: null, error: null, elapsedSeconds: null })
    const start = Date.now()

    try {
      setState(s => ({ ...s, status: 'processing' }))
      const result = await transcribeAudio(file, options, abortRef.current.signal)
      const elapsed = parseFloat(((Date.now() - start) / 1000).toFixed(1))

      if (result.success) {
        setState({ status: 'done', result, error: null, elapsedSeconds: elapsed })
      } else {
        setState({ status: 'error', result: null, error: result.errorMessage ?? 'Onbekende fout', elapsedSeconds: null })
      }
    } catch (err: unknown) {
      if ((err as Error).name === 'CanceledError') return
      setState({ status: 'error', result: null, error: (err as Error).message, elapsedSeconds: null })
    }
  }

  function reset() {
    abortRef.current?.abort()
    setState({ status: 'idle', result: null, error: null, elapsedSeconds: null })
  }

  return { state, run, reset }
}
