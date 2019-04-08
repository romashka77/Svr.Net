import React, { Component } from 'react';
//import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class Claim extends Component {
  render() {
    if (!this.props.activeclaims) {
      return (<p>Выберите иск</p>);
    }
    return (
      <div>
        <h2>{this.props.activeclaims.name}</h2>
        <div>{this.props.activeclaims.id}</div>
      </div>
    );
  }
}

function mapStateToProps(state) {
  return {
    activeclaims: state.activeclaims
  };
}

export default connect(mapStateToProps)(Claim);