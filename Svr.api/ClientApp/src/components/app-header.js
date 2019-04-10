import React from 'react';
import './app-header.css';

export default ({ label, count }) => (
    <div className="app-header d-flex">
        <h1>
            {label}
        </h1>
        <h2>Колличество {count}</h2>
    </div>
);