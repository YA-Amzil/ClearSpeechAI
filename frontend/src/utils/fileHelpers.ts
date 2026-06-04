export const ALLOWED_EXTENSIONS = ['.wav', '.mp3', '.m4a', '.ogg', '.flac', '.webm', '.mp4']
export const MAX_FILE_SIZE_BYTES = 25 * 1024 * 1024 // 25 MB

export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1024 / 1024).toFixed(2)} MB`
}

export function isValidAudioFile(file: File): { valid: boolean; error?: string } {
  const ext = '.' + file.name.split('.').pop()?.toLowerCase()
  if (!ALLOWED_EXTENSIONS.includes(ext)) {
    return { valid: false, error: `Ongeldig bestandstype "${ext}". Gebruik: ${ALLOWED_EXTENSIONS.join(', ')}` }
  }
  if (file.size > MAX_FILE_SIZE_BYTES) {
    return { valid: false, error: 'Bestand is groter dan 25 MB.' }
  }
  return { valid: true }
}

export function countWords(text: string): number {
  return text.trim().split(/\s+/).filter(Boolean).length
}
