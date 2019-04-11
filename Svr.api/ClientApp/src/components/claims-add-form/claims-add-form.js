import React, { Component } from 'react';
import './claims-add-form.css';

export default class ClaimsAddForm extends Component {
  render() {
    return (
      <div className="claims-add-form">
        <input type="text" className="form-control" />
        <button className="btn btn-outline-secondary" onClick={() => this.props.onAdded('test')}>Добавить</button>
      </div>
      );
  };
}
