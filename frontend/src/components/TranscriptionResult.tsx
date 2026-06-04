import { useState } from 'react'
import styles from './TranscriptionResult.module.css'
import { TranscriptionResult as Result } from '../services/transcriptionService'
import { countWords } from '../utils/fileHelpers'

interface Props {
  result: Result
  elapsedSeconds: number | null
}

export default function TranscriptionResult({ result, elapsedSeconds }: Props) {
  const [copied, setCopied] = useState(false)
  const text = result.text ?? ''
  const words = countWords(text)

  function copy() {
    navigator.clipboard.writeText(text).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 1500)
    })
  }

  return (
    <div className={styles.card}>
      <div className={styles.header}>
        <div className={styles.label}>
          Transcriptie
          <span className={styles.badge}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" width="11" height="11">
              <path d="M20 6L9 17l-5-5" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
            {result.language ?? 'auto'}
          </span>
        </div>
        <button className={styles.copyBtn} onClick={copy}>
          {copied
            ? <><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" width="13" height="13"><path d="M20 6L9 17l-5-5" strokeLinecap="round" strokeLinejoin="round"/></svg> gekopieerd</>
            : <><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" width="13" height="13"><rect x="9" y="9" width="13" height="13" rx="2"/><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/></svg> kopiëren</>
          }
        </button>
      </div>
      <pre className={styles.text}>{text}</pre>
      <div className={styles.meta}>
        <span><strong>{words.toLocaleString('nl')}</strong> woorden</span>
        <span><strong>{text.length.toLocaleString('nl')}</strong> tekens</span>
        {elapsedSeconds !== null && <span><strong>{elapsedSeconds}s</strong> verwerktijd</span>}
        {result.format && <span><strong>{result.format}</strong></span>}
      </div>
    </div>
  )
}
