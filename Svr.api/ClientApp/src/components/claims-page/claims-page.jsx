import React, { Component } from 'react';
import ClaimsHeader from '../claims-header';
import ClaimsList from '../claims-list';
import SearchPanel from '../search-panel';
import ItemStatusFilter from '../item-status-filter';
import ClaimsAddForm from '../claims-add-form';
import './claims-page.css';



export default class ClaimsPage extends Component {
    maxId = 100;
    state = {
        claimsData: [
            this.createClaimsItem('Иск 1'),
            this.createClaimsItem('Иск 2'),
            this.createClaimsItem('Иск 3'),
        ],
        searchText: ''
    };

    createClaimsItem(name) {
        return {
            id: this.maxId++,
            name
        }
    }

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

    addItem = (text) => {
        const newItem = this.createClaimsItem(text);
        this.setState(({ claimsData }) => {
            return { claimsData: [...claimsData, newItem] };
        });
    };

    onSearchChange = (searchText) => {
        this.setState({ searchText });
    }

    search(items, searchText) {
        if (searchText.length === 0) {
            return items;
        }
        return items.filter((item) => {
            return item.name.toUpperCase().indexOf(searchText.toUpperCase()) > -1;
        });
    }

    

    render() {
        const { claimsData, searchText } = this.state;
        const visibleItems = this.search(claimsData, searchText);
        return (
            <div className="claims-page">
                <ClaimsHeader label="Иски" count={claimsData.length} />
                <div className="top-panel d-flex">
                    <SearchPanel onSearchChange={this.onSearchChange} />
                    <ItemStatusFilter />
                </div>
                <ClaimsList data={visibleItems} onDeleted={this.deleteItem} />
                <ClaimsAddForm onAdded={this.addItem} />
            </div>
        );
    }
}
