export function FormField({ id, label, error, ...inputProps }) {
    return (
        <div className="form-field">
            <label htmlFor={id} className="form-field__label">
                {label}
            </label>
            <input id={id} className="form-field__input" {...inputProps} />
            {error && <span className="form-field__error">{error}</span>}
        </div>
    )
}