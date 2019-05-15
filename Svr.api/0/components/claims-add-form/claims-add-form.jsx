import React, { Component } from 'react';
import './claims-add-form.css';

export default class ClaimsAddForm extends Component {
    state = {
        name: ''
    }

    onNameChange = (e) => {
        this.setState({ name: e.target.value });
    };

    onSubmit = (e) => {
        e.preventDefault();//не отправлять данные на сервер, страница не перезагружается
        this.props.onAdded(this.state.name);
        this.setState({ name: '' });
    };

    render() {
        return (
            <form className="claims-add-form d-flex" onSubmit={this.onSubmit}>
                <input type="text"
                    className="form-control"
                    onChange={this.onNameChange}
                    placeholder="Введите наименование иска..."
                    value={this.state.name}
                />
                <button className="btn btn-outline-secondary" >Добавить</button>
            </form>
        );
    }
}
