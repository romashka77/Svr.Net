import React from 'react';

const {
  Provider: claimStoreServiceProvider,
  Consumer: claimStoreServiceConsumer
} = React.createContext();

export {
  claimStoreServiceProvider,
  claimStoreServiceConsumer
};
