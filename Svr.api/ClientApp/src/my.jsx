import React from 'react';
import ReactDom from 'react-dom';
import { Provider } from 'react-redux';
import { BrowserRouter as Router } from 'react-router-dom';

import App from './components/app';
import ErrorBoundry from './components/error-boundry';
import ClaimStoreService from './services/claim-store-service';
import { ClaimStoreServiceProvider } from './components/claim-store-service-context';

import store from './store';

const claimStoreService = new ClaimStoreService();
ReactDom.render(
  <Provider store={store}>
      <ErrorBoundry>
          <ClaimStoreServiceProvider value={claimStoreService}>
              <Router>
                  <App />
              </Router>
          </ClaimStoreServiceProvider>
      </ErrorBoundry>
  </Provider>,
  document.getElementById('root')
);