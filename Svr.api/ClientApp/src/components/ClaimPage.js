import React from 'react';
import Claims from '../containers/claims';
import Claim from '../containers/claim';

const ClaimPage = () => (
  <div>
    <h2>Иски:</h2>
    <Claims />
    <hr />
    <h3>Детали</h3>
    <Claim />
  </div>
);

export default ClaimPage;
//