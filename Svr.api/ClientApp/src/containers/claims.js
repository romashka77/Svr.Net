import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { select } from '../actions/claims'

class Claims extends Component {

  ShowList() {
    return this.props.claims.map((claim) => {
      return (
        <li onClick={() => this.props.select(claim)} key={claim.id}>{claim.name}</li>
      );
    });
  }

  render() {
    return (
        <ol>
          {this.ShowList()}
        </ol>
    );
  }
}

function mapStateToProps(state) {
  return {
    claims: state.claims
  };
}

function matchDispatchToProps(dispatch) {
  return bindActionCreators({ select: select }, dispatch);
}

export default connect(mapStateToProps, matchDispatchToProps)(Claims);