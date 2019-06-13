import PropTypes from 'prop-types';
import React from 'react';
import { connect } from 'react-redux';
import getClaims from '../actions/claims';

class Claims extends React.Component {
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
                <input
                    ref={(input) => { this.idInput = input; }}
                    type="number"
                />
                <input
                    ref={(input) => { this.nameInput = input; }}
                    type="text"
                />
                <button onClick={this.addClaime.bind(this)} >
Добавить иск
                </button>
            </div>
            <div>
                <input
                    onChange={this.findClaime.bind(this)}
                    ref={(input) => { this.searchInput = input; }}
                    type="text"
                />
                <button onClick={this.findClaime.bind(this)} >
Поиск
                </button>
            </div>
            <div>
                <button onClick={this.props.onGetClaims} >
Получить список исков
                </button>
            </div>
            <ul>
                {
            this.props.claims.map((claim, index) => (<li key={index}>
                {claim.id}
                {claim.name}
                                                     </li>))
          }
            </ul>
        </div>
    );
  }
}
Claims.propTypes = {
  claims: PropTypes.array,
  onAddClaim: PropTypes.func,
  onFindClaim: PropTypes.func,
  onGetClaims: PropTypes.func,
};

export default connect(
  state => ({
    claims: state.claims
      .filter(claim => claim.name.toUpperCase().includes(state.filterClaims.toUpperCase())),
  }),
  dispatch => ({
    onAddClaim: (claimId, claimName) => {
      const payload = {
        id: claimId,
        name: claimName,
      };
      dispatch({ type: 'ADD_CLAIM', payload });
    },
    onFindClaim: (searchClaim) => {
      dispatch({ type: 'FIND_CLAIM', payload: searchClaim });
    },
    onGetClaims: () => {
      dispatch(getClaims());
    },
  }),
)(Claims);
