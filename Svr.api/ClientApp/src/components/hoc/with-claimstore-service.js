import React from 'react';
import { BookstoreServiceConsumer } from '../claim-store-service-context';

const withClaimStoreService = () => (Wrapped) => {

  return (props) => {
    return (
      <BookstoreServiceConsumer>
        {
          (bookstoreService) => {
            return (<Wrapped {...props}
                     bookstoreService={bookstoreService}/>);
          }
        }
      </BookstoreServiceConsumer>
    );
  }
};

export default withClaimStoreService;
