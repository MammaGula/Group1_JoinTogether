export function FormField({ id,label, error, ...inputProps }) {
    return (
        <div className="form-field">
            <label htmlFor={id} className="form-label">
                {label}
            </label>
            <input id={id} className="form-input" {...inputProps} />
            {error && <span className="error">{error}</span>}
        </div>
    )
}
