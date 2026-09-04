// An original, abstract line-art map: a handful of streets and place
// markers standing in for "activities happening around the city."
// It intentionally avoids mimicking any real map provider's tile style.
export function CityMapIllustration() {
  return (
    <svg
      viewBox="0 0 400 320"
      role="img"
      aria-label="Illustration of a city map with pins marking nearby activities"
      className="city-map"
    >
      <rect x="0" y="0" width="400" height="320" rx="20" className="city-map__ground" />

      <path
        d="M0 90 C 90 60, 140 130, 230 100 S 340 40, 400 70"
        className="city-map__road"
      />
      <path
        d="M0 210 C 100 190, 160 250, 250 220 S 340 260, 400 230"
        className="city-map__road"
      />
      <path d="M60 0 C 40 90, 90 150, 70 320" className="city-map__road" />
      <path d="M300 0 C 320 100, 270 180, 310 320" className="city-map__road" />
      <path d="M0 150 L 400 150" className="city-map__road city-map__road--thin" />
      <path d="M200 0 L 200 320" className="city-map__road city-map__road--thin" />

      <path
        d="M140 40 C 120 40, 110 60, 130 90 L 180 150 C 195 130, 210 100, 195 65 C 185 45, 160 40, 140 40 Z"
        className="city-map__block"
      />
      <path
        d="M250 170 C 230 170, 220 190, 235 215 L 270 260 C 285 245, 300 220, 288 190 C 280 172, 265 170, 250 170 Z"
        className="city-map__block city-map__block--soft"
      />

      <g className="city-map__pin">
        <circle cx="255" cy="118" r="7" />
        <circle cx="255" cy="118" r="16" className="city-map__pin-ring" />
      </g>
      <g className="city-map__pin city-map__pin--muted">
        <circle cx="110" cy="200" r="6" />
      </g>
      <g className="city-map__pin city-map__pin--muted">
        <circle cx="320" cy="90" r="6" />
      </g>
      <g className="city-map__pin city-map__pin--muted">
        <circle cx="170" cy="250" r="6" />
      </g>
    </svg>
  )
}