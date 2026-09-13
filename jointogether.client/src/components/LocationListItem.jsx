export function LocationListItem({ location, isSelected, onSelect }) {
  return (
    <li
      className={
        "location-list-item" + (isSelected ? " location-list-item--active" : "")
      }
      onClick={onSelect}
    >
      {location.category && (
        <span className="location-list-item__badge">{location.category}</span>
      )}

      <div className="location-list-item__body">
        <h3 className="location-list-item__name">{location.name}</h3>
        <p className="location-list-item__description">{location.description}</p>
      </div>

      <span className="location-list-item__count">
        {location.quizQuestionCount} frågor
      </span>
    </li>
  );
}

export default LocationListItem;