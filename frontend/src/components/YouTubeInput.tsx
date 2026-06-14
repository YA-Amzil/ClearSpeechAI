import styles from "./YouTubeInput.module.css";

interface Props {
  url: string;
  onChange: (v: string) => void;
  onSubmit: () => void;
}

export default function YouTubeInput({ url, onChange, onSubmit }: Props) {
  return (
    <div className={styles.wrapper}>
      <label className={styles.label}>YouTube URL</label>

      <div className={styles.row}>
        <input
          type="text"
          value={url}
          onChange={(e) => onChange(e.target.value)}
          placeholder="https://www.youtube.com/watch?v=..."
          className={styles.input}
        />

        <button className={styles.btn} onClick={onSubmit}>
          Transcribe
        </button>
      </div>
    </div>
  );
}
