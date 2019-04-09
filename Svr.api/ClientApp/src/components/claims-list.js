import React from 'react';
import ClaimsListItem from './claims-list-item';

export default ({ data }) => {
  const elements = data.map((item) => {
    return (
      <li /*key={item.id}*/>
        <ClaimsListItem name={item.name} />
      </li>
      );
  });
  return (
    <ul>
      {elements}
    </ul>
  );
}