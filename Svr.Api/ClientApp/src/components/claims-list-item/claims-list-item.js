import React, { Component } from 'react';
import './claims-list-item.css';
import 'font-awesome/css/font-awesome.css';

export default class ClaimsListItem extends Component {

  state = {
    done: false
  };

  onNameClick = () => {
    console.log('Клик на ', this.props.name);
  };

  render() {
    const { name, onDeleted } = this.props;
    return (
      <span className="claims-list-item">
        <span className="name" onClick={this.onNameClick}>
          {name}
        </span>

        <button type="button"
          className="btn btn-outline-success btn-sm float-right">
          <i className="fa fa-exclamation" />
        </button>

        <button type="button"
          className="btn btn-outline-danger btn-sm float-right"
          onClick={onDeleted}>
          <i className="fa fa-trash-o" />
        </button>
      </span>
    );
  }
}