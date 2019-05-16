import React from 'react';
import { ClaimStoreServiceConsumer } from '../claim-store-service-context';

const withClaimStoreService = () => (Wrapped) =>
{
  return (props) =>
  {
    return (
      <ClaimStoreServiceConsumer>
        {
          (claimStoreService) =>
          {
            return (<Wrapped {...props}
              claimStoreService={claimStoreService} />);
          }
        }
      </ClaimStoreServiceConsumer>
    );
  }
};

export default withClaimStoreService;
