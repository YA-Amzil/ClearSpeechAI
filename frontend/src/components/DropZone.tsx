import { useRef } from 'react'
import styles from './DropZone.module.css'
import { useFileDropzone } from '../hooks/useFileDropzone'
import { ALLOWED_EXTENSIONS } from '../utils/fileHelpers'

interface Props {
  onFile: (file: File) => void
}

export default function DropZone({ onFile }: Props) {
  const inputRef = useRef<HTMLInputElement>(null)
  const {
    isDragging,
    dropError,
    handleDragOver,
    handleDragLeave,
    handleDrop,
    handleInputChange
  } = useFileDropzone(onFile)

  return (
    <div>
      <div
        className={`${styles.zone} ${isDragging ? styles.dragging : ''}`}
        onClick={() => inputRef.current?.click()}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        role="button"
        tabIndex={0}
        onKeyDown={(e) => e.key === 'Enter' && inputRef.current?.click()}
        aria-label="Sleep audiobestand hierheen of klik om te bladeren"
      >
        <svg className={styles.icon} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
          <path d="M12 2a3 3 0 0 1 3 3v7a3 3 0 0 1-6 0V5a3 3 0 0 1 3-3z" />
          <path d="M19 10v2a7 7 0 0 1-14 0v-2" strokeLinecap="round" />
          <line x1="12" y1="19" x2="12" y2="23" strokeLinecap="round" />
          <line x1="8" y1="23" x2="16" y2="23" strokeLinecap="round" />
        </svg>
        <p className={styles.title}>Sleep een audiobestand hierheen</p>
        <p className={styles.sub}>of klik om te bladeren</p>
        <p className={styles.hint}>
          Ondersteund: {ALLOWED_EXTENSIONS.join(' ')} · max 25 MB
        </p>
      </div>
      {dropError && <p className={styles.error}>{dropError}</p>}
      <input
        ref={inputRef}
        type="file"
        accept="audio/*"
        style={{ display: 'none' }}
        onChange={handleInputChange}
      />
    </div>
  )
}
