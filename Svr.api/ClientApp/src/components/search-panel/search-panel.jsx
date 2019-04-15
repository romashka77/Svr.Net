import React from 'react';
import './search-panel.css';


export default () => {
    return (
        <input
            type="text" 
            className="form-control search-input" 
            placeholder="Поиск..."
        />
    );
}