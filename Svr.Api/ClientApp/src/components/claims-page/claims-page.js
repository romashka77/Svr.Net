import React, { Component } from 'react';
import ClaimsHeader from '../claims-header';
import ClaimsList from '../claims-list';
import SearchPanel from '../search-panel';
import ItemStatusFilter from '../item-status-filter';
import './claims-page.css';



export default class ClaimsPage extends Component {

  state = {
    claimsData: [
      { id: 1, name: 'Иск 1' },
      { id: 2, name: 'Иск 2' },
      { id: 3, name: 'Иск 3' },
    ]
  };

  deleteItem = (id) => {
    this.setState(({ claimsData }) => {
      const index = claimsData.findIndex((el) => el.id === id);
      //claimsData.splice(index, 1);
      const newArray = [
        ...claimsData.slice(0, index),
        ...claimsData.slice(index + 1)
      ];
      return { claimsData: newArray };
    });
  };

  render() {
    return (
      <div className="claims-page">
        <ClaimsHeader label="Иски" count={this.state.claimsData.length} />
        <div className="top-panel d-flex">
          <SearchPanel />
          <ItemStatusFilter />
        </div>
        <ClaimsList data={this.state.claimsData} onDeleted={this.deleteItem} />
      </div>
    );
  }
}
