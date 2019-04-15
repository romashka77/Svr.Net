import React from 'react';
import './claims-header.css';

export default ({ label, count }) => (
  <div className="claims-header d-flex">
        <h1>
            {label}
        </h1>
        <h2>Колличество {count}</h2>
    </div>
);