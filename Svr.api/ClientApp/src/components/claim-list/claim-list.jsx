import React, { Component } from 'react';
import ClaimListItem from '../claim-list-item';

//import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

import { withClaimStoreService } from '../hoc';
//import { fetchBooks, claimAddedToCart } from '../../actions';
//import { compose } from '../../utils';

import Spinner from '../spinner';
import ErrorIndicator from '../error-indicator';

import './claim-list.css';

//https://react-bootstrap-table.github.io/react-bootstrap-table2/storybook/index.html?selectedKind=Data&selectedStory=Load%20data%20with%20Filter%20and%20Pagination&full=0&addons=1&stories=1&panelRight=0&addonPanel=storybook%2Factions%2Factions-panel
import BootstrapTable from 'react-bootstrap-table-next';
import filterFactory, { textFilter } from 'react-bootstrap-table2-filter';
import paginationFactory from 'react-bootstrap-table2-paginator';

class ClaimList extends Component 
{
  componentDidMount() {
    const { claimStoreService } = this.props;
    const data = claimStoreService.getClaims();
    console.log(data);
    const col = claimStoreService.getColumns();
    console.log(col);
  }

  render()
  {
    const { claims, columns } = this.props;
    return (
      <div>
        <ul className="claim-list">
          {
            claims.map((claim) =>
            {
              return (
                <li key={claim.id}>
                  <ClaimListItem
                    claim={claim}
                  //onAddedToCart={() => onAddedToCart(claim.id)}
                  />
                </li>
              );
            })
          }
        </ul>
        <BootstrapTable
          bootstrap4
          striped
          hover
          condensed
          bordered={false}
          keyField='id'
          data={claims}
          columns={columns}
          //filter={filterFactory()}
          pagination={paginationFactory()}
        />
      </div>
    );
  }

};

const mapStateToProps = (state) =>
{
  return {
    claims: state.claims,
    columns: state.columns
  };
};

const mapDispatchToProps = (dispatch) => {

}

export default withClaimStoreService()(connect(mapStateToProps)(ClaimList));

//class ClaimListContainer extends Component {

//  //componentDidMount() {
//  //  this.props.fetchBooks();
//  //}

//  render() {
//    const { caims, loading, error,
//        //onAddedToCart
//    } = this.props;

//    if (loading) {
//      return <Spinner />;
//    }

//    if (error) {
//      return <ErrorIndicator />;
//    }

//    return <ClaimList claims={claims}
//      //onAddedToCart={onAddedToCart}
//    />;
//  }
//}



//const mapDispatchToProps = (dispatch, { claimStoreService }) => {

//  return bindActionCreators({
//    fetchClaims: fetchBooks(claimStoreService),
//    onAddedToCart: claimAddedToCart
//  }, dispatch);
//};

//export default compose(
//  withClaimStoreService(),
//  connect(mapStateToProps, mapDispatchToProps)
//)(ClaimListContainer);
