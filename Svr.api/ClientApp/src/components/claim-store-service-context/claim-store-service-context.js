import React from 'react';

const {
  Provider: ClaimStoreServiceProvider,
  Consumer: ClaimStoreServiceConsumer
} = React.createContext();

export {
  ClaimStoreServiceProvider,
  ClaimStoreServiceConsumer
};