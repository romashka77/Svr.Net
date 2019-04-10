import React from 'react';
import ClaimsListItem from './claims-list-item';
import './claims-list.css';

export default ({ data }) => {
    const elements = data.map((item) => {
        const { id, ...itemProps } = item;
        //console.log('itemProps', itemProps);
        return (
            <li key={id} className="list-group-item">
                <ClaimsListItem {...itemProps} />
            </li>
        );
    });
    return (
        <ul className="list-group claims-list">
            {elements}
        </ul>
    );
}