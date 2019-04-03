import React, { Component } from 'react';
import { connect } from 'react-redux';
import { getClaims } from '../actions/claims';

class ClaimsList extends Component {
    addClaime() {
        this.props.onAddClaim(this.idInput.value, this.nameInput.value);
        this.idInput.value = '';
        this.nameInput.value = '';
    }
    findClaime() {
        this.props.onFindClaim(this.searchInput.value);
    }
    render() {
        return (
            <div>
                <div>
                    <input type="number" ref={(input) => { this.idInput = input }} />
                    <input type="text" ref={(input) => { this.nameInput = input }} />
                    <button onClick={this.addClaime.bind(this)} >Добавить иск</button>
                </div>
                <div>
                    <input type="text" onChange={this.findClaime.bind(this)} ref={(input) => { this.searchInput = input }} />
                    <button onClick={this.findClaime.bind(this)} >Поиск</button>
                </div>
                <div>
                    <button onClick={this.props.onGetClaims} >Получить список исков</button>
                </div>
                <ul>
                    {
                        this.props.claims.map((claim, index) =>
                            <li key={index}>{claim.id} {claim.name} </li>
                        )
                    }
                </ul>
            </div>
        );
    }
}

//function mapStateToProps(state) {
//    return {
//        claims: state.claims
//    };
//}

export default connect(
    state => ({
        claims: state.claims.filter(claim => claim.name.toUpperCase().includes(state.filterClaims.toUpperCase()))
    }),
    dispatch => ({
        onAddClaim: (claimId, claimName) => {
            const payload = {
                id: claimId,
                name: claimName
            };
            dispatch({ type: 'ADD_CLAIM', payload });
        },
        onFindClaim: (searchClaim) => {
            dispatch({ type: 'FIND_CLAIM', payload: searchClaim });
        },
        onGetClaims: () => {
            dispatch(getClaims());
        }
    })
)(ClaimsList);