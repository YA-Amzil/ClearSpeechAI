import { useState, useCallback } from 'react'
import { isValidAudioFile } from '../utils/fileHelpers'

export function useFileDropzone(onFile: (file: File) => void) {
  const [isDragging, setIsDragging] = useState(false)
  const [dropError, setDropError] = useState<string | null>(null)

  const handleDragOver = useCallback((e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setIsDragging(true)
  }, [])

  const handleDragLeave = useCallback(() => setIsDragging(false), [])

  const handleDrop = useCallback((e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setIsDragging(false)
    const file = e.dataTransfer.files[0]
    if (!file) return
    const check = isValidAudioFile(file)
    if (!check.valid) { setDropError(check.error ?? 'Ongeldig bestand'); return }
    setDropError(null)
    onFile(file)
  }, [onFile])

  const handleInputChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    const check = isValidAudioFile(file)
    if (!check.valid) { setDropError(check.error ?? 'Ongeldig bestand'); return }
    setDropError(null)
    onFile(file)
  }, [onFile])

  return { isDragging, dropError, handleDragOver, handleDragLeave, handleDrop, handleInputChange }
}
