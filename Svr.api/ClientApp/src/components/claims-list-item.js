import React from 'react';
import './claims-list-item.css';

export default ({ name }) => {
    return (
        <span className="claims-list-item">
            {name}
        </span>
    );
}