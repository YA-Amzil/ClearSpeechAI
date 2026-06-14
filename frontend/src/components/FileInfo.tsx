import styles from "./FileInfo.module.css";
import { formatFileSize } from "../utils/fileHelpers";

interface Props {
  file: File;
  onClear: () => void;
}

export default function FileInfo({ file, onClear }: Props) {
  return (
    <div className={styles.wrapper}>
      {/* File icon */}
      <svg
        className={styles.icon}
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      >
        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
        <polyline points="14 2 14 8 20 8" />
        <path d="M10 13v4" />
        <circle cx="10" cy="17" r="1" />
        <path d="M14 13v4" />
        <circle cx="14" cy="17" r="1" />
      </svg>

      <span className={styles.name}>{file.name}</span>
      <span className={styles.size}>{formatFileSize(file.size)}</span>

      <button
        className={styles.clear}
        onClick={onClear}
        aria-label="Remove file"
      >
        <svg
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          width="14"
          height="14"
        >
          <path d="M18 6L6 18M6 6l12 12" strokeLinecap="round" />
        </svg>
      </button>
    </div>
  );
}
