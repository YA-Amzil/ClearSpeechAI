import styles from "./TranscribeForm.module.css";
import { TranscriptionOptions } from "../services/transcriptionService";
import {
  LANGUAGE_OPTIONS,
  FORMAT_OPTIONS,
  TEMPERATURE_OPTIONS,
} from "../utils/formatLabels";

interface Props {
  options: TranscriptionOptions;
  onChange: (opts: TranscriptionOptions) => void;
  onSubmit: () => void;
  disabled: boolean;
}

export default function TranscribeForm({
  options,
  onChange,
  onSubmit,
  disabled,
}: Props) {
  function set<K extends keyof TranscriptionOptions>(
    key: K,
    value: TranscriptionOptions[K],
  ) {
    onChange({ ...options, [key]: value });
  }

  return (
    <div className={styles.wrapper}>
      <div className={styles.row}>
        <div className={styles.field}>
          <label htmlFor="lang">Language</label>
          <select
            id="lang"
            value={options.language}
            onChange={(e) => set("language", e.target.value)}
            disabled={disabled}
          >
            {LANGUAGE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>
                {o.label}
              </option>
            ))}
          </select>
        </div>

        <div className={styles.field}>
          <label htmlFor="fmt">Output format</label>
          <select
            id="fmt"
            value={options.responseFormat}
            onChange={(e) => set("responseFormat", e.target.value)}
            disabled={disabled}
          >
            {FORMAT_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>
                {o.label}
              </option>
            ))}
          </select>
        </div>

        <div className={styles.field}>
          <label htmlFor="temp">Temperature</label>
          <select
            id="temp"
            value={String(options.temperature)}
            onChange={(e) => set("temperature", parseFloat(e.target.value))}
            disabled={disabled}
          >
            {TEMPERATURE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>
                {o.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className={styles.field}>
        <label htmlFor="prompt">
          Prompt <span className={styles.optional}>(optional)</span>
        </label>
        <input
          id="prompt"
          type="text"
          placeholder="Give the AI a hint about the topic..."
          value={options.prompt ?? ""}
          onChange={(e) => set("prompt", e.target.value || undefined)}
          disabled={disabled}
        />
      </div>

      <button className={styles.btn} onClick={onSubmit} disabled={disabled}>
        <svg
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          width="15"
          height="15"
        >
          <polygon points="5,3 19,12 5,21" fill="currentColor" stroke="none" />
        </svg>
        {disabled ? "Processing..." : "Transcribe audio"}
      </button>
    </div>
  );
}
