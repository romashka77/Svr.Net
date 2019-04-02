import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
//import { disconnect } from 'cluster';

class ClaimsList extends Component {
    addClime() {
        console.log('addClime', this.idInput.value);
        console.log('addClime', this.nameInput.value);
        this.props.onAddClaim(this.idInput.value, this.nameInput.value);
        this.idInput.value = '';
        this.nameInput.value = '';
    }
    render() {
        console.log(this.props.claims);
        return (
            <div>
                <input type="number" ref={(input) => { this.idInput = input }} />
                <input type="text" ref={(input) => { this.nameInput = input }} />
                <button onClick={this.addClime.bind(this)} >Добавить иск</button>
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
    state => ({ claims: state.claims }),
    dispatch => ({
        onAddClaim: (claimId, claimName) => {
            dispatch({ type: 'ADD_CLAIM', claim: { id: claimId, name: claimName } })
        }
    })
)(ClaimsList);
//export default connect(mapStateToProps)(ClaimsList);