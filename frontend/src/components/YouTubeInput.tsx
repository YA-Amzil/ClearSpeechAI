import styles from "./YouTubeInput.module.css";

interface Props {
  url: string;
  onChange: (v: string) => void;
}

export default function YouTubeInput({ url, onChange }: Props) {
  return (
    <div className={styles.wrapper}>
      <label className={styles.label}>YouTube URL</label>

      <input
        type="text"
        value={url}
        onChange={(e) => onChange(e.target.value)}
        placeholder="https://www.youtube.com/watch?v=..."
        className={styles.input}
      />
    </div>
  );
}
