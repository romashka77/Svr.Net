import React from 'react';
//import Claims from '../containers/claims';
//import Claim from '../containers/claim';
import AppHeader from './app-header';
import ClaimsList from './claims-list';
import SearchPanel from './search-panel';

const ClaimsData = [
  {id: 1, name: 'Иск 0'},
  { id: 2, name: 'Иск 2' },
  { id: 3, name: 'Иск 3' },
];

export default () => (
  <div>
    <AppHeader label="Иски" />
    <SearchPanel />
    <ClaimsList data={ClaimsData} />
  </div>
);
