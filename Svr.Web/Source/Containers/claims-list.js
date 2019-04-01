import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class ClaimsList extends Component {
    render() {
        return (
            <ol>
                <li>1</li>
                <li>2</li>
            </ol>
        );
    }
}

function mapStateToProps(state) {
    return {
        claims: state.claims
    };
}


export default connect(mapStateToProps)(ClaimsList);